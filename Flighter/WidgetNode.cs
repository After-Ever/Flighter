using System;
using System.Collections.Generic;

using UnityEngine;

namespace Flighter
{
    public struct NodeLayout
    {
        public Vector2 size, offset;

        public NodeLayout(Vector2 size, Vector2 offset)
        {
            this.size = size;
            this.offset = offset;
        }

        public NodeLayout(float width, float height, float x = 0, float y = 0)
        {
            size = new Vector2(width, height);
            offset = new Vector2(x, y);
        }
    }

    /// <summary>
    /// Delegate to use for getting an offset given a size.
    /// </summary>
    /// <param name="size"></param>
    /// <returns>The offset based on <para>size</para></returns>
    public delegate Vector2 OffsetCallback(Vector2 size);
    
    public class WidgetNode
    {
        public readonly Widget widget;
        public readonly BuildContext buildContext;

        /// <summary>
        /// The layout properties of this node.
        /// </summary>
        public NodeLayout Layout { get; private set; }

        ElementNode elementNode;

        WidgetNode parent;
        List<WidgetNode> children = new List<WidgetNode>();

        Queue<WidgetNode> inheritedChildren;
        bool isConstructing = true;

        // TODO: Describe all this...
        public WidgetNode(
            Widget widget, 
            BuildContext buildContext, 
            WidgetNode parent,
            ElementNode inheritedElementNode = null, 
            Queue<WidgetNode> inheritedChildren = null)
        {
            this.widget = widget ?? throw new ArgumentNullException("WidgetNode's widget must not be null.");
            this.buildContext = buildContext;
            this.parent = parent;

            this.inheritedChildren = inheritedChildren;

            if (this.widget is StatelessWidget slw)
            {
                if (inheritedElementNode != null)
                    throw new Exception("StatelessWidget cannot inherit an element node!");

                var child = slw.Build(this.buildContext);
                var childNode = Add(child, this.buildContext);

                Layout = childNode.Layout;
            }
            else if (this.widget is StatefulWidget sfw)
            {
                if (inheritedElementNode != null)
                {
                    if (inheritedChildren?.Count != 1)
                        throw new Exception("If a StatefulWidget inherits an ElementNode," +
                            "it should inherit exactly one child.");
                    ConnectInheritedElementNode(inheritedElementNode);

                    // Directly adopt the inherited child. This is okay
                    // because the state element will be marked dirty,
                    // rebuilding its widget, and thus causing this widget
                    // to go through normal replacement.
                    var childNode = inheritedChildren.Dequeue();
                    AdoptNode(childNode);

                    Layout = childNode.Layout;
                }
                else
                {
                    var state = sfw.CreateState();
                    // Must connect the element before building the rest of the tree.
                    ConnectElement(new StateElement(state));

                    // Manually do the first build. The rest will be handled with state updates.
                    var child = state.Build(this.buildContext);
                    var childNode = Add(child, this.buildContext);

                    Layout = childNode.Layout;
                }
            }
            else if (this.widget is LayoutWidget lw)
            {
                if (lw is DisplayWidget dw)
                {
                    if (inheritedElementNode != null)
                    {
                        ConnectInheritedElementNode(inheritedElementNode);
                    }
                    else
                    {
                        ConnectElement(dw.CreateElement());
                    }
                }
                else if (inheritedElementNode != null)
                    throw new Exception("This widget should not inherit ElementNode.");

                var buildResults = lw.Layout(buildContext, this);

                Layout = new NodeLayout(buildResults.size, Vector2.zero);
            }
            else
            {
                throw new Exception("Unsupported Widget type: " + this.widget.GetType());
            }

            // Remove any inherited children which have not been used.
            PruneInheritedChildren();

            isConstructing = false;
        }

        /// <summary>
        /// Add a child widget to this.
        /// All additions must occure durring the constructor; after construction,
        /// the sub tree is final.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public WidgetNode Add(Widget widget, BuildContext context)
        {
            if (!isConstructing)
                throw new Exception("Cannot add children unless the node is being constructed.");

            ElementNode childElementNode = null;
            Queue<WidgetNode> orphans = null;

            if ((inheritedChildren?.Count ?? 0) > 0)
            {
                var c = inheritedChildren.Dequeue();
                // The context and widget are the same, so no need to add the new widget.
                if (context.Equals(c.buildContext) && widget.IsSame(c.widget))
                {
                    AdoptNode(c);
                    return c;
                }
                    
                if (widget.CanReplace(c.widget))
                {
                    childElementNode = c.elementNode;
                    c.elementNode = null;
                    orphans = c.EmancipateChildrenAndPrune();
                }
                else
                {
                    c.Prune();
                }
            }

            WidgetNode child = new WidgetNode(
                                        widget: widget,
                                        buildContext: context,
                                        parent: this,
                                        inheritedElementNode: childElementNode,
                                        inheritedChildren: orphans);

            children.Add(child);
            return child;
        }

        public void SetOffset(Vector2 offset)
        {
            if (elementNode == null)
                throw new Exception("Only Widgets with connected element node can have an offset.");
            if (!elementNode.IsDirty)
                throw new Exception("Cannot set offset when attached element node is clean.");

            Layout = new NodeLayout(Layout.size, offset);
        }

        /// <summary>
        /// Use <paramref name="newKids"/> to replace the current set of children,
        /// if there are any. The same replacement rules will apply as when inheriting
        /// children.
        /// Old children will be pruned if not re-adopted.
        /// </summary>
        /// <param name="newKids"></param>
        public void ReplaceChildren(List<(Widget,BuildContext)> newKids)
        {
            isConstructing = true;

            inheritedChildren = EmancipateChildren();
            newKids.ForEach((c) => Add(c.Item1,c.Item2));
            PruneInheritedChildren();

            isConstructing = false;
        }

        /// <summary>
        /// Connect the given node to this.
        /// </summary>
        /// <param name="node">The node to adopt. This must be a free node.</param>
        void AdoptNode(WidgetNode node)
        {
            children.Add(node);
            node.parent = this;

            var nearestAncestorElementNode = NearestAncestorElementNode();
            node.GetElementSurface().ForEach((e) => ConnectInheritedElementNode(e, nearestAncestorElementNode));
        }

        /// <summary>
        /// Disconnect the element surface.
        /// </summary>
        /// <returns>The element surface, ie the nodes emancipated.</returns>
        List<ElementNode> EmancipateElementSurface()
        {
            var surface = GetElementSurface();
            surface.ForEach((e) => e.Emancipate());
            return surface;
        }

        /// <summary>
        /// Connect the given element node to this.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nearestAncestor">Manually pass the nearest ancestor to avoid searching each call.</param>
        void ConnectInheritedElementNode(ElementNode node, ElementNode nearestAncestor = null)
        {
            nearestAncestor = nearestAncestor ?? NearestAncestorElementNode();

            elementNode = node;
            // If there is no nearestAncestor, that's fine! We will be the root.
            nearestAncestor?.ConnectNode(elementNode);
            elementNode.Element.UpdateWidgetNode(this);
        }

        /// <summary>
        /// Connect the given element to this node, adding it to the element tree.
        /// </summary>
        /// <param name="element"></param>
        void ConnectElement(Element element)
        {
            element.UpdateWidgetNode(this);
            var nearestNode = NearestAncestorElementNode();
            elementNode = nearestNode?.AddChild(element) ?? throw new Exception("No ancestor connected to an element tree!");
        }

        /// <summary>
        /// Remove this node and all children.
        /// This will Prune any attached elements as well.
        /// </summary>
        void Prune()
        {
            Emancipate();

            // We must clean up any element nodes attached to this tree.
            // Pruning an elementNode will also clear all children, so we can
            // stop this traversal once we hit such a node.
            if (elementNode != null)
            {
                elementNode.Prune();
            }
            else
            {
                // Pruning removes from this list, so we don't enumerate.
                while (children.Count > 0)
                {
                    children[0].Prune();
                }
            }
        }

        /// <summary>
        /// Remove this node, but leave the children orphans.
        /// This node's elements will be deconstructed, but children will be left alone.
        /// </summary>
        /// <returns></returns>
        Queue<WidgetNode> EmancipateChildrenAndPrune()
        {
            var emancipatedChildren = EmancipateChildren();

            // Then prune.
            Prune();

            return emancipatedChildren;
        }

        Queue<WidgetNode> EmancipateChildren()
        {
            // Take out all the children.
            Queue<WidgetNode> emancipatedChildren = new Queue<WidgetNode>(children);
            foreach (var c in emancipatedChildren)
            {
                c.Emancipate();
            }

            return emancipatedChildren;
        }

        /// <summary>
        /// Get all element nodes that would attach to an ancestor.
        /// </summary>
        /// <returns></returns>
        List<ElementNode> GetElementSurface()
        {
            if (elementNode != null)
                return new List<ElementNode> { elementNode };

            var r = new List<ElementNode>();
            children.ForEach((c) => r.AddRange(c.GetElementSurface()));

            return r;
        }

        void PruneInheritedChildren()
        {
            while ((inheritedChildren?.Count ?? 0) > 0)
                inheritedChildren.Dequeue().Prune();

            inheritedChildren = null;
        }

        /// <summary>
        /// Remove this from its parent.
        /// Also disconnects any attached elements from the element tree.
        /// </summary>
        void Emancipate()
        {
            parent?.children?.Remove(this);
            parent = null;
            EmancipateElementSurface();
        }

        /// <summary>
        /// Find the nearest ancestor with an attached
        /// element node. Returns attached element node if this has one.
        /// </summary>
        /// <returns>The found node if one exists, null otherwise.</returns>
        ElementNode NearestAncestorElementNode()
        {
            if (elementNode != null) return elementNode;
            return parent?.NearestAncestorElementNode();
        }
    }
}

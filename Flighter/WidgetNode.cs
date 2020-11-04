using System;
using System.Collections.Generic;

namespace Flighter
{
    public class WidgetNode
    {
        public readonly Widget Widget;

        /// <summary>
        /// The BuildContext with which this node was built.
        /// </summary>
        public readonly BuildContext BuildContext;

        /// <summary>
        /// The result of building this node.
        /// </summary>
        public readonly BuildResult BuildResult;

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
            this.Widget = widget ?? throw new ArgumentNullException("WidgetNode's widget must not be null.");
            this.BuildContext = buildContext ?? throw new ArgumentNullException("WidgetNode's buildContext must not be null.");
            this.parent = parent;

            this.inheritedChildren = inheritedChildren;

            if (Widget is StatelessWidget slw)
            {
                if (inheritedElementNode != null)
                    throw new Exception("StatelessWidget cannot inherit an element node!");

                var child = slw.Build(BuildContext);
                var childNode = Add(child, BuildContext);

                BuildResult = childNode.BuildResult;
            }
            else if (Widget is StatefullWidget sfw)
            {
                if (inheritedElementNode != null)
                {
                    ConnectInheritedElementNode(inheritedElementNode);
                }
                else
                {
                    var state = sfw.CreateState();
                    // Must connect the element before building the rest of the tree.
                    ConnectElement(new StateElement(state));

                    // Manually do the first build. The rest will be handled with state updates.
                    var child = state.Build(BuildContext);
                    var childNode = Add(child, BuildContext);

                    BuildResult = childNode.BuildResult;
                }

                BuildResult = children[0].BuildResult;
            }
            else if (Widget is LayoutWidget lw)
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

                BuildResult = lw.Layout(buildContext, this);
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

            if (inheritedChildren != null)
            {
                try
                {
                    var c = inheritedChildren.Dequeue();
                    // The context and widget are the same, so no need to add the new widget.
                    if (context == c.BuildContext && widget.IsSame(c.Widget))
                    {
                        children.Add(c);
                        c.parent = this;
                        return c;
                        // TODO: need to connect to element tree?
                    }
                    
                    if (widget.CanReplace(c.Widget))
                    {
                        childElementNode = c.DisconnectElementNode();
                        orphans = c.PruneAndAdopt();
                    }
                    else
                    {
                        c.Prune();
                    }
                }
                catch (InvalidOperationException) { }
            }

            WidgetNode child = new WidgetNode(
                                        widget,
                                        context,
                                        this,
                                        childElementNode,
                                        orphans);

            children.Add(child);
            return child;
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
        /// Disconnect this tree from the element tree.
        /// If this node is itself connected, then that connection is broken.
        /// Otherwise, this is called on it's children.
        /// </summary>
        /// <returns>The disconnected node, if there is one. Null otherwise.</returns>
        ElementNode DisconnectElementNode()
        {
            if (elementNode != null)
            {
                elementNode.Emancipate();
                return  elementNode;
            }
            return null;
        }

        /// <summary>
        /// Connect the given element node to this.
        /// </summary>
        /// <param name="node"></param>
        void ConnectInheritedElementNode(ElementNode node)
        {
            var nearestAncestor = NearestAncestorElementNode();
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
        Queue<WidgetNode> PruneAndAdopt()
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

        void PruneInheritedChildren()
        {
            while (inheritedChildren != null)
            {
                try
                {
                    inheritedChildren.Dequeue().Prune();
                }
                catch (InvalidOperationException)
                {
                    inheritedChildren = null;
                }
            }
        }

        /// <summary>
        /// Remove this from its parent.
        /// Does NOT disconnect an attached element from the element tree.
        /// </summary>
        void Emancipate()
        {
            parent?.children?.Remove(this);
            parent = null;
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

﻿using System;
using System.Collections.Generic;

using Flighter.Input;

namespace Flighter
{
    public delegate bool WidgetNodeCondition(WidgetNode w);

    public struct NodeLayout
    {
        public Size size;
        public Point offset;

        public NodeLayout(Size size, Point offset)
        {
            this.size = size;
            this.offset = offset;
        }

        public NodeLayout(float width, float height, float x = 0, float y = 0)
        {
            size = new Size(width, height);
            offset = new Point(x, y);
        }
    }
    
    public class WidgetNode
    {
        public readonly WidgetForest forest;
        public readonly Widget widget;
        public readonly BuildContext buildContext;

        NodeLayout layout;
        WidgetNode parent;
        readonly List<WidgetNode> children = new List<WidgetNode>();
        ElementNode elementNode;

        public Size Size => layout.size;
        public Point Offset => layout.offset;

        Point? cachedElementOffset;
        Point? cachedAbsoluteOffset;

        /// <summary>
        /// Constructs a widget node.
        /// Adds this as a child of <paramref name="parent"/>.
        /// </summary>
        /// <param name="forest"></param>
        /// <param name="widget"></param>
        /// <param name="buildContext"></param>
        /// <param name="layout"></param>
        /// <param name="parent"></param>
        /// <param name="childrenBuilders"></param>
        /// <param name="elementNode"></param>
        public WidgetNode(
            WidgetForest forest,
            Widget widget,
            BuildContext buildContext,
            NodeLayout layout,
            WidgetNode parent,
            List<WidgetNodeBuilder> childrenBuilders,
            ElementNode elementNode = null)
        {
            this.forest = forest ?? throw new ArgumentNullException("Must belong to a WidgetTree.");
            this.parent = parent;
            parent?.children?.Add(this);

            if (parent != null && parent.forest != forest)
                throw new Exception("Tree must be the same as parent tree.");

            this.widget = widget ?? throw new ArgumentNullException("WidgetNode's widget must not be null.");
            this.buildContext = buildContext;
            this.layout = layout;

            if (elementNode != null)
            {
                var nearestAncestor = GetNearestAncestorElementNode();

                // Connect first so we don't connect to ourself!
                // If there is no ancestor, that's fine! We'll just be a root.
                GetNearestAncestorElementNode()?.ConnectNode(elementNode);
                this.elementNode = elementNode;

                elementNode.Update(this);
            }

            childrenBuilders.ConvertAll((c) => c.Build(this));

            forest.WidgetAdded(widget);
        }

        /// <summary>
        /// Update this nodes connection with respects to <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layout">Optionally provide a new layout value for the widget.</param>
        public void UpdateConnection(WidgetNode parent, NodeLayout? layout = null)
        {
            if (this.parent != null)
                throw new Exception("Node must not have parent to update the connection.");

            ClearCachedOffsets();

            this.parent = parent ?? throw new ArgumentNullException();
            parent.children.Add(this);
            if (parent.forest != forest)
                throw new Exception("Tree must be the same as the parent tree.");
            
            if (layout != null)
                this.layout = layout.Value;

            var elementParent = parent.GetNearestAncestorElementNode();
            GetElementSurface().ForEach((e) => elementParent.ConnectNode(e));
            
            forest.WidgetAdded(widget);
        }

        /// <summary>
        /// Use <paramref name="newKids"/> to replace the current set of children,
        /// if there are any. The same replacement rules will apply as when inheriting
        /// children.
        /// Old children will be disposed if not re-adopted.
        /// </summary>
        /// <param name="newKids"></param>
        public void ReplaceChildren(List<(Widget, BuildContext)> newKids)
        {
            var freeChildren = EmancipateChildren();

            newKids.ForEach(
                (c) =>
                {
                    (var widget, var context) = c;

                    ElementNode nodeToInherit = null;
                    Queue<WidgetNode> childrenToInherit = null;

                    if (freeChildren?.Count > 0)
                    {
                        var toReplace = freeChildren.Dequeue();
                        
                        if (widget.CanReplace(toReplace.widget))
                        {
                            nodeToInherit = toReplace.TakeElementNode();
                            childrenToInherit = toReplace.EmancipateChildren();
                        }

                        toReplace.Dispose();
                    }

                    new WidgetNodeBuilder(
                        forest,
                        widget, 
                        context,
                        nodeToInherit,
                        childrenToInherit
                      ).Build(this);
                });

            // Clear any remaining emancipated children.
            if (freeChildren != null)
            {
                foreach (var c in freeChildren)
                    c.Dispose();
            }
        }

        public void Rebuild()
        {
            ReplaceChildren(children.ConvertAll((c) => (c.widget, c.buildContext)));
        }
        
        /// <summary>
        /// Remove this from its parent.
        /// Also disconnects any attached elements from the element tree.
        /// </summary>
        public void Emancipate()
        {
            parent?.children?.Remove(this);
            parent = null;

            GetElementSurface().ForEach((e) => e.Emancipate());
            ClearCachedOffsets();

            forest.WidgetRemoved(widget);
        }

        public Queue<WidgetNode> EmancipateChildren()
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
        /// Remove this node and all children.
        /// This will dispose any attached elements as well.
        /// </summary>
        public void Dispose()
        {
            Emancipate();

            // With the subtree free we just need to dispose any
            // attached element nodes. Pruning the surface will dispose
            // any descendants as well.
            GetElementSurface().ForEach((e) => e.Dispose());
        }

        /// <summary>
        /// Remove this node, but leave the children orphans.
        /// This node's elements will be deconstructed, but children will be left alone.
        /// </summary>
        /// <returns></returns>
        public Queue<WidgetNode> EmancipateChildrenAndDispose()
        {
            // Emancipate the children...
            var emancipatedChildren = EmancipateChildren();
            // And dispose!
            Dispose();

            return emancipatedChildren;
        }
        
        public ElementNode TakeElementNode()
        {
            var e = elementNode;
            elementNode = null;
            e?.Emancipate();
            // TODO Should we do this? It is technically correct, but it might be confusing to elements
            //      when they briefly don't have a widget node. And these nodes should always end up reconnected within the frame or so...
            e?.element?.UpdateWidgetNode(null);
            return e;
        }
        
        public Point GetElementOffset()
        {
            if (cachedElementOffset != null) return cachedElementOffset.Value;

            if (parent?.elementNode != null)
                cachedElementOffset = Offset;
            else
                cachedElementOffset = layout.offset + parent?.GetElementOffset()
                    ?? Point.Zero;

            return cachedElementOffset.Value;
        }

        public Point GetAbsoluteOffset()
        {
            if (cachedAbsoluteOffset != null) return cachedAbsoluteOffset.Value;

            if (parent == null)
                cachedAbsoluteOffset = Offset;
            else
                cachedAbsoluteOffset = Offset + parent.GetAbsoluteOffset();

            return cachedAbsoluteOffset.Value;
        }

        public bool IsHovering(Point p)
        {
            var absOffset = GetAbsoluteOffset();

            if (p.x < absOffset.x || p.y < absOffset.y)
                return false;
            
            p -= absOffset;
            
            return p.x <= Size.width && p.y <= Size.height;
        }

        /// <summary>
        /// Search through the widget tree, 
        /// </summary>
        /// <param name="condition">Whether this widget should be added.</param>
        /// <param name="continueSearchCondition">If provided, this is called first on
        /// this WidgetNode. If it returns false, the search will stop without adding
        /// this widget, or calling <paramref name="condition"/></param>
        public List<Widget> GetWidgetsWhere(
            WidgetNodeCondition condition, 
            WidgetNodeCondition continueSearchCondition = null,
            List<Widget> baseList = null)
        {
            if (condition == null)
                throw new ArgumentNullException();

            if (baseList == null)
                baseList = new List<Widget>();

            if (!(continueSearchCondition?.Invoke(this) ?? true))
                return baseList;

            if (condition(this))
                baseList.Add(widget);
            
            children.ForEach((c) =>
            {
                c.GetWidgetsWhere(
                    condition,
                    continueSearchCondition,
                    baseList);
            });
            
            return baseList;
        }

        /// <summary>
        /// Get all context dependent input widgets in this tree.
        /// If an InputWidget has <see cref="InputWidget.onlyWhileHovering"/>
        /// set to false, it will be ignored in this search.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<InputWidget> GetContextDependentInputWidgets(InputContext context)
            => GetWidgetsWhere(
                continueSearchCondition: (w) => w.IsHovering(context.mousePosition), 
                condition: (w) => w.widget is InputWidget i
                                && i.onlyWhileHovering)
            .ConvertAll((w) => w as InputWidget);

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
        
        /// <summary>
        /// Find the nearest ancestor with an attached
        /// element node. Returns attached element node if this has one.
        /// </summary>
        /// <returns>The found node if one exists, null otherwise.</returns>
        ElementNode GetNearestAncestorElementNode()
        {
            if (elementNode != null)
                return elementNode;

            return parent?.GetNearestAncestorElementNode();
        }

        void ClearCachedOffsets()
        {
            // If this node has neither offset set, then its children can't have theirs set.
            if (cachedElementOffset == null && cachedAbsoluteOffset == null)
                return;

            cachedElementOffset = cachedAbsoluteOffset = null;

            children.ForEach((c) => c.ClearCachedOffsets());
        }

        public string Print(int indent = 0)
        {
            string r = "";
            for (int i = 0; i < indent; ++i)
                r += "-";

            r += widget.GetType() + "\n";

            foreach (var c in children)
                r += c.Print(indent + 1);

            return r;
        }
    }
}

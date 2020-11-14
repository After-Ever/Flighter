using System;
using System.Collections.Generic;

namespace Flighter
{
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
        public readonly Widget widget;
        public readonly BuildContext buildContext;

        NodeLayout layout;
        WidgetNode parent;
        readonly List<WidgetNode> children;
        public readonly ElementNode elementNode;

        public Size Size => layout.size;
        public Point Offset => layout.offset;

        Point? cachedElementOffset;
        Point? cachedAbsoluteOffset;
        
        public WidgetNode(
            Widget widget,
            BuildContext buildContext,
            NodeLayout layout,
            WidgetNode parent,
            List<WidgetNodeBuilder> childrenBuilders,
            ElementNode elementNode = null)
        {
            this.parent = parent;
            this.widget = widget ?? throw new ArgumentNullException("WidgetNode's widget must not be null.");
            this.buildContext = buildContext;
            this.layout = layout;

            if (elementNode != null)
            {
                elementNode.element.UpdateWidgetNode(this);

                // Connect first so we don't connect to ourself!
                // If there is no ancestor, that's fine! We'll just be a root.
                GetNearestAncestorElementNode()?.ConnectNode(elementNode);
                this.elementNode = elementNode;
            }

            children = childrenBuilders.ConvertAll((c) => c.Build(this));
        }

        public void UpdateConnection(WidgetNode parent, NodeLayout? layout = null)
        {
            if (parent != null)
                throw new Exception("Node must not have parent to udpate the connection.");

            ClearCachedOffsets();

            this.parent = parent;
            if (layout != null)
                this.layout = layout.Value;

            var elementParent = parent.GetNearestAncestorElementNode();
            GetElementSurface().ForEach((e) => elementParent.ConnectNode(e));
        }

        /// <summary>
        /// Use <paramref name="newKids"/> to replace the current set of children,
        /// if there are any. The same replacement rules will apply as when inheriting
        /// children.
        /// Old children will be pruned if not re-adopted.
        /// </summary>
        /// <param name="newKids"></param>
        public void ReplaceChildren(List<(Widget, BuildContext)> newKids)
        {
            var freeChildren = EmancipateChildren();

            var newChildNodes = newKids.ConvertAll(
                (c) =>
                {
                    (var widget, var context) = c;

                    ElementNode nodeToInherit = null;
                    Queue<WidgetNode> childrenToInherit = null;

                    if (freeChildren?.Count > 0)
                    {
                        var toReplace = freeChildren.Dequeue();

                        if (context.Equals(toReplace.buildContext) && widget.IsSame(toReplace.widget))
                        {
                            toReplace.UpdateConnection(this);
                            return toReplace;
                        }

                        if (widget.CanReplace(toReplace.widget))
                        {
                            nodeToInherit = toReplace.elementNode;
                            childrenToInherit = toReplace.EmancipateChildren();
                        }

                        toReplace.Prune();
                    }

                    return new WidgetNodeBuilder(
                        widget, 
                        context,
                        nodeToInherit,
                        childrenToInherit
                      ).Build(this);
                });

            children.AddRange(newChildNodes);

            // Clear any remaining emancipated children.
            if (freeChildren != null)
            {
                foreach (var c in freeChildren)
                    c.Prune();
            }
        }
        
        /// <summary>
        /// Remove this from its parent.
        /// Also disconnects any attached elements from the element tree.
        /// </summary>
        void Emancipate()
        {
            parent?.children?.Remove(this);
            parent = null;
            GetElementSurface().ForEach((e) => e.Emancipate());
            ClearCachedOffsets();
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
        /// This will Prune any attached elements as well.
        /// </summary>
        public void Prune()
        {
            Emancipate();

            // With the subtree free we just need to prune any
            // attached element nodes. Pruning the surface will prune
            // any descendants as well.
            GetElementSurface().ForEach((e) => e.Prune());
        }

        /// <summary>
        /// Remove this node, but leave the children orphans.
        /// This node's elements will be deconstructed, but children will be left alone.
        /// </summary>
        /// <returns></returns>
        public Queue<WidgetNode> EmancipateChildrenAndPrune()
        {
            // Emancipate the children...
            var emancipatedChildren = EmancipateChildren();
            // And prune!
            Prune();

            return emancipatedChildren;
        }
        
        public Point GetElementOffset()
        {
            if (cachedElementOffset != null) return cachedElementOffset.Value;

            if (parent?.elementNode != null)
                cachedElementOffset = Offset;
            else
                cachedElementOffset = layout.offset + parent?.GetElementOffset()
                    ?? throw new Exception("Not rooted in element tree.");

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
    }
}

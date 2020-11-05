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
    
    public class WidgetNode
    {
        public readonly Widget widget;
        public readonly BuildContext buildContext;
        public NodeLayout layout;
        
        WidgetNode parent;
        readonly List<WidgetNode> children;
        public readonly ElementNode elementNode;
        
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
                // Connect first so we don't connect to ourself!
                // If there is no ancestor, that's fine! We'll just be a root.
                GetNearestAncestorElementNode()?.ConnectNode(elementNode);
                this.elementNode = elementNode;
            }

            children = childrenBuilders.ConvertAll((c) => c.Build(this));
        }

        public void UpdateConnection(WidgetNode parent, NodeLayout layout)
        {
            if (parent != null)
                throw new Exception("Node must not have parent to udpate the connection.");

            this.parent = parent;
            this.layout = layout;

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

                    WidgetNode toReplace = null;
                    if (freeChildren?.Count > 0)
                        toReplace = freeChildren.Dequeue();

                    // TODO, before directly passing the replaced element node and children, should check if they are the same, or replacement is allowed.
                    var newBuilder = new WidgetNodeBuilder(
                        null, 
                        widget, 
                        context,
                        toReplace.elementNode,
                        toReplace.EmancipateChildrenAndPrune());
                    
                    return newBuilder.Build(this);
                });

            children.AddRange(newChildNodes);
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
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using Flighter.Input;
using System.Linq;

namespace Flighter
{
    internal delegate bool WidgetNodeCondition(WidgetNode w);

    public struct NodeLayout
    {
        public Size size;
        public Vector2 offset;

        public NodeLayout(Size size, Vector2 offset)
        {
            this.size = size;
            this.offset = offset;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NodeLayout))
            {
                return false;
            }

            var layout = (NodeLayout)obj;
            return EqualityComparer<Size>.Default.Equals(size, layout.size) &&
                   EqualityComparer<Vector2>.Default.Equals(offset, layout.offset);
        }

        public override int GetHashCode()
        {
            var hashCode = -1455214714;
            hashCode = hashCode * -1521134295 + EqualityComparer<Size>.Default.GetHashCode(size);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(offset);
            return hashCode;
        }

        public override string ToString()
            => "Size: " + size + ", Offset:" + offset;
    }
    
    public class WidgetNode : TreeNode
    {
        public readonly WidgetForest forest;
        public readonly Widget widget;
        public readonly BuildContext buildContext;

        NodeLayout layout;
        ElementNode elementNode;
        InputNode inputNode;

        public Size Size => layout.size;
        public Vector2 Offset => layout.offset;

        Vector2? cachedElementOffset;
        Vector2? cachedAbsoluteOffset;

        /// <summary>
        /// When not null, the specified node is currently rebuilding.
        /// Some operations will fail when a child is rebuilding.
        /// </summary>
        WidgetNode rebuildingChild;

        protected WidgetNode parent => Parent as WidgetNode;

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
        internal WidgetNode(
            WidgetForest forest,
            Widget widget,
            BuildContext buildContext,
            NodeLayout layout,
            WidgetNode parent,
            List<WidgetNodeBuilder> childrenBuilders,
            ElementNode elementNode = null)
        {
            this.forest = forest ?? throw new ArgumentNullException("Must belong to a WidgetForest.");

            parent?.AddChild(this);

            if (parent != null && parent.forest != forest)
                throw new Exception("Forest must be the same as parent tree.");

            this.widget = widget ?? throw new ArgumentNullException("WidgetNode's widget must not be null.");
            this.buildContext = buildContext;
            this.layout = layout;

            if (elementNode != null)
            {
                // Connect first so we don't connect to ourself!
                // If there is no ancestor, that's fine! We'll just be a root.
                GetNearestAncestorElementNode()?.AddChild(elementNode);
                this.elementNode = elementNode;
                
                elementNode.Inflate(this);
            }

            if (widget is InputWidget)
            {
                var inputParent = (FirstAncestorWhere(
                    includeThis: false,
                    predicate: node =>
                        (node as WidgetNode).inputNode != null)
                    as WidgetNode)?.inputNode;

                inputNode = new InputNode(this, inputParent);
            }

            childrenBuilders.ConvertAll((c) => c.Build(this));

            forest.WidgetAdded(widget);
        }

        /// <summary>
        /// Rebuild this widget, creating a new WidgetNode.
        /// The new node will replace this one in the tree.
        /// </summary>
        internal void Rebuild()
        {
            // The root widget may have a rebuild triggered if a child changed size,
            // but the root should never be rebuilt.
            if (widget is RootWidget)
                return;

            if (parent == null)
                throw new Exception("Cannot rebuild root node!");

            // Let our parent know we are rebuilding.
            parent.ChildRebuilding(this);
            var b = new WidgetNodeBuilder(
                forest,
                widget,
                buildContext,
                TakeElementNode(),
                new Queue<WidgetNode>(EmancipateChildren().ConvertAll(treeNode => treeNode as WidgetNode)));
            b.Offset = Offset;
            var node = b.Build(parent);

            // If the size is different, need to notify our parent incase they need to rebuild.
            if (!Size.Equals(node.Size))
                (node.parent)?.ChildResized(node);
        }
        
        /// <summary>
        /// Disconnects any attached elements from the element tree.
        /// </summary>
        protected override void WasEmancipated()
        {
            // TODO Could come up with "node group" idea to link the connecting and removing of composite nodes.
            GetElementSurface().ForEach((e) => e.Emancipate());
            ClearCachedOffsets();
            GetInputSurface().ForEach((i) => i.Emancipate());

            forest.WidgetRemoved(widget);
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
            GetInputSurface().ForEach((i) => i.Emancipate());
        }
        
        internal ElementNode TakeElementNode()
        {
            var e = elementNode;
            elementNode = null;
            e?.Emancipate();
            return e;
        }

        internal Vector2 GetElementOffset()
        {
            if (cachedElementOffset != null) return cachedElementOffset.Value;

            if (parent?.elementNode != null)
                cachedElementOffset = Offset;
            else
                cachedElementOffset = layout.offset + parent?.GetElementOffset()
                    ?? Vector2.Zero;

            return cachedElementOffset.Value;
        }

        internal Vector2 GetAbsoluteOffset()
        {
            if (cachedAbsoluteOffset != null) return cachedAbsoluteOffset.Value;

            if (parent == null)
                cachedAbsoluteOffset = Offset;
            else
                cachedAbsoluteOffset = Offset + parent.GetAbsoluteOffset();

            return cachedAbsoluteOffset.Value;
        }

        public void DistributeInputEvent(InputEvent e)
        {
            var inputSurface = GetInputSurface();
            for (int i = inputSurface.Count - 1; i >= 0; --i)
            {
                if (e.FullyAbsorbed)
                    return;

                inputSurface[i].DistributeInputEvent(e);
            }
        }

        public bool IsHovering(Vector2 p)
        {
            var absOffset = GetAbsoluteOffset();

            if (p.X < absOffset.X || p.Y < absOffset.Y)
                return false;
            
            p -= absOffset;
            
            return p.X <= Size.width && p.Y <= Size.height;
        }

        protected override int NewChildIndex(TreeNode childToBeAdded)
        {
            if (rebuildingChild != null)
            {
                var replaceIndex = IndexOf(Children, rebuildingChild);
                if (replaceIndex == -1)
                    throw new Exception("rebuildingChild not found in children.");

                rebuildingChild.Emancipate();
                rebuildingChild = null;

                return replaceIndex;
            }

            return -1;
        }

        // TODO Make public util.
        int IndexOf<T>(IEnumerable<T> enumerable, T obj)
        {
            var i = 0;
            foreach (var t in enumerable)
            {
                if (t.Equals(obj))
                    return i;
                ++i;
            }

            return -1;
        }

        void ChildResized(WidgetNode changedNode)
        {
            if (widget is StatefulWidget || widget is StatelessWidget)
            {
                if (changedNode != Children[0])
                    throw new Exception("Changed node is not the child node!");

                layout.size = changedNode.Size;
                parent?.ChildResized(this);
            }
            else
                Rebuild();
        }

        void ChildRebuilding(WidgetNode child)
        {
            if (rebuildingChild != null)
                throw new Exception("Already rebuilding a child.");

            rebuildingChild = child;
        }

        /// <summary>
        /// Get all element nodes that would attach to an ancestor.
        /// </summary>
        /// <returns></returns>
        List<ElementNode> GetElementSurface()
        {
            List<TreeNode> baseList = null;
            SurfaceWhere(ref baseList, node => (node as WidgetNode).elementNode != null);

            return baseList
                .Select(node => (node as WidgetNode).elementNode)
                .ToList();
        }

        List<InputNode> GetInputSurface()
        {
            List<TreeNode> baseList = null;
            SurfaceWhere(ref baseList, node => (node as WidgetNode).inputNode != null);

            return baseList
                .Select(node => (node as WidgetNode).inputNode)
                .ToList();
        }

        /// <summary>
        /// Find the nearest ancestor with an attached
        /// element node. Returns attached element node if this has one.
        /// </summary>
        /// <returns>The found node if one exists, null otherwise.</returns>
        ElementNode GetNearestAncestorElementNode()
            => GetFirstAncestorWhere(
                includeSelf: true,
                condition: (w) => w.elementNode != null)?.elementNode;

        WidgetNode GetFirstAncestorWhere(WidgetNodeCondition condition, bool includeSelf = false)
        {
            if (includeSelf && condition(this))
                return this;

            return parent?.GetFirstAncestorWhere(condition, true);
        }

        void ClearCachedOffsets()
        {
            // If this node has neither offset set, then its children can't have theirs set.
            if (cachedElementOffset == null && cachedAbsoluteOffset == null)
                return;

            cachedElementOffset = cachedAbsoluteOffset = null;

            // TODO Could use "Search" for these and such operations.
            foreach (var c in Children)
                (c as WidgetNode).ClearCachedOffsets();
        }

        public string Print(int indent = 0)
        {
            string r = "";
            for (int i = 0; i < indent; ++i)
                r += "-";

            r += widget.GetType() + "\n";

            foreach (var c in Children)
                r += (c as WidgetNode).Print(indent + 1);

            return r;
        }
    }
}

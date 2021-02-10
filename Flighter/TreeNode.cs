using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class TreeNode
    {
        public TreeNode Parent { get; private set; } = null;
        public IReadOnlyList<TreeNode> Children => children;
        List<TreeNode> children = new List<TreeNode>();

        public void AddChild(TreeNode child)
        {
            if (child.Parent != null)
                throw new Exception("Cannot add child with a parent.");

            var index = NewChildIndex(child);
            if (index == -1)
                children.Add(child);
            else
                children.Insert(index, child);

            child.Parent = this;
        }

        public void Emancipate()
        {
            if (Parent == null)
                return;

            if (!(Parent?.children?.Remove(this) ?? false))
                throw new Exception("Node not in parents chilren list.");

            Parent.ChildWasEmancipated(this);
            Parent = null;

            WasEmancipated();
        }

        public List<TreeNode> EmancipateChildren()
        {
            var l = new List<TreeNode>(children);
            foreach (var child in l)
                child.Emancipate();

            return l;
        }

        /// <summary>
        /// Find the first ancestor node to satisfy <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includeThis">Whether to include this in the search.</param>
        public TreeNode FirstAncestorWhere(Predicate<TreeNode> predicate, bool includeThis = true)
        {
            if (includeThis && predicate(this))
                return this;
            return Parent?.FirstAncestorWhere(predicate);
        }

        /// <summary>
        /// Perform a depth first search on the tree, starting from this node.
        /// </summary>
        /// <param name="onNode">Logic to run when arriving at each node.</param>
        /// <param name="takeNode">Whether the given node should be considered in the search.</param>
        /// <param name="stopSearch">Whether the search should stop entirely at this node.</param>
        /// <param name="includeThis">Whether the calling node should have <paramref name="onNode"/>
        /// called on it.</param>
        /// <returns>false if the search was stopped part way.</returns>
        public bool DFSearch(
            Action<TreeNode> onNode, 
            Predicate<TreeNode> takeNode = null,
            Predicate<TreeNode> stopSearch = null,
            bool includeThis = true)
        {
            if (stopSearch?.Invoke(this) ?? false)
                return false;

            if (takeNode?.Invoke(this) ?? true)
            {
                if (includeThis)
                    onNode(this);

                foreach (var child in Children)
                    if (!child.DFSearch(onNode, takeNode, stopSearch))
                        return false;
            }

            return true;
        }

        /// <summary>
        /// Right to left, depth first search.
        /// </summary>
        /// <param name="onNode"></param>
        /// <param name="takeNode"></param>
        /// <param name="stopSearch"></param>
        /// <param name="includeThis"></param>
        /// <returns></returns>
        public bool DFR2LSearch(
            Action<TreeNode> onNode,
            Predicate<TreeNode> takeNode = null,
            Predicate<TreeNode> stopSearch = null,
            bool includeThis = true)
        {
            if (stopSearch?.Invoke(this) ?? false)
                return false;

            if (takeNode?.Invoke(this) ?? true)
            {
                if (includeThis)
                    onNode(this);

                for (int i = Children.Count - 1; i >= 0; --i)
                    if (!Children[i].DFR2LSearch(onNode, takeNode, stopSearch))
                        return false;
            }

            return true;
        }

        public void SurfaceWhere(
            ref List<TreeNode> baseList,
            Predicate<TreeNode> predicate,
            bool includeThis = true)
        {
            if (baseList == null)
                baseList = new List<TreeNode>();

            if (includeThis && predicate(this))
            {
                baseList.Add(this);
                return;
            }

            foreach (var c in Children)
                c.SurfaceWhere(ref baseList, predicate);
        }

        /// <summary>
        /// Returns the index where the new child should be inserted,
        /// or -1 to insert at the end.
        /// </summary>
        /// <param name="childToBeAdded"></param>
        /// <returns></returns>
        protected virtual int NewChildIndex(TreeNode childToBeAdded) => -1;
        protected virtual void ChildWasAdded(TreeNode newChild) { }
        protected virtual void ChildWasEmancipated(TreeNode emancipatedChild) { }
        protected virtual void WasEmancipated() { }

        // - Search up
        // - Search down
        // - Emancipate
        // - EmancipateChildren
        // - AddChild

        // - Virtual event methods for all the things.
    }
}

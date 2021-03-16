using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; private set; } = null;
        public IReadOnlyList<TreeNode<T>> Children => children;
        List<TreeNode<T>> children = new List<TreeNode<T>>();

        public T data;

        public TreeNode(T data = default)
        {
            this.data = data;
        }

        public void AddChild(TreeNode<T> child)
        {
            if (child.Parent != null)
                throw new Exception("Cannot add child with a parent.");

            children.Add(child);
            child.Parent = this;
        }

        public void Emancipate()
        {
            if (Parent == null)
                return;

            if (!(Parent?.children?.Remove(this) ?? false))
                throw new Exception("Node not in parents children list.");

            Parent = null;
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
            Action<TreeNode<T>> onNode, 
            Predicate<TreeNode<T>> takeNode = null,
            Predicate<TreeNode<T>> stopSearch = null,
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
            Action<TreeNode<T>> onNode,
            Predicate<TreeNode<T>> takeNode = null,
            Predicate<TreeNode<T>> stopSearch = null,
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
    }
}

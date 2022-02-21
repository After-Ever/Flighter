using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    /// <summary>
    /// Data of a <see cref="TreeNode{T}"/> can subclass this
    /// to automatically receive access to the node it becomes 
    /// apart of.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeNodeData<T>
    {
        // TODO probably add some events?
        protected TreeNode<T> node { get; private set; }
        protected bool inTree { get; private set; } = false;

        internal void SetNode(TreeNode<T> node) 
        { 
            this.node = node;
            inTree = node != null;
        }
    }

    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; private set; } = null;
        public IReadOnlyList<TreeNode<T>> Children => children;
        List<TreeNode<T>> children = new List<TreeNode<T>>();

        public T data;

        public TreeNode(T data = default)
        {
            this.data = data;

            if (data != null && data is TreeNodeData<T> tnd)
                tnd.SetNode(this);
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
            if (includeThis && (stopSearch?.Invoke(this) ?? false))
                return false;

            if (!includeThis || (takeNode?.Invoke(this) ?? true))
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
            if (includeThis && (stopSearch?.Invoke(this) ?? false))
                return false;

            if (!includeThis || (takeNode?.Invoke(this) ?? true))
            {
                if (includeThis)
                    onNode(this);

                for (int i = Children.Count - 1; i >= 0; --i)
                    if (!Children[i].DFR2LSearch(onNode, takeNode, stopSearch))
                        return false;
            }

            return true;
        }

        /// <summary>
        /// Search the tree using a breadth first search.
        /// </summary>
        /// <param name="onNode">Run on each searched node.</param>
        /// <param name="takeNode">Should this node be considered at all?</param>
        /// <param name="stopSearch">If this returns true, the search will be stopped.</param>
        /// <param name="includeThis">Whether to ignore this node. (Used for starting search).</param>
        /// <returns></returns>
        public TreeNode<T> BFSearch(
            Action<TreeNode<T>> onNode,
            Predicate<TreeNode<T>> takeNode = null,
            Predicate<TreeNode<T>> stopSearch = null,
            bool includeThis = true)
        {
            Queue<TreeNode<T>> toSearch = new Queue<TreeNode<T>>();

            if (includeThis)
                toSearch.Enqueue(this);
            else
                children.ForEach(c => toSearch.Enqueue(c));

            while (toSearch.Count > 0)
            {
                var n = toSearch.Dequeue();
                if (!(takeNode?.Invoke(this) ?? true))
                    continue;
                if (stopSearch?.Invoke(this) ?? false)
                    break;

                onNode(n);
                n.children.ForEach(c => toSearch.Enqueue(c));
            }

            return null;
        }
    }
}

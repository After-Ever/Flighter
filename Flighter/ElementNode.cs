using System;
using System.Collections.Generic;

using UnityEngine;

namespace Flighter
{
    public class ElementNode
    {
        public Element Element { get; private set; }

        ElementNode parent;
        List<ElementNode> children = new List<ElementNode>();

        bool isDirty = true;
        public bool IsDirty
        {
            get => isDirty || HasDirtyChild;
        }
        public bool HasDirtyChild { get; private set; } = false;

        public ElementNode(Element element, ElementNode parent)
        {
            this.Element = element;
            this.parent = parent;
        }

        public void Update()
        {
            if (!IsDirty) return;

            if (isDirty)
            {
                if (Element.IsInitialized)
                {
                    Element.Update();
                }
                else
                {
                    InitElement();
                }
            }
            if (HasDirtyChild)
            {
                var dirtyChildren = GetDirtyChildren();

                foreach (var c in dirtyChildren)
                {
                    c.Update();
                }
            }

            SetClean();
        }

        public ElementNode AddChild(Element element)
        {
            var node = new ElementNode(element, this);
            children.Add(node);

            SetChildDirty();

            return node;
        }

        public void ConnectNode(ElementNode node)
        {
            if (node.parent != null)
                throw new Exception("Cannot add a node with a parent.");

            children.Add(node);
            node.parent = this;
            SetChildDirty();
        }

        public void SetDirty()
        {
            if (isDirty) return;

            isDirty = true;
            parent?.SetChildDirty();
        }

        /// <summary>
        /// Remove this node, leaving it's children orphans.
        /// </summary>
        public void Prune()
        {
            foreach(var c in children)
            {
                c.parent = null;
                c.Element.RectTransform.SetParent(null, false);
                c.SetDirty();
                // At this point, the removed children should be garbage
                // collected, unless they are referenced elsewhere.
            }

            parent?.RemoveChild(this);

            Element.Clear();
        }

        void SetClean()
        {
            isDirty = false;
            HasDirtyChild = false;
        }

        void SetChildDirty()
        {
            if (HasDirtyChild) return;
            HasDirtyChild = true;
            if (isDirty) return;

            parent?.SetChildDirty();
        }

        List<ElementNode> GetDirtyChildren()
        {
            return children.FindAll((n) => n.IsDirty);
        }

        /// <summary>
        /// Remove a node from this node's children.
        /// The removed node is left as is; not pruned.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        void RemoveChild(ElementNode node)
        {
            if (!children.Remove(node))
                throw new Exception("Can't remove none child node");
            
            if (HasDirtyChild 
                && children.Find((n) => n.IsDirty) == null)
                HasDirtyChild = false;
        }

        protected virtual void InitElement()
        {
            if (Element.IsInitialized) return;
            if (parent == null)
                throw new Exception("Cannot initialize an element with no parent.");

            var newObj = new GameObject(Element.Name);
            var rectTransform = newObj.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.Element.RectTransform, false);

            Element.Init(rectTransform);
        }
    }
}

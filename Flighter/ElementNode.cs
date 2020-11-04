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

        // TODO: Describe how these dirty variables are used.
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

            Element.SetDirtyCallback(() => SetDirty());
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
            if (element.IsInitialized)
                throw new Exception("Cannot add an initialized element!");

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
            
            if (node.Element.IsInitialized)
            {
                var rect = node.Element.RectTransform;
#if !TEST
                rect.SetParent(Element.RectTransform, false);
#endif
            }
            
            SetChildDirty();
        }

        public void SetDirty()
        {
            if (isDirty) return;

            isDirty = true;
            parent?.SetChildDirty();
        }

        /// <summary>
        /// Remove this from its parent.
        /// </summary>
        public void Emancipate()
        {
            parent?.RemoveChild(this);
        }

        /// <summary>
        /// Remove this node, and any children.
        /// </summary>
        public void Prune()
        {
            // TODO: What about children?
            Emancipate();

            Element.TearDown();
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
        /// </summary>
        /// <param name="node">The node to remove.</param>
        void RemoveChild(ElementNode node)
        {
            if (!children.Remove(node))
                throw new Exception("Can't remove none child node");
#if !TEST
            node.Element.RectTransform?.SetParent(null);
#endif
            node.parent = null;
            node.SetDirty();

            if (HasDirtyChild 
                && children.Find((n) => n.IsDirty) == null)
                HasDirtyChild = false;
        }

        protected virtual void InitElement()
        {
            if (Element.IsInitialized) return;
            if (parent == null || !parent.Element.IsInitialized)
                throw new Exception("Cannot initialize an element without an initialized parent.");
#if !TEST
            var newObj = new GameObject(Element.Name);
            var rectTransform = newObj.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.Element.RectTransform, false);
#else
            RectTransform rectTransform = null;
#endif
            Element.Init(rectTransform);
        }

        public string Print(int indent = 0)
        {
            string r = "";
            for (int i = 0; i < indent; ++i)
                r += "-";

            r += Element.Name + "\n";

            foreach (var c in children)
                r += c.Print(indent + 1);

            return r;
        }
    }
}

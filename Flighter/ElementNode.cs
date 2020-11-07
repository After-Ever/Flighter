using System;
using System.Collections.Generic;

namespace Flighter
{
    public class ElementNode
    {
        public readonly Element element;

        ElementNode parent;
        List<ElementNode> children = new List<ElementNode>();

        ComponentProvider componentProvider;
        ComponentProvider ComponentProvider
        {
            get
            {
                if (componentProvider != null)
                    return componentProvider;

                return componentProvider = 
                    parent?.ComponentProvider ?? throw new Exception("Could not find component provider.");
            }
        }

        /// <summary>
        /// This node needs an update.
        /// </summary>
        public bool IsDirty { get; private set; } = true;
        /// <summary>
        /// This node has descendants which need updates.
        /// </summary>
        public bool HasDirtyChild { get; private set; } = false;

        public ElementNode(Element element, ElementNode parent, ComponentProvider componentProvider = null)
        {
            this.element = element ?? throw new ArgumentNullException();
            this.parent = parent;
            this.componentProvider = componentProvider;

            this.element.SetDirtyCallback(() => SetDirty());
        }

        public void Update()
        {
            if (!IsDirty && !HasDirtyChild) return;

            if (IsDirty)
            {
                if (element.IsInitialized)
                {
                    element.Update();
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

            // Just pass the componentProvider field (instead of the property)
            // because we don't care if it's null here; it can always search later.
            var node = new ElementNode(element, this, componentProvider);
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
            
            if (node.element.IsInitialized)
            {
                var rect = node.element.DisplayRect;
                rect.SetParent(element.DisplayRect);
            }
            
            SetChildDirty();
        }

        public void SetDirty()
        {
            if (IsDirty) return;

            IsDirty = true;
            parent?.SetChildDirty();
        }

        /// <summary>
        /// Remove this from its parent.
        /// Sets this as dirty.
        /// </summary>
        public void Emancipate()
        {
            if (parent == null)
                return;

            if (!parent.children.Remove(this))
                throw new Exception("Node not in parent's child list.");
            
            parent = null;
            SetDirty();
        }

        /// <summary>
        /// Remove this node, and any children.
        /// </summary>
        public void Prune()
        {
            // TODO: What about children?
            Emancipate();

            element.TearDown();
        }

        void SetClean()
        {
            IsDirty = false;
            HasDirtyChild = false;
        }

        void SetChildDirty()
        {
            if (HasDirtyChild) return;
            HasDirtyChild = true;
            if (IsDirty) return;

            parent?.SetChildDirty();
        }

        List<ElementNode> GetDirtyChildren()
        {
            return children.FindAll((n) => n.IsDirty || n.HasDirtyChild);
        }

        protected virtual void InitElement()
        {
            if (element.IsInitialized) return;
            if (parent == null || !parent.element.IsInitialized)
                throw new Exception("Cannot initialize an element without an initialized parent.");

            var rect = parent.element.DisplayRect.CreateChild();
            rect.Name = element.Name;

            // Use the componentProvider property here to make sure we find it.
            element.Init(rect, ComponentProvider);
            element.Update();
        }

        public string Print(int indent = 0)
        {
            string r = "";
            for (int i = 0; i < indent; ++i)
                r += "-";

            r += element.Name + "\n";

            foreach (var c in children)
                r += c.Print(indent + 1);

            return r;
        }
    }
}

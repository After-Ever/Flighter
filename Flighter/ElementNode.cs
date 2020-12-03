using System;
using System.Collections.Generic;

namespace Flighter
{
    public class ElementNode
    {
        public readonly Element element;

        ElementNode parent;
        List<ElementNode> children = new List<ElementNode>();

        ComponentProvider _componentProvider;
        ComponentProvider ComponentProvider
        {
            get
            {
                if (_componentProvider != null)
                    return _componentProvider;

                return _componentProvider = 
                    parent?.ComponentProvider ?? throw new Exception("Could not find component provider.");
            }
        }

        /// <summary>
        /// This node needs an update.
        /// </summary>
        public bool IsDirty { get; private set; } = false;
        /// <summary>
        /// This node has descendants which need updates.
        /// </summary>
        public bool HasDirtyChild { get; private set; } = false;

        public ElementNode(Element element, ElementNode parent, ComponentProvider componentProvider = null)
        {
            this.element = element ?? throw new ArgumentNullException();
            this.parent = parent;
            this._componentProvider = componentProvider;

            this.element.SetDirtyCallback(SetDirty);
        }

        public void Inflate(WidgetNode widgetNode)
        {
            element.UpdateWidgetNode(widgetNode);
            InitOrConnectElement();
            
            // Set clean first incase the updates set it dirty again.
            SetClean();
            element.Update();
        }

        public void Update(int d = 0)
        {
            if (!IsDirty && !HasDirtyChild) return;

            if (IsDirty)
            {
                element.widgetNode.Rebuild();
            }
            else
            {
                // Make a copy, as updates may change family hierarchy.
                var toUpdate = new List<ElementNode>(children);
                toUpdate.ForEach((c) => c.Update(d + 1));
            }
        }

        public void ConnectNode(ElementNode node)
        {
            if (node.parent != null)
                throw new Exception("Cannot add a node with a parent.");

            children.Add(node);
            node.parent = this;

            if (node.IsDirty || node.HasDirtyChild)
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
        /// Does not change dirty status.
        /// </summary>
        public void Emancipate()
        {
            if (parent == null)
                return;

            if (!parent.children.Remove(this))
                throw new Exception("Node not in parent's child list.");

            if (IsDirty || HasDirtyChild)
                parent.UpdateChildDirtyStatus();
            
            parent = null;

            element.Disconnect();
        }
        
        /// <summary>
        /// Remove and destroy this node, and any children.
        /// </summary>
        public void Dispose(bool emancipate = true)
        {
            if (emancipate)
                Emancipate();
            
            children.ForEach((c) => c.Dispose(false));

            element.TearDown();
        }

        void SetClean()
        {
            bool hasChanged = IsDirty || HasDirtyChild;

            IsDirty = false;
            HasDirtyChild = false;

            if (hasChanged)
                parent?.UpdateChildDirtyStatus();
        }

        void SetChildDirty()
        {
            if (HasDirtyChild) return;
            HasDirtyChild = true;
            if (IsDirty) return;

            parent?.SetChildDirty();
        }
        
        /// <summary>
        /// Checks the status of <see cref="HasDirtyChild"/>.
        /// </summary>
        /// TODO: This one is a little wormy... Make sure everything is correct, and tested.
        void UpdateChildDirtyStatus()
        {
            var newStatus = children.Exists((n) => n.IsDirty || n.HasDirtyChild);

            if (newStatus == HasDirtyChild)
                return;

            HasDirtyChild = newStatus;

            // If this is dirty, no need to update parents, as they still have a dirty child.
            if (IsDirty)
                return;

            if (HasDirtyChild)
                parent?.SetChildDirty();
            else
                parent?.UpdateChildDirtyStatus();
        }

        protected virtual void InitOrConnectElement()
        {
            if (element.IsInitialized && element.IsConnected) return;
            if (parent == null || !parent.element.IsInitialized)
                throw new Exception("Cannot initialize an element without an initialized parent.");

            var parentRect = parent.element.DisplayRect;

            if (!element.IsInitialized)
            {
                // Use the ComponentProvider property here to make sure we find it.
                element.Init(parentRect.CreateChild(), ComponentProvider);
            }
            else
            {
                element.Reconnect(parentRect);
            }
        }

        public string Print(int indent = 0)
        {
            string r = "";
            for (int i = 0; i < indent; ++i)
                r += "-";

            r += element.Name + (IsDirty? "*\n" : "\n");

            foreach (var c in children)
                r += c.Print(indent + 1);

            return r;
        }
    }
}

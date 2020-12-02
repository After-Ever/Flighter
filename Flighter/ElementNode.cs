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

        public ElementNode(Element element, ElementNode parent, ComponentProvider componentProvider = null)
        {
            this.element = element ?? throw new ArgumentNullException();
            this.parent = parent;
            this._componentProvider = componentProvider;
        }

        /// <summary>
        /// Update this node, but not children.
        /// Called exclusively when a new WidgetNode is connected to this element.
        /// </summary>
        /// <param name="widgetNode">The WidgetNode this element is attached to.</param>
        public void Update(WidgetNode widgetNode)
        {
            if (!element.IsConnected)
                InitOrConnectElement();

            element.UpdateWidgetNode(widgetNode);
            element.Update();
        }

        public ElementNode AddChild(Element element)
        {
            if (element.IsInitialized)
                throw new Exception("Cannot add an initialized element!");

            // Just pass the componentProvider field (instead of the property)
            // because we don't care if it's null here; it can always search later.
            var node = new ElementNode(element, this, _componentProvider);
            children.Add(node);

            return node;
        }

        public void ConnectNode(ElementNode node)
        {
            if (node.parent != null)
                throw new Exception("Cannot add a node with a parent.");

            children.Add(node);
            node.parent = this;
        }

        /// <summary>
        /// Remove this from its parent.
        /// </summary>
        public void Emancipate()
        {
            if (parent == null)
                return;

            if (!parent.children.Remove(this))
                throw new Exception("Node not in parent's child list.");
            
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

        // "internal" to allow RootElementNode to override.
        internal virtual void InitOrConnectElement()
        {
            if (element.IsInitialized) return;
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

            r += element.Name + "\n";

            foreach (var c in children)
                r += c.Print(indent + 1);

            return r;
        }
    }
}

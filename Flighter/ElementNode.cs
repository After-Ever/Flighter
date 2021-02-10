using System;
using System.Collections.Generic;
using System.Linq;

namespace Flighter
{
    public class ElementNode : TreeNode
    {
        public readonly Element element;

        ElementNode parent => Parent as ElementNode;

        WidgetNode currentWidgetNode;
        bool changedWidgetNode = false;

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

        bool needsRebuild = false;

        public ElementNode(Element element, ElementNode parent, ComponentProvider componentProvider = null)
        {
            this.element = element ?? throw new ArgumentNullException();
            parent?.AddChild(this);
            this._componentProvider = componentProvider;

            this.element.SetElementNode(this);
        }

        public void DoRebuilds()
        {
            if (needsRebuild)
            {
                // Rebuilding should call Infalte, setting this as no longer needing a rebuild,
                // and updating the parent.
                currentWidgetNode.Rebuild();

                if (needsRebuild)
                    throw new Exception("Rebuild did not inflate rebuilt element.");
            }
            else
            {
                // Make a copy, as they will be removed and readded durring rebuilds.
                var childrenToRebuild = Children.Select(node => node as ElementNode).ToList();
                foreach (var child in childrenToRebuild)
                    child.DoRebuilds();
            }
        }

        public void Update()
        {
            // TODO: Currently updating every node in the tree, but that not nearly always required!
            //       Need to check if the node needs an update...

            if (changedWidgetNode)
            {
                changedWidgetNode = false;
                element.UpdateWidgetNode(currentWidgetNode);
            }

            EnsureElementConnected();

            element.Update();

            foreach (var e in Children)
                (e as ElementNode).Update();
        }

        /// <summary>
        /// Called when the connected WidgetNode changes.
        /// This method does not trigger any updates on the underlying Element.
        /// <see cref="Update()"/> Triggers the Element updates.
        /// </summary>
        /// <param name="widgetNode"></param>
        internal void Inflate(WidgetNode widgetNode)
        {
            changedWidgetNode = changedWidgetNode || widgetNode != currentWidgetNode;
            currentWidgetNode = widgetNode;

            needsRebuild = false;
        }

        internal void RequestRebuild()
        {
            needsRebuild = true;
        }

        protected override void WasEmancipated()
        {
            element.Disconnect();
        }

        /// <summary>
        /// Remove and destroy this node, and any children.
        /// </summary>
        public void Dispose(bool emancipate = true)
        {
            if (emancipate)
                Emancipate();

            foreach (var c in Children)
                (c as ElementNode).Dispose(false);

            element.TearDown();
        }

        internal virtual void EnsureElementConnected()
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

            r += element.Name + (needsRebuild? "*" : "") + "\n";

            foreach (var c in Children)
                r += (c as ElementNode).Print(indent + 1);

            return r;
        }
    }
}

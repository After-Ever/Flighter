using System;
using System.Collections.Generic;

namespace Flighter
{
    public class ElementNode
    {
        public readonly Element element;

        ElementNode parent;
        List<ElementNode> children = new List<ElementNode>();

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
        int childrenNeedingRebuild = 0;

        bool TreeNeedsRebuild => needsRebuild || childrenNeedingRebuild > 0;

        public ElementNode(Element element, ElementNode parent, ComponentProvider componentProvider = null)
        {
            this.element = element ?? throw new ArgumentNullException();
            this.parent = parent;
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
            }

            while (childrenNeedingRebuild > 0)
            {
                children.Find((e) => e.TreeNeedsRebuild).DoRebuilds();
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

            InitOrConnectElement();

            element.Update();
            children.ForEach((e) => e.Update());
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

            var treeNeededRebuild = TreeNeedsRebuild;
            needsRebuild = false;

            if (TreeNeedsRebuild != treeNeededRebuild)
                parent?.ChildRebuilt();
        }

        internal void ConnectNode(ElementNode node)
        {
            if (node.parent != null)
                throw new Exception("Cannot add a node with a parent.");

            children.Add(node);
            node.parent = this;

            if (node.TreeNeedsRebuild)
                ChildRequestedRebuild();
        }

        internal void RequestRebuild()
        {
            if (needsRebuild) return;

            var treeNeededRebuild = TreeNeedsRebuild;
            needsRebuild = true;

            if (!treeNeededRebuild)
                parent?.ChildRequestedRebuild();
        }

        /// <summary>
        /// Remove this from its parent.
        /// </summary>
        internal void Emancipate()
        {
            if (parent == null)
                return;

            if (!parent.children.Remove(this))
                throw new Exception("Node not in parent's child list.");

            if (TreeNeedsRebuild)
                parent.ChildRebuilt();
            
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

        void ChildRequestedRebuild()
        {
            bool treeNeededRebuild = TreeNeedsRebuild;
            childrenNeedingRebuild++;

            if (!treeNeededRebuild)
                parent?.ChildRequestedRebuild();
        }

        void ChildRebuilt()
        {

            bool treeNeededRebuild = TreeNeedsRebuild;
            childrenNeedingRebuild--;

            if (TreeNeedsRebuild != treeNeededRebuild)
                parent?.ChildRebuilt();
        }

        // TODO: Come up with a better name...
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

            r += element.Name + (needsRebuild? "*" : "") + "\n";

            foreach (var c in children)
                r += c.Print(indent + 1);

            return r;
        }
    }
}

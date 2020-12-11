using System;

namespace Flighter
{
    /// <summary>
    /// Used to impact the display tree.
    /// </summary>
    public abstract class Element
    {
        public WidgetNode widgetNode { get; private set; }
        public W GetWidget<W>() where W : Widget
        {
            return widgetNode?.widget as W;
        }

        /// <summary>
        /// Whether the Element has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;
        /// <summary>
        /// Whether the Element is connected to the element tree.
        /// </summary>
        public bool IsConnected { get; private set; } = false;
        
        /// <summary>
        /// The DisplayRect this element will instrument.
        /// It should not be modified outside the element, save for changing
        /// family hierarchy.
        /// 
        /// This will be null before <see cref="Init(IDisplayRect)"/> is called,
        /// and after <see cref="TearDown"/> is called.
        /// </summary>
        public IDisplayRect DisplayRect { get; private set; }

        public virtual string Name => "Element";

        protected ComponentProvider componentProvider;

        ElementNode elementNode;

        // The following are for the subclass implementations.

        protected abstract void _Init();
        protected abstract void _Update();
        protected virtual void _TearDown() { }
        protected virtual void _WidgetNodeChanged(WidgetNode oldNode) { }

        /// <summary>
        /// Instrument the element. Element not guaranteed to display correctly until
        /// after <see cref="Update()"/> is called.
        /// </summary>
        /// <param name="rectTransform"></param>
        internal void Init(IDisplayRect displayRect, ComponentProvider componentProvider)
        {
            if (IsInitialized) return;

            this.componentProvider = componentProvider ?? throw new ArgumentNullException();
            DisplayRect = displayRect ?? throw new ArgumentNullException();
            DisplayRect.Name = Name;

            _Init();

            IsInitialized = true;
            IsConnected = true;
        }

        internal void Disconnect()
        {
            IsConnected = false;
            // We don't actually disconnect the parent rect, as
            // at the end of an update cycle the element will either have
            // been reconnected, or be torn down.
        }

        internal void Reconnect(IDisplayRect parentRect)
        {
            if (!IsInitialized)
                throw new ElementUninitializedException();
            if (IsConnected)
                throw new Exception("Cannot reconnect a connected element.");

            DisplayRect.SetParent(parentRect);
            IsConnected = true;
        }

        internal void UpdateWidgetNode(WidgetNode newWidgetNode)
        {
            var oldNode = widgetNode;
            widgetNode = newWidgetNode;
            _WidgetNodeChanged(oldNode);
        }
        
        /// <summary>
        /// Update the display of the element.
        /// This should be called any time the context of the element changes,
        /// for instance, if a parent size or position changes, or the base widget changes.
        /// Must not be called before <see cref="Init(RectTransform)"/>.
        /// </summary>
        internal void Update()
        {
            if (!IsInitialized)
                throw new ElementUninitializedException("Element must be initialized before calling update.");
            
            SizeAndPositionRect();
            _Update();
        }

        internal virtual void TearDown()
        {
            // TODO Probably don't need to tear down if not initialized?
            _TearDown();
            DisplayRect?.TearDown();

            DisplayRect = null;
            IsInitialized = false;
            IsConnected = false;
            widgetNode = null;
        }

        internal void SetElementNode(ElementNode node)
        {
            elementNode = node;
        }

        protected void RequestRebuild()
        {
            elementNode?.RequestRebuild();
        }

        void SizeAndPositionRect()
        {
            if (widgetNode == null)
                return;

            DisplayRect.Size = widgetNode.Size;
            DisplayRect.Offset = widgetNode.GetElementOffset();
        }
    }

    public class ElementUninitializedException : Exception
    {
        public ElementUninitializedException() 
            : base() { }

        public ElementUninitializedException(string message)
            : base(message) { }
    }
}

using System;

namespace Flighter
{
    /// <summary>
    /// Used to impact the display tree.
    /// </summary>
    public abstract class Element
    {
        protected WidgetNode widgetNode;
        public W GetWidget<W>() where W : Widget
        {
            return widgetNode?.widget as W;
        }

        /// <summary>
        /// To be called when the element would like an update. Should
        /// not be called manually... Probably...
        /// </summary>
        protected Action setDirty;

        /// <summary>
        /// Whether the Element has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;
        
        // TODO: Update this doc. Assess if it needs to be public
        /// <summary>
        /// The RectTransform which represents this Element.
        /// Should not be modified outside the element, save for changing
        /// family hierarchy.
        /// 
        /// This will be null before <see cref="Init(IDisplayRect)"/> is called,
        /// and after <see cref="TearDown"/> is called.
        /// </summary>
        public IDisplayRect DisplayRect { get; private set; }

        public virtual string Name => "Element";

        protected ComponentProvider componentProvider;

        // The following are for the subclass implementations.

        protected abstract void _Init();
        protected abstract void _Update();

        /// <summary>
        /// Instrument the element. Element not guaranteed to display correctly until
        /// after <see cref="Update()"/> is called.
        /// </summary>
        /// <param name="rectTransform"></param>
        public void Init(IDisplayRect displayRect, ComponentProvider componentProvider)
        {
            if (IsInitialized) return;

            this.componentProvider = componentProvider ?? throw new ArgumentNullException();
            DisplayRect = displayRect ?? throw new ArgumentNullException();
            DisplayRect.Name = Name;

            _Init();

            IsInitialized = true;
        }

        public void Disconnect()
        {
            if (!IsInitialized)
                return;

            // TODO: Should this happen? It is over safe, as each element should be
            //       either added or pruned in the end, so intermediat opperations
            //       are not needed...
            DisplayRect.SetParent(null);
            IsInitialized = false;
        }

        public void Reconnect(IDisplayRect parentRect)
        {
            if (DisplayRect == null)
                throw new ElementUninitializedException();
            if (IsInitialized)
                throw new Exception("Cannot reconnect an initialized element.");

            DisplayRect.SetParent(parentRect);
            IsInitialized = true;
        }

        public void UpdateWidgetNode(WidgetNode newWidgetNode)
        {
            widgetNode = newWidgetNode;
        }
        
        /// <summary>
        /// Update the display of the element.
        /// This should be called any time the context of the element changes,
        /// for instance, if a parent size or position changes, or the base widget changes.
        /// Must not be called before <see cref="Init(RectTransform)"/>.
        /// </summary>
        public void Update()
        {
            if (!IsInitialized)
                throw new ElementUninitializedException("Element must be initialized before calling update.");
            
            SizeAndPositionRect();
            _Update();
        }

        public void TearDown()
        {
            DisplayRect?.TearDown();

            DisplayRect = null;
            IsInitialized = false;
            widgetNode = null;
        }

        public void SetDirtyCallback(Action setDirty)
        {
            this.setDirty = setDirty;
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

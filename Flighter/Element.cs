using System;

namespace Flighter
{
    /// <summary>
    /// Used to impact the display tree.
    /// </summary>
    public abstract class Element
    {
        protected WidgetNode widgetNode;
        protected W GetWidget<W>() where W : Widget
        {
            return widgetNode.widget as W;
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

        protected IComponentProvider componentProvider;

        // The following are for the subclass implementations.

        protected abstract void _Init();
        protected abstract void _Update();

        /// <summary>
        /// Instrument the element. Element not guaranteed to display correctly until
        /// after <see cref="Update()"/> is called.
        /// </summary>
        /// <param name="rectTransform"></param>
        public void Init(IDisplayRect displayRect, IComponentProvider componentProvider)
        {
            if (IsInitialized) return;

            componentProvider = componentProvider ?? throw new ArgumentNullException();
            DisplayRect = displayRect ?? throw new ArgumentNullException();

            _Init();

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
            if (!IsInitialized) return;

            DisplayRect.TearDown();

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
            DisplayRect.Offset = widgetNode.Offset;
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

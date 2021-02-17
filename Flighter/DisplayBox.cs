using System;
using System.Numerics;

namespace Flighter
{
    /// <summary>
    /// Used to impact the display tree.
    /// </summary>
    public abstract class DisplayBox
    {
        internal Widget widget;
        internal Size size;
        internal Vector2 offset;

        public W GetWidget<W>() where W : Widget 
            => widget as W; 

        /// <summary>
        /// Whether the Element has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;
        
        /// <summary>
        /// The DisplayRect this element will instrument.
        /// It should not be modified outside the element, save for changing
        /// family hierarchy.
        /// 
        /// This will be null before <see cref="Init(IDisplayRect)"/> is called,
        /// and after <see cref="TearDown"/> is called.
        /// </summary>
        public IDisplayRect DisplayRect { get; private set; }

        public virtual string Name => "DisplayBox";

        protected ComponentProvider componentProvider;

        // The following are for the subclass implementations.

        protected abstract void _Init();
        protected abstract void _Update();
        protected virtual void _TearDown() { }

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
            // No need to tear down if not yet initialized.
            if (!IsInitialized)
                return;

            _TearDown();
            DisplayRect?.TearDown();

            DisplayRect = null;
            IsInitialized = false;

            widget = null;
        }

        void SizeAndPositionRect()
        {
            if (widget == null)
                return;

            DisplayRect.Size = size;
            DisplayRect.Offset = offset;
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

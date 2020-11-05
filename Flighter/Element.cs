using System;

using UnityEngine;

namespace Flighter
{
    /// <summary>
    /// Used to impact the display tree.
    /// </summary>
    public abstract class Element
    {
        protected WidgetNode widgetNode;
        public Widget widget => widgetNode?.widget;

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
        /// This will be null before <see cref="Init(RectTransform)"/> is called,
        /// and after <see cref="TearDown"/> is called.
        /// </summary>
        public RectTransform RectTransform { get; private set; }

        public virtual string name => "Element";

        // The following are for the subclass implementations.

        protected abstract void _Init();
        protected abstract void _Update();

        /// <summary>
        /// Instrument the element, displaying it.
        /// </summary>
        /// <param name="rectTransform"></param>
        public void Init(RectTransform rectTransform)
        {
            if (IsInitialized) return;

            RectTransform = rectTransform;
            SizeAndPositionRect();
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
#if !TEST
            UnityEngine.Object.Destroy(RectTransform.gameObject);
#endif
            RectTransform = null;
            IsInitialized = false;
            widgetNode = null;
        }

        public void SetDirtyCallback(Action setDirty)
        {
            this.setDirty = setDirty;
        }

        void SizeAndPositionRect()
        {
#if !TEST
            RectTransform.anchoredPosition = WidgetNode.BuildResult.Offset;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, WidgetNode.BuildResult.Size.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, WidgetNode.BuildResult.Size.y);
#endif
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

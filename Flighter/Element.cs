using System;

using UnityEngine;

namespace Flighter
{
    /// <summary>
    /// Used to impact the display tree.
    /// </summary>
    public abstract class Element
    {
        ~Element()
        {
            Clear();
        }

        /// <summary>
        /// Whether the Element has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;
        
        /// <summary>
        /// The RectTransform which represents this Element.
        /// Should not be modified outside the element, save for changing
        /// family hierarchy.
        /// </summary>
        public RectTransform RectTransform { get; private set; }

        /// <summary>
        /// Instrument the element, displaying it.
        /// </summary>
        /// <param name="rectTransform"></param>
        public void Init(RectTransform rectTransform)
        {
            if (IsInitialized) return;

            RectTransform = rectTransform;

            _Init();

            IsInitialized = true;
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

            _Update();
        }

        /// <summary>
        /// Destroy anything used to display the element.
        /// Must recall <see cref="Init(RectTransform)"/> to display the element.
        /// Safe to call from any state.
        /// </summary>
        public void Clear()
        {
            // If not initialized, then there should be nothing to clear.
            if (!IsInitialized) return;

            _Clear();

            IsInitialized = false;
            // TODO: Should we just destroy the gameobject, and call it a day?
            //       (Have to make sure the kids are safe...).
            RectTransform = null;
        }

        public virtual string Name => "Element";

        // The following are for the subclass implementations.

        protected abstract void _Init();
        protected abstract void _Update();
        protected abstract void _Clear();
    }

    public class ElementUninitializedException : Exception
    {
        public ElementUninitializedException() 
            : base() { }

        public ElementUninitializedException(string message)
            : base(message) { }
    }
}

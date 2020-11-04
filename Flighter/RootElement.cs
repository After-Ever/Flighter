using System;

using UnityEngine;

namespace Flighter
{
    public class RootElement : Element
    {
        public override string Name => "Root";

        protected override void _Init() { }

        protected override void _Update() { }
    }

    /// <summary>
    /// The root of an element tree.
    /// </summary>
    public class RootElementNode : ElementNode
    {
        readonly RectTransform parent;

        /// <param name="parent">The parent object of
        /// the tree. This object will not be modified,
        /// and should not be modified externally.</param>
        public RootElementNode(RectTransform parent)
            : base(new RootElement(), null)
        {
            this.parent = parent;
        }

        // Overriding lets us avoid not having a parent,
        // by manually passing the rectTransform.
        protected override void InitElement()
        {
            Element.Init(parent);
        }
    }
}

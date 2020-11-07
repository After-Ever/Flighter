using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Flighter.Core
{
    public enum TextAlign
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public struct TextStyle
    {
        public string font;
        public float size;
        public float lineHeight;
        public Color color;
    }

    public enum TextOverflow
    {
        Clip,
        Ellipsis,
        Overflow
    }

    public abstract class TextComponent : Component
    {
        /// <summary>
        /// The text to display.
        /// </summary>
        public abstract string Data { get; set; }
        /// <summary>
        /// Style of the text.
        /// </summary>
        public abstract TextStyle Style { get; set; }
        /// <summary>
        /// Alignment of the text.
        /// </summary>
        public abstract TextAlign Alignment { get; set; }
        /// <summary>
        /// How the text handles overflowing.
        /// </summary>
        public abstract TextOverflow Overflow { get; set; }
    }

    public abstract class ColorComponent : Component
    {
        public abstract Color Color { get; set; }
    }
}

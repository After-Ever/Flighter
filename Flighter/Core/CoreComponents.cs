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

    public interface ITextComponent : IComponent
    {
        /// <summary>
        /// The text to display.
        /// </summary>
        string Data { get; set; }
        /// <summary>
        /// Style of the text.
        /// </summary>
        TextStyle Style { get; set; }
        /// <summary>
        /// Alignment of the text.
        /// </summary>
        TextAlign Alignment { get; set; }
        /// <summary>
        /// How the text handles overflowing.
        /// </summary>
        TextOverflow Overflow { get; set; }
    }

    public interface IColorComponent : IComponent
    {
        Color Color { get; set; }
    }
}

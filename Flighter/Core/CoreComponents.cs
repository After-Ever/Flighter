

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

    public struct Color
    {
        public float r, g, b, a;

        public Color(float r = 0, float g = 0, float b = 0, float a = 1)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return "r:" + r + ", g:" + g + ", b:" + b + ", a:" + a;
        }
    }

    public abstract class ColorComponent : Component
    {
        public abstract Color Color { get; set; }
    }
}

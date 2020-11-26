

namespace Flighter.Core
{
    public delegate Widget WidgetBuilder();

    public enum TextAlign
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public enum FontStyle
    {
        Normal,
        Bold,
        Italic,
        BoldAndItalic
    }

    public enum TextOverflow
    {
        Clip,
        Overflow
    }

    public interface IFontHandle { }

    public struct TextStyle
    {
        public IFontHandle font;
        public int size;
        public float lineSpacing;
        public TextAlign textAlign;
        public FontStyle fontStyle;
        public bool wrapLines;
        public TextOverflow textOverflow;
        public Color color;
    }

    public abstract class TextComponent : Component
    {
        public abstract string Data { get; set; }
        public abstract TextStyle? Style { get; set; }
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

    public enum BoxFit
    {
        /// <summary>
        /// As big as possible within the box, without distorting.
        /// </summary>
        Contain,
        /// <summary>
        /// Fill the entire frame, without distorting, cropping edges.
        /// </summary>
        Cover,
        /// <summary>
        /// Fill the entire frame, distorting.
        /// </summary>
        Fill,
        /// <summary>
        /// Top and bottom edges will be flush with edges.
        /// </summary>
        FitHeight,
        /// <summary>
        /// Left and right edges will be flush with edges.
        /// </summary>
        FitWidth
    }

    public abstract class ImageComponent : Component
    {
        public abstract IImageHandle ImageHandle { get; set; }
        public abstract Color? Color { get; set; }
    }

    public abstract class ClipComponent : Component { }
}

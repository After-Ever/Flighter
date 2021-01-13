

using System.Collections.Generic;

namespace Flighter.Core
{
    public delegate Widget WidgetBuilder(BuildContext context);

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
        // TODO: Define all these.
        public IFontHandle font;
        public int size;
        public float lineSpacing;
        public TextAlign textAlign;
        public FontStyle fontStyle;
        public bool wrapLines;
        public TextOverflow textOverflow;
        public Color color;

        /// <summary>
        /// Create a new <see cref="TextStyle"/> with the values specified.
        /// Values left null will be copied from this.
        /// </summary>
        /// <returns></returns>
        public TextStyle From(
            IFontHandle font = null,
            int? size = null,
            float? lineSpacing = null,
            TextAlign? textAlign = null,
            FontStyle? fontStyle = null,
            bool? wrapLines = null,
            TextOverflow? textOverflow = null,
            Color? color = null)
        {
            return new TextStyle
            {
                font = font ?? this.font,
                size = size ?? this.size,
                lineSpacing = lineSpacing ?? this.lineSpacing,
                textAlign = textAlign ?? this.textAlign,
                fontStyle = fontStyle ?? this.fontStyle,
                wrapLines = wrapLines ?? this.wrapLines,
                textOverflow = textOverflow ?? this.textOverflow,
                color = color ?? this.color,
            };
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TextStyle))
            {
                return false;
            }

            var style = (TextStyle)obj;
            return EqualityComparer<IFontHandle>.Default.Equals(font, style.font) &&
                   size == style.size &&
                   lineSpacing == style.lineSpacing &&
                   textAlign == style.textAlign &&
                   fontStyle == style.fontStyle &&
                   wrapLines == style.wrapLines &&
                   textOverflow == style.textOverflow &&
                   color == style.color;
        }

        public override int GetHashCode()
        {
            var hashCode = -1944643188;
            hashCode = hashCode * -1521134295 + EqualityComparer<IFontHandle>.Default.GetHashCode(font);
            hashCode = hashCode * -1521134295 + size.GetHashCode();
            hashCode = hashCode * -1521134295 + lineSpacing.GetHashCode();
            hashCode = hashCode * -1521134295 + textAlign.GetHashCode();
            hashCode = hashCode * -1521134295 + fontStyle.GetHashCode();
            hashCode = hashCode * -1521134295 + wrapLines.GetHashCode();
            hashCode = hashCode * -1521134295 + textOverflow.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(color);
            return hashCode;
        }

        public static bool operator ==(TextStyle style1, TextStyle style2)
        {
            return style1.Equals(style2);
        }

        public static bool operator !=(TextStyle style1, TextStyle style2)
        {
            return !(style1 == style2);
        }
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

        public override bool Equals(object obj)
        {
            if (!(obj is Color))
            {
                return false;
            }

            var color = (Color)obj;
            return r == color.r &&
                   g == color.g &&
                   b == color.b &&
                   a == color.a;
        }

        public override int GetHashCode()
        {
            var hashCode = -490236692;
            hashCode = hashCode * -1521134295 + r.GetHashCode();
            hashCode = hashCode * -1521134295 + g.GetHashCode();
            hashCode = hashCode * -1521134295 + b.GetHashCode();
            hashCode = hashCode * -1521134295 + a.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return "r:" + r + ", g:" + g + ", b:" + b + ", a:" + a;
        }

        public static bool operator ==(Color color1, Color color2)
        {
            return color1.Equals(color2);
        }

        public static bool operator !=(Color color1, Color color2)
        {
            return !(color1 == color2);
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

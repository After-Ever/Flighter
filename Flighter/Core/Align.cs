using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter.Core
{
    public struct Alignment
    {
        public static readonly Alignment TopLeft = new Alignment(0, 0);
        public static readonly Alignment TopCenter = new Alignment(0.5f, 0);
        public static readonly Alignment TopRight = new Alignment(1, 0);
        public static readonly Alignment MiddleLeft = new Alignment(0, 0.5f);
        public static readonly Alignment MiddleCenter = new Alignment(0.5f, 0.5f);
        public static readonly Alignment MiddleRight = new Alignment(1, 0.5f);
        public static readonly Alignment BottomLeft = new Alignment(0, 1);
        public static readonly Alignment BottomCenter = new Alignment(0.5f, 1);
        public static readonly Alignment BottomRight = new Alignment(1, 1);

        public float Horizontal
        {
            get => horizontal;
            set
            {
                CheckValue(value);
                horizontal = value;
            }
        }
        float horizontal;

        public float Vertical
        {
            get => vertical;
            set
            {
                CheckValue(value);
                vertical = value;
            }
        }
        float vertical;

        public Vector2 AsPoint() => new Vector2(horizontal, vertical);

        public Alignment(float horizontal = 0, float vertical = 0)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;

            CheckValue(horizontal);
            CheckValue(vertical);
        }

        void CheckValue(float value)
        {
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException("Alignment value must be between 0.0 and 1.0.");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Alignment))
            {
                return false;
            }

            var alignment = (Alignment)obj;
            return Horizontal == alignment.Horizontal &&
                   horizontal == alignment.horizontal &&
                   Vertical == alignment.Vertical &&
                   vertical == alignment.vertical;
        }

        public override int GetHashCode()
        {
            var hashCode = 801727432;
            hashCode = hashCode * -1521134295 + Horizontal.GetHashCode();
            hashCode = hashCode * -1521134295 + horizontal.GetHashCode();
            hashCode = hashCode * -1521134295 + Vertical.GetHashCode();
            hashCode = hashCode * -1521134295 + vertical.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Alignment alignment1, Alignment alignment2)
        {
            return alignment1.Equals(alignment2);
        }

        public static bool operator !=(Alignment alignment1, Alignment alignment2)
        {
            return !(alignment1 == alignment2);
        }
    }

    public class Align : LayoutWidget
    {
        public readonly Widget child;
        public readonly Alignment alignment;

        public Align(Widget child, Alignment alignment)
        {
            this.child = child;
            this.alignment = alignment;
        }

        public override bool Equals(object obj)
        {
            var align = obj as Align;
            return align != null &&
                   child.Equals(align.child) &&
                   alignment == align.alignment;
        }

        public override int GetHashCode()
        {
            var hashCode = 1394767171;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<Alignment>.Default.GetHashCode(alignment);
            return hashCode;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            var childSize = childNode.size;

            var extraSpace = new Vector2();
            if (float.IsPositiveInfinity(context.constraints.maxWidth))
            {
                extraSpace.X = 0;
            }
            else
            {
                extraSpace.X = context.constraints.maxWidth - childSize.width;
            }

            if (float.IsPositiveInfinity(context.constraints.maxHeight))
            {
                extraSpace.Y = 0;
            }
            else
            {
                extraSpace.Y = context.constraints.maxHeight - childSize.height;
            }

            var offset = new Vector2(extraSpace.X * alignment.Horizontal, extraSpace.Y * alignment.Vertical);

            childNode.Offset = offset;
            return new BuildResult((childSize.ToVector2() + extraSpace).ToSize());
        }
    }
}

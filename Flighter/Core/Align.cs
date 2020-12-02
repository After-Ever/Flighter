using System;
using System.Collections.Generic;
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

        public Point AsPoint() => new Point(horizontal, vertical);

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

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            var childSize = childNode.size;

            var extraSpace = new Point();
            if (float.IsPositiveInfinity(context.constraints.maxWidth))
            {
                extraSpace.x = 0;
            }
            else
            {
                extraSpace.x = context.constraints.maxWidth - childSize.width;
            }

            if (float.IsPositiveInfinity(context.constraints.maxHeight))
            {
                extraSpace.y = 0;
            }
            else
            {
                extraSpace.y = context.constraints.maxHeight - childSize.height;
            }

            var offset = new Point(extraSpace.x * alignment.Horizontal, extraSpace.y * alignment.Vertical);

            childNode.Offset = offset;
            return new BuildResult((childSize.ToPoint() + extraSpace).ToSize());
        }
    }
}

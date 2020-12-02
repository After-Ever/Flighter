using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public enum MainAxisAlignment
    {
        Start,
        Center,
        End,
        /// <summary>
        /// Place the same amount of space between elements, and half that space at the ends.
        /// </summary>
        SpaceAround,
        /// <summary>
        /// Place the same amount of space between elements, and none at the ends.
        /// </summary>
        SpaceBetween,
        /// <summary>
        /// Place the same amount of space between elements and at the ends.
        /// </summary>
        SpaceEvenly
    }

    public enum CrossAxisAlignment
    {
        Start,
        Center,
        End,
        Stretch
    }
    
    public enum MainAxisSize
    {
        Max,
        Min
    }

    public enum Axis
    {
        Horizontal,
        Vertical
    }

    public enum HorizontalDirection
    {
        LeftToRight,
        RightToLeft
    }

    public enum VerticalDirection
    {
        TopToBottom,
        BottomToTop
    }

    public class SequenceLayout : LayoutWidget
    {
        public readonly List<Widget> children;
        public readonly Axis axis;
        public readonly HorizontalDirection horizontalDirection;
        public readonly VerticalDirection verticalDirection;
        public readonly MainAxisAlignment mainAxisAlignment;
        public readonly CrossAxisAlignment crossAxisAlignment;
        public readonly MainAxisSize mainAxisSize;

        public SequenceLayout(
            List<Widget> children,
            Axis axis,
            HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight,
            VerticalDirection verticalDirection = VerticalDirection.TopToBottom,
            MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
            CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Start,
            MainAxisSize mainAxisSize = MainAxisSize.Max)
        {
            this.children = children;
            this.axis = axis;
            this.horizontalDirection = horizontalDirection;
            this.verticalDirection = verticalDirection;
            this.mainAxisAlignment = mainAxisAlignment;
            this.crossAxisAlignment = crossAxisAlignment;
            this.mainAxisSize = mainAxisSize;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            if (float.IsPositiveInfinity(MaxOnMain(context)) && mainAxisSize != MainAxisSize.Min)
                throw new Exception("Main axis must be bound.");

            Dictionary<Widget, WidgetNodeBuilder> widgetNodes = new Dictionary<Widget, WidgetNodeBuilder>();

            List<Widget> absoluteChildren = new List<Widget>();
            List<Flex> flexChildren = new List<Flex>();
            
            children.ForEach((c) =>
            {
                if (c is Flex f)
                    flexChildren.Add(f);
                else
                    absoluteChildren.Add(c);
            });

            BuildContext absoluteContext = MakeAbsoluteContext(context);

            float totalMainSize = 0;
            float crossAxisSize = 0;
            absoluteChildren.ForEach((w) =>
            {
                var n = widgetNodes[w] = node.AddChildWidget(w, absoluteContext);
                totalMainSize += SizeOnMain(n);
                crossAxisSize = Math.Max(crossAxisSize, SizeOnCross(n));
            });

            if (flexChildren.Count > 0)
            {
                float remainingOnMain = RemainingOnMain(context, totalMainSize);

                float totalFlex = 0;
                flexChildren.ForEach((f) => totalFlex += f.flexValue);

                flexChildren.ForEach((w) =>
                {
                    float mainSize = (w.flexValue / totalFlex) * remainingOnMain;
                    var n = widgetNodes[w] = node.AddChildWidget(w, MakeFlexContent(context, mainSize));
                    totalMainSize += SizeOnMain(n);
                    crossAxisSize = Math.Max(crossAxisSize, SizeOnCross(n));
                });
            }

            float freeSpaceOnMain = RemainingOnMain(context, totalMainSize);
            float runningMainOffset = StartOffsetOnMain(freeSpaceOnMain, children.Count);
            float mainSpace = SpaceOnMain(freeSpaceOnMain, children.Count);

            LayoutOrder().ForEach((w) =>
            {
                var n = widgetNodes[w];
                SetOffset(n, runningMainOffset, crossAxisSize);
                runningMainOffset += SizeOnMain(n) + mainSpace;
            });

            return new BuildResult(FinalSize(context, totalMainSize, crossAxisSize));
        }

        List<Widget> LayoutOrder()
        {
            if ((axis == Axis.Horizontal && horizontalDirection == HorizontalDirection.RightToLeft) ||
                   (axis == Axis.Vertical && verticalDirection == VerticalDirection.BottomToTop))
            {
                var r = new List<Widget>(children);
                r.Reverse();
                return r;
            }

            return children;
        }
        
        /// <param name="context"></param>
        /// <param name="mainTotal">Without added space.</param>
        /// <param name="crossTotal"></param>
        /// <returns></returns>
        Size FinalSize(BuildContext context, float mainTotal, float crossTotal)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    if (mainAxisSize == MainAxisSize.Max)
                        return new Size(context.constraints.maxWidth, crossTotal);
                    else
                        return new Size(mainTotal, crossTotal);
                case Axis.Vertical:
                    if (mainAxisSize == MainAxisSize.Max)
                        return new Size(crossTotal, context.constraints.maxHeight);
                    else
                        return new Size(crossTotal, mainTotal);
                default:
                    throw new NotSupportedException();
            }
        }

        float StartOffsetOnMain(float freeSpaceOnMain, int childCount)
        {
            if (childCount == 0)
                return 0;

            float value;
            switch (mainAxisAlignment)
            {
                case MainAxisAlignment.Start:
                    value = 0;
                    break;
                case MainAxisAlignment.End:
                    value = freeSpaceOnMain;
                    break;
                case MainAxisAlignment.Center:
                    return freeSpaceOnMain / 2;
                case MainAxisAlignment.SpaceAround:
                    return freeSpaceOnMain / childCount / 2;
                case MainAxisAlignment.SpaceBetween:
                    return 0;
                case MainAxisAlignment.SpaceEvenly:
                    return freeSpaceOnMain / (childCount + 1);
                default:
                    throw new NotSupportedException();
            }
            
            // Above, it is assumed the direction is LTR, or TTB. This will correct that.
            if ((axis == Axis.Horizontal && horizontalDirection == HorizontalDirection.RightToLeft) ||
                (axis == Axis.Vertical && verticalDirection == VerticalDirection.BottomToTop))
                value = freeSpaceOnMain - value;

            return value;
        }

        float SpaceOnMain(float freeSpaceOnMain, int childCount)
        {
            if (childCount == 0)
                return 0;

            switch (mainAxisAlignment)
            {
                case MainAxisAlignment.Start:
                case MainAxisAlignment.End:
                case MainAxisAlignment.Center:
                    return 0;
                case MainAxisAlignment.SpaceAround:
                    return freeSpaceOnMain / childCount;
                case MainAxisAlignment.SpaceBetween:
                    return freeSpaceOnMain / (childCount - 1);
                case MainAxisAlignment.SpaceEvenly:
                    return freeSpaceOnMain / (childCount + 1);
                default:
                    throw new NotSupportedException();
            }
        }

        float RemainingOnMain(BuildContext context, float takenOnMain)
        {
            switch (mainAxisSize)
            {
                case MainAxisSize.Max:
                    return MaxOnMain(context) - takenOnMain;
                case MainAxisSize.Min:
                    return Math.Max(0, MinOnMain(context) - takenOnMain);
                default:
                    throw new NotSupportedException();
            }
        }

        void SetOffset(WidgetNodeBuilder node, float offsetOnMain, float totalSizeOnCross)
        {
            var freeOnCross = totalSizeOnCross - SizeOnCross(node);
            float crossOffset;
            switch (crossAxisAlignment)
            {
                case CrossAxisAlignment.Stretch:
                    if (freeOnCross != 0)
                        throw new Exception("CrossAxisAlignment set to stretch, but there is free space!");
                    crossOffset = 0;
                    break;
                case CrossAxisAlignment.Start:
                    crossOffset = 0;
                    break;
                case CrossAxisAlignment.End:
                    crossOffset = freeOnCross;
                    break;
                case CrossAxisAlignment.Center:
                    crossOffset = freeOnCross / 2;
                    break;
                default:
                    throw new NotSupportedException();
            }

            // Above, it is assumed the direction is LTR, or TTB. This will correct that.
            if ((axis == Axis.Horizontal && verticalDirection == VerticalDirection.BottomToTop) ||
                (axis == Axis.Vertical && horizontalDirection == HorizontalDirection.RightToLeft))
                crossOffset = freeOnCross - crossOffset;

            switch (axis)
            {
                case Axis.Horizontal:
                    node.Offset = new Point(offsetOnMain, crossOffset);
                    break;
                case Axis.Vertical:
                    node.Offset = new Point(crossOffset, offsetOnMain);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        BuildContext MakeAbsoluteContext(BuildContext context)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return new BuildContext(new BoxConstraints(
                        minHeight: context.constraints.minHeight,
                        maxHeight: context.constraints.maxHeight));
                case Axis.Vertical:
                    return new BuildContext(new BoxConstraints(
                        minWidth: context.constraints.minWidth,
                        maxWidth: context.constraints.maxWidth));
                default:
                    throw new NotSupportedException();
            }
        }

        BuildContext MakeFlexContent(BuildContext context, float mainAxisSize)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return new BuildContext(new BoxConstraints(
                        minWidth: mainAxisSize,
                        maxWidth: mainAxisSize,
                        minHeight: context.constraints.minHeight,
                        maxHeight: context.constraints.maxHeight));
                case Axis.Vertical:
                    return new BuildContext(new BoxConstraints(
                        minHeight: mainAxisSize,
                        maxHeight: mainAxisSize,
                        minWidth: context.constraints.minWidth,
                        maxWidth: context.constraints.maxWidth));
                default:
                    throw new NotSupportedException();
            }
        }

        float MaxOnMain(BuildContext context)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return context.constraints.maxWidth;
                case Axis.Vertical:
                    return context.constraints.maxHeight;
                default:
                    throw new NotSupportedException();
            }
        }

        float MinOnMain(BuildContext context)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return context.constraints.minWidth;
                case Axis.Vertical:
                    return context.constraints.minHeight;
                default:
                    throw new NotSupportedException();
            }
        }

        float MaxOnCross(BuildContext context)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return context.constraints.maxHeight;
                case Axis.Vertical:
                    return context.constraints.maxWidth;
                default:
                    throw new NotSupportedException();
            }
        }

        float SizeOnMain(WidgetNodeBuilder node)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return node.size.width;
                case Axis.Vertical:
                    return node.size.height;
                default:
                    throw new NotSupportedException();
            }
        }

        float SizeOnCross(WidgetNodeBuilder node)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return node.size.height;
                case Axis.Vertical:
                    return node.size.width;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

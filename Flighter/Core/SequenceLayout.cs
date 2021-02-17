using System;
using System.Collections.Generic;
using System.Numerics;

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
        public readonly int crossAxisRestrictionIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        /// <param name="axis"></param>
        /// <param name="horizontalDirection"></param>
        /// <param name="verticalDirection"></param>
        /// <param name="mainAxisAlignment"></param>
        /// <param name="crossAxisAlignment"></param>
        /// <param name="mainAxisSize"></param>
        /// <param name="crossAxisRestrictionIndex">If set to a value other than -1,
        /// the widget at that index will be used to determine the max cross
        /// axis size for the rest of the widgets. Must be a non-Flex widget.</param>
        public SequenceLayout(
            List<Widget> children,
            Axis axis,
            HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight,
            VerticalDirection verticalDirection = VerticalDirection.TopToBottom,
            MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
            CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Start,
            MainAxisSize mainAxisSize = MainAxisSize.Max,
            int crossAxisRestrictionIndex = -1, 
            string key = null)
            : base(key)
        {
            this.children = children;
            this.axis = axis;
            this.horizontalDirection = horizontalDirection;
            this.verticalDirection = verticalDirection;
            this.mainAxisAlignment = mainAxisAlignment;
            this.crossAxisAlignment = crossAxisAlignment;
            this.mainAxisSize = mainAxisSize;
            this.crossAxisRestrictionIndex = crossAxisRestrictionIndex;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            
            Dictionary<Widget, IChildLayout> widgetNodes = new Dictionary<Widget, IChildLayout>(new WidgetEquality());

            List<Widget> absoluteChildren = new List<Widget>();
            List<Flex> flexChildren = new List<Flex>();
            
            children.ForEach((c) =>
            {
                if (c is Flex f)
                    flexChildren.Add(f);
                else
                    absoluteChildren.Add(c);
            });

            if (flexChildren.Count != 0)
            {
                if (mainAxisSize == MainAxisSize.Min)
                    throw new Exception("A SequenceLayout with min MainAxisSize cannot have Flex children.");

                if (float.IsInfinity(MaxOnMain(context)))
                    throw new Exception("Main axis must be bound when " +
                        "a SequenceLayout has Flex children, and the mainAxisSize is Max");
            }
            
            float totalFlex = 0;
            flexChildren.ForEach((f) => totalFlex += f.flexValue);

            BoxConstraints absoluteConstraints = MakeAbsoluteConstraints(context.constraints);

            float totalMainSize = 0;
            float crossAxisSize = 0;

            if (crossAxisRestrictionIndex != -1)
            {
                var crossRestrictingWidget = children[crossAxisRestrictionIndex];

                IChildLayout n;
                if (crossRestrictingWidget is Flex f)
                {
                    if (absoluteChildren.Count != 0)
                        throw new Exception("Cannot use a Flex widget as the cross axis restrictor" +
                            " when absolute children are present.");

                    var spaceToTakeFactor = totalFlex == 0
                        ? 0
                        : f.flexValue / totalFlex;

                    n = layout.LayoutChild(
                            crossRestrictingWidget,
                            MakeFlexConstraints(absoluteConstraints, MaxOnMain(context) * spaceToTakeFactor));
                }
                else
                    n = layout.LayoutChild(crossRestrictingWidget, absoluteConstraints);

                widgetNodes[crossRestrictingWidget] = n;

                if (axis == Axis.Horizontal)
                    absoluteConstraints.maxHeight = n.size.height;
                else
                    absoluteConstraints.maxWidth = n.size.width;
            }

            foreach (var w in absoluteChildren)
            {
                IChildLayout n;

                // Need to check if it already was layed out for crossAxisRestriction.
                if (widgetNodes.ContainsKey(w))
                    n = widgetNodes[w];
                else
                    n = widgetNodes[w] = layout.LayoutChild(w, absoluteConstraints);
                
                totalMainSize += SizeOnMain(n);
                crossAxisSize = Math.Max(crossAxisSize, SizeOnCross(n));
            };

            if (flexChildren.Count > 0)
            {
                var remainingOnMain = RemainingOnMain(context, totalMainSize);
                var sizePerFlex = totalFlex == 0
                    ? 0
                    : remainingOnMain / totalFlex;

                foreach (var w in flexChildren)
                {
                    IChildLayout n;

                    // Need to check if it already was layed out for crossAxisRestriction.
                    if (widgetNodes.ContainsKey(w))
                        n = widgetNodes[w];
                    else
                    {
                        float mainSize = w.flexValue * sizePerFlex;
                        n = widgetNodes[w] = layout.LayoutChild(w, MakeFlexConstraints(absoluteConstraints, mainSize));
                    }

                    totalMainSize += SizeOnMain(n);
                    crossAxisSize = Math.Max(crossAxisSize, SizeOnCross(n));
                };
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

            return FinalSize(context, totalMainSize, crossAxisSize);
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
                    return new Size(mainTotal, crossTotal);
                case Axis.Vertical:
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

        void SetOffset(IChildLayout layout, float offsetOnMain, float totalSizeOnCross)
        {
            var freeOnCross = totalSizeOnCross - SizeOnCross(layout);
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
                    layout.offset = new Vector2(offsetOnMain, crossOffset);
                    break;
                case Axis.Vertical:
                    layout.offset = new Vector2(crossOffset, offsetOnMain);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        BoxConstraints MakeAbsoluteConstraints(BoxConstraints constraints)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return new BoxConstraints(
                        minHeight: constraints.minHeight,
                        maxHeight: constraints.maxHeight);
                case Axis.Vertical:
                    return new BoxConstraints(
                        minWidth: constraints.minWidth,
                        maxWidth: constraints.maxWidth);
                default:
                    throw new NotSupportedException();
            }
        }

        BoxConstraints MakeFlexConstraints(BoxConstraints constraints, float mainAxisSize)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return new BoxConstraints(
                        minWidth: mainAxisSize,
                        maxWidth: mainAxisSize,
                        minHeight: constraints.minHeight,
                        maxHeight: constraints.maxHeight);
                case Axis.Vertical:
                    return new BoxConstraints(
                        minHeight: mainAxisSize,
                        maxHeight: mainAxisSize,
                        minWidth: constraints.minWidth,
                        maxWidth: constraints.maxWidth);
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

        float SizeOnMain(IChildLayout layout)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return layout.size.width;
                case Axis.Vertical:
                    return layout.size.height;
                default:
                    throw new NotSupportedException();
            }
        }

        float SizeOnCross(IChildLayout layout)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return layout.size.height;
                case Axis.Vertical:
                    return layout.size.width;
                default:
                    throw new NotSupportedException();
            }
        }

        public override bool Equals(object obj)
        {
            var layout = obj as SequenceLayout;
            return layout != null &&
                   EqualityComparer<List<Widget>>.Default.Equals(children, layout.children) &&
                   axis == layout.axis &&
                   horizontalDirection == layout.horizontalDirection &&
                   verticalDirection == layout.verticalDirection &&
                   mainAxisAlignment == layout.mainAxisAlignment &&
                   crossAxisAlignment == layout.crossAxisAlignment &&
                   mainAxisSize == layout.mainAxisSize;
        }

        public override int GetHashCode()
        {
            var hashCode = -1852071883;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Widget>>.Default.GetHashCode(children);
            hashCode = hashCode * -1521134295 + axis.GetHashCode();
            hashCode = hashCode * -1521134295 + horizontalDirection.GetHashCode();
            hashCode = hashCode * -1521134295 + verticalDirection.GetHashCode();
            hashCode = hashCode * -1521134295 + mainAxisAlignment.GetHashCode();
            hashCode = hashCode * -1521134295 + crossAxisAlignment.GetHashCode();
            hashCode = hashCode * -1521134295 + mainAxisSize.GetHashCode();
            return hashCode;
        }
    }
}

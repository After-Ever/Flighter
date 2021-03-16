using System;
using System.Collections.Generic;
using System.Numerics;

namespace Flighter.Core
{
    /// <summary>
    /// Lays out a collection in a strict grid, where every cell will have the same size.
    /// 
    /// If <see cref="matchSizeIndex"/> == -1, then the context must be constrained, and
    /// the size will be the space divided.
    /// Otherwise, the size will be the size of the specified child when layed out with the above constraints.
    /// </summary>
    public class Grid : LayoutWidget
    {
        public readonly List<Widget> children;
        public readonly int matchSizeIndex;
        public readonly Axis mainFillAxis;
        public readonly HorizontalDirection horizontalFillDirection;
        public readonly VerticalDirection verticalFillDirection;
        /// <summary>
        /// The number of slots on the axis perpendicular to <see cref="mainFillDirection"/>.
        /// </summary>
        public readonly int crossAxisCount;
        public readonly int mainAxisCount;

        public Grid(
            List<Widget> children,
            int crossAxisCount,
            Axis mainFillAxis = Axis.Vertical, 
            HorizontalDirection horizontalFillDirection = HorizontalDirection.LeftToRight, 
            VerticalDirection verticalFillDirection = VerticalDirection.TopToBottom, 
            int matchSizeIndex = 0,
            string key = null)
            : base(key)
        {
            this.children = children ?? throw new ArgumentNullException(nameof(children));
            this.matchSizeIndex = matchSizeIndex;
            this.mainFillAxis = mainFillAxis;
            this.horizontalFillDirection = horizontalFillDirection;
            this.verticalFillDirection = verticalFillDirection;
            this.crossAxisCount = crossAxisCount;

            var mainAxisCount = children.Count / crossAxisCount;
            mainAxisCount += (children.Count % crossAxisCount) > 0 ? 1 : 0;
            this.mainAxisCount = mainAxisCount;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            if (matchSizeIndex == -1 && context.constraints.IsUnconstrained)
                throw new Exception("Grid must be contained when no matchSizeIndex is provided!");

            BoxConstraints baseConstraints = new BoxConstraints();
            if (mainFillAxis == Axis.Horizontal)
            {
                baseConstraints.maxWidth = context.constraints.maxWidth / mainAxisCount;
                baseConstraints.maxHeight = context.constraints.maxHeight / crossAxisCount;
            }
            else
            {

                baseConstraints.maxWidth = context.constraints.maxWidth / crossAxisCount;
                baseConstraints.maxHeight = context.constraints.maxHeight / mainAxisCount;
            }

            IChildLayout matchChildLayout = null;
            if (matchSizeIndex != -1)
            {
                matchChildLayout = layout.LayoutChild(children[matchSizeIndex], baseConstraints);
                baseConstraints = BoxConstraints.Tight(matchChildLayout.size);
            }

            int onMain = 0, onCross = 0, i = -1;
            foreach (var c in children)
            {
                var childLayout = ++i == matchSizeIndex
                    ? matchChildLayout
                    : layout.LayoutChild(c, baseConstraints);
                childLayout.offset = GetOffset(ref onMain, ref onCross, baseConstraints);
            }

            if (mainFillAxis == Axis.Horizontal)
            {
                return new Size(
                    baseConstraints.maxWidth * mainAxisCount,
                    baseConstraints.maxHeight * crossAxisCount);
            }
            else
            {
                return new Size(
                    baseConstraints.maxWidth * crossAxisCount,
                    baseConstraints.maxHeight * mainAxisCount);
            }
        }

        Vector2 GetOffset(ref int onMain, ref int onCross, BoxConstraints cellConstraints)
        {
            float x, y;

            if (mainFillAxis == Axis.Horizontal)
            {
                if (horizontalFillDirection == HorizontalDirection.LeftToRight)
                    x = cellConstraints.maxWidth * onMain;
                else
                    x = cellConstraints.maxWidth * (mainAxisCount - onMain - 1);

                if (verticalFillDirection == VerticalDirection.TopToBottom)
                    y = cellConstraints.maxHeight * onCross;
                else
                    y = cellConstraints.maxWidth * (crossAxisCount - onCross - 1);
            }
            else
            {

                if (horizontalFillDirection == HorizontalDirection.LeftToRight)
                    x = cellConstraints.maxWidth * onCross;
                else
                    x = cellConstraints.maxWidth * (mainAxisCount - onCross - 1);

                if (verticalFillDirection == VerticalDirection.TopToBottom)
                    y = cellConstraints.maxHeight * onMain;
                else
                    y = cellConstraints.maxWidth * (crossAxisCount - onMain - 1);
            }

            if (++onCross == crossAxisCount)
            {
                onCross = 0;
                ++onMain;
            }

            return new Vector2(x, y);
        }
    }
}

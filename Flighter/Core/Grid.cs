using System;
using System.Collections.Generic;
using System.Numerics;

namespace Flighter.Core
{
    /// <summary>
    /// Lays out a collection in a strict grid.
    /// 
    /// Must have a constrained context.
    /// </summary>
    public class Grid : LayoutWidget
    {
        public readonly List<Widget> children;
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
            string key = null)
            : base(key)
        {
            this.children = children ?? throw new ArgumentNullException(nameof(children));
            this.mainFillAxis = mainFillAxis;
            this.horizontalFillDirection = horizontalFillDirection;
            this.verticalFillDirection = verticalFillDirection;
            this.crossAxisCount = crossAxisCount;

            var mainAxisCount = children.Count / crossAxisCount;
            mainAxisCount += (children.Count % crossAxisCount) > 0 ? 1 : 0;
            this.mainAxisCount = mainAxisCount;
        }

        public override bool Equals(object obj)
        {
            return obj is Grid grid &&
                   EqualityComparer<List<Widget>>.Default.Equals(children, grid.children) &&
                   mainFillAxis == grid.mainFillAxis &&
                   horizontalFillDirection == grid.horizontalFillDirection &&
                   verticalFillDirection == grid.verticalFillDirection &&
                   crossAxisCount == grid.crossAxisCount &&
                   mainAxisCount == grid.mainAxisCount;
        }

        public override int GetHashCode()
        {
            int hashCode = -868255901;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Widget>>.Default.GetHashCode(children);
            hashCode = hashCode * -1521134295 + mainFillAxis.GetHashCode();
            hashCode = hashCode * -1521134295 + horizontalFillDirection.GetHashCode();
            hashCode = hashCode * -1521134295 + verticalFillDirection.GetHashCode();
            hashCode = hashCode * -1521134295 + crossAxisCount.GetHashCode();
            hashCode = hashCode * -1521134295 + mainAxisCount.GetHashCode();
            return hashCode;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            if (context.constraints.IsUnconstrained)
                throw new Exception("Grid must be contained!");

            BoxConstraints cellConstraints = new BoxConstraints();
            if (mainFillAxis == Axis.Horizontal)
            {
                cellConstraints.maxWidth = context.constraints.maxWidth / mainAxisCount;
                cellConstraints.maxHeight = context.constraints.maxHeight / crossAxisCount;
            }
            else
            {

                cellConstraints.maxWidth = context.constraints.maxWidth / crossAxisCount;
                cellConstraints.maxHeight = context.constraints.maxHeight / mainAxisCount;
            }

            int onMain = 0, onCross = 0;
            foreach (var c in children)
            {
                var childLayout = layout.LayoutChild(c, cellConstraints);
                childLayout.offset = GetOffset(ref onMain, ref onCross, cellConstraints);
            }

            return new Size(context.constraints.maxWidth, context.constraints.maxHeight);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AEUtils;

namespace Flighter.Core
{
    public class Grid : LayoutWidget
    {
        public enum Mode
        {
            Uniform,
            UniformCrossAxis,
            UnconstrainedCrossAxis
        }

        public readonly Widget[,] widgets;
        public readonly Mode mode;
        public readonly Axis flexAxis;
        public readonly Alignment alignment;
        public readonly bool emptyAsFlex;

        public Grid(
            Widget[,] widgets, 
            Mode mode = Mode.UnconstrainedCrossAxis,
            Axis flexAxis = Axis.Horizontal,
            Alignment? alignment = null,
            bool emptyAsFlex = false)
        {
            this.widgets = widgets;
            this.mode = mode;
            this.flexAxis = flexAxis;
            this.alignment = alignment ?? Alignment.TopLeft;
            this.emptyAsFlex = emptyAsFlex;
        }

        public override Size Layout(BuildContext context, ILayoutController layoutController)
        {
            var columns = widgets.GetLength(0);
            var rows = widgets.GetLength(1);

            float getAxisParam(BoxConstraints c, bool main)
                => (flexAxis == Axis.Horizontal) == main
                    ? c.maxWidth
                    : c.maxHeight;
            void setAxisParam(ref BoxConstraints c, bool main, float value, float? verticalValue = null)
            {
                if ((flexAxis == Axis.Horizontal) == main)
                    c.maxWidth = value;
                else
                    c.maxHeight = verticalValue ?? value;
            }

            int mainCount, crossCount;
            if (flexAxis == Axis.Vertical)
            {
                mainCount = rows;
                crossCount = columns;
            }
            else
            {
                mainCount = columns;
                crossCount = rows;
            }

            var topConstraint = context.constraints;
            var children = new IChildLayout[columns, rows];

            var columnWidths = new float[columns];
            var rowHeights = new float[rows];

            void AddChild(int c, int r, BoxConstraints bc)
            {
                children[c, r] = layoutController.LayoutChild(
                    constraints: bc,
                    child: widgets[c, r]);
                columnWidths[c] = Math.Max(columnWidths[c], children[c, r].size.width);
                rowHeights[r] = Math.Max(rowHeights[r], children[c, r].size.height);
            }

            if (mode == Mode.Uniform)
            {
                var bc = new BoxConstraints(
                    maxWidth: topConstraint.maxWidth / columns,
                    maxHeight: topConstraint.maxHeight / rows);

                for (int c = 0; c < columns; ++c)
                {
                    for (int r = 0; r < rows; ++r)
                    {
                        AddChild(c, r, bc);
                    }
                }
            }
            else
            {
                var bc = BoxConstraints.Free;

                switch (mode)
                {
                    case Mode.UniformCrossAxis:
                        setAxisParam(ref bc, false,
                            getAxisParam(topConstraint, false) / crossCount);
                        break;
                    case Mode.UnconstrainedCrossAxis:
                        // Both axis are unconstrained.
                        break;
                    default:
                        throw new NotSupportedException(Enum.GetName(typeof(Mode), mode));
                }

                var flex = new List<(int c, int r, Flex child)>();


                var flexTotals = new float[mainCount];
                void addToFlexCount(int c, int r, float value)
                {
                    var i = flexAxis == Axis.Horizontal ? c : r;
                    flexTotals[i] += value;
                }

                // Layout all the absolute children
                for (var c = 0; c < columns; ++c)
                {
                    for (var r = 0; r < rows; ++r)
                    {
                        var w = widgets[c, r];
                        if (w == null)
                        {
                            if (emptyAsFlex)
                                w = new Spacer();
                            else
                                continue;
                        }

                        // Collect flex children, and track row and column totals
                        if (w is Flex f)
                        {
                            flex.Add((c, r, f));
                            addToFlexCount(c, r, f.flexValue);
                        }
                        else
                        {
                            AddChild(c, r, bc);
                        }
                    }
                }

                if (flex.Count > 0)
                {
                    if (float.IsInfinity(getAxisParam(topConstraint, true)))
                        throw new Exception("Flex axis must be constrained " +
                            "when Flex children are contained.");

                    var curMainSizes = flexAxis == Axis.Horizontal
                        ? columnWidths
                        : rowHeights;

                    var mainTotal = curMainSizes.Sum();
                    var remaining = getAxisParam(topConstraint, true) - mainTotal;
                    var flexTotal = flexTotals.Sum();

                    var used = flexTotals
                        .IndicesWhere(v => v != 0)
                        .Select(i => curMainSizes[i])
                        .Sum();

                    var targetSizes = flexTotals
                        .IndicesWhere(v => v != 0)
                        .Select(i => (i, (remaining + used) * (flexTotals[i] / flexTotal)))
                        .ToList();

                    targetSizes.Sort((a, b) => (a.Item2 - curMainSizes[a.i]) < (b.Item2 - curMainSizes[b.i]) ? -1 : 1);

                    while (remaining > 0 && targetSizes.Count > 0)
                    {
                        // Pop the last item.
                        (int i, float target) = targetSizes[targetSizes.Count - 1];
                        targetSizes.RemoveAt(targetSizes.Count - 1);

                        var r = target - curMainSizes[i];
                        if (r <= 0)
                            break;
                        var t = Math.Min(remaining, r);
                        remaining -= t;
                        curMainSizes[i] += t;
                    }

                    foreach (var (c, r, child) in flex)
                    {
                        setAxisParam(ref bc, true, columnWidths[c], rowHeights[r]);
                        AddChild(c, r, bc);
                    }
                }
            }

            var cOS = Vector2.Zero;
            for (int c = 0; c < columns; ++c)
            {
                var rOS = cOS;
                for (int r = 0; r < rows; ++r)
                {
                    var child = children[c, r];
                    if (child != null)
                    {
                        var extraSpace =
                            new Vector2(columnWidths[c], rowHeights[r])
                            - child.size.ToVector2();
                        child.offset = rOS + alignment.AsPoint() * extraSpace;
                    }

                    rOS += Vector2.UnitY * rowHeights[r];
                }

                cOS += Vector2.UnitX * columnWidths[c];
            }

            return new Size(
                columnWidths.Sum(),
                rowHeights.Sum());
        }
    }
}

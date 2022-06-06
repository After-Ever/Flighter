using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Flighter.Core
{
    public class Grid : LayoutWidget
    {
        public readonly Widget[,] widgets;
        public readonly Alignment alignment;
        public readonly bool emptyAsFlex;

        public Grid(
            Widget[,] widgets, 
            Alignment alignment = default, 
            bool emptyAsFlex = false)
        {
            this.widgets = widgets ?? throw new ArgumentNullException(nameof(widgets));
            this.alignment = alignment;
            this.emptyAsFlex = emptyAsFlex;
        }

        public override Size Layout(BuildContext context, ILayoutController layoutController)
        {
            var columns = widgets.GetLength(0);
            var rows = widgets.GetLength(1);
            var absoluteBc = BoxConstraints.Free;
            var children = new IChildLayout[columns, rows];
            var flex = new List<(int c, int r, Flex child)>();
            var columnFlexTotals = new float[columns];
            var columnWidths = new float[columns];
            var rowFlexTotals = new float[rows];
            var rowHeights = new float[rows];
            // Layout the absolute children
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
                        columnFlexTotals[c] += f.flexValue;
                        rowFlexTotals[r] += f.flexValue;
                        continue;
                    }

                    children[c, r] = layoutController.LayoutChild(w, absoluteBc);
                    columnWidths[c] = Math.Max(columnWidths[c], children[c, r].size.width);
                    rowHeights[r] = Math.Max(rowHeights[r], children[c, r].size.height);
                }
            }

            var absoluteWidth = columnWidths.Sum();
            var absoluteHeight = rowHeights.Sum();
            var bc = context.constraints;
            if (absoluteWidth > bc.maxWidth || absoluteHeight > bc.maxHeight)
                throw new Exception("Contents of grid exceed constraints.");
            if (flex.Count > 0)
            {
                if (bc.IsUnconstrained)
                    throw new Exception("Grid must be constrained when containing flex children.");

                var remainingWidth = bc.maxWidth - absoluteWidth;
                var remainingHeight = bc.maxHeight - absoluteHeight;
                // TODO If a column/row only has one flex, it has much less inluence
                //      than one with many... Should use average? Yeah!
                var totalColumnFlex = columnFlexTotals.Sum();
                var totalRowFlex = rowFlexTotals.Sum();

                IEnumerable<int> FlexItems(float[] flexValues)
                {
                    for (int i = 0; i < flexValues.Length; ++i)
                    {
                        if (flexValues[i] != 0)
                            yield return i;
                    }
                }
                // The amount currently taken up by columns/rows with flex children.
                float usedWidth = 0;
                float usedHeight = 0;
                foreach (var i in FlexItems(columnFlexTotals))
                    usedWidth += columnWidths[i];
                foreach (var i in FlexItems(rowFlexTotals))
                    usedHeight += rowHeights[i];

                var targetColumnWidths = new List<(int column, float width)>();
                var targetRowHeights = new List<(int row, float height)>();
                foreach (var i in FlexItems(columnFlexTotals))
                    targetColumnWidths.Add((i,
                        (remainingWidth + usedWidth) * (columnFlexTotals[i] / totalColumnFlex)));
                foreach (var i in FlexItems(rowFlexTotals))
                    targetRowHeights.Add((i,
                        (remainingHeight + usedHeight) * (rowFlexTotals[i] / totalRowFlex)));

                void Sort(List<(int, float)> l, float[] s)
                    => l.Sort((a, b) => (a.Item2 - s[a.Item1]) < (b.Item2 - s[b.Item1]) ? -1 : 1);
                Sort(targetColumnWidths, columnWidths);
                Sort(targetRowHeights, rowHeights);

                void Distribute(List<(int, float)> targets, float[] currentSizes, ref float remaining)
                {
                    while (remaining > 0 && targets.Count > 0)
                    {
                        // Pop the last item.
                        (int i, float target) = targets[targets.Count - 1];
                        targets.RemoveAt(targets.Count - 1);

                        var r = target - currentSizes[i];
                        if (r <= 0)
                            break;
                        var t = Math.Min(remaining, r);
                        remaining -= t;
                        currentSizes[i] += t;
                    }
                }
                Distribute(targetColumnWidths, columnWidths, ref remainingWidth);
                Distribute(targetRowHeights, rowHeights, ref remainingHeight);

                foreach (var (c, r, child) in flex)
                {
                    var con = BoxConstraints.Loose(columnWidths[c], rowHeights[r]);
                    children[c, r] = layoutController.LayoutChild(child, con);
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

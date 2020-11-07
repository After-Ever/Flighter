using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Column : LayoutWidget
    {
        public readonly List<Widget> children;

        public Column(List<Widget> children)
        {
            this.children = children;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            if (float.IsPositiveInfinity(context.constraints.maxHeight))
                throw new Exception("Column cannot have unconstrained height.");

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

            var absoluteBuildContext = new BuildContext(new BoxConstraints(
                    minWidth: context.constraints.minWidth,
                    maxWidth: context.constraints.maxWidth
                ));

            float totalAbsoluteHeight = 0;
            absoluteChildren.ForEach((c) =>
            {
                var n = widgetNodes[c] = node.AddChildWidget(c, absoluteBuildContext);
                totalAbsoluteHeight += n.size.height;
            });

            float remainingHeight = context.constraints.maxHeight - totalAbsoluteHeight;
            float totalFlex = 0;
            flexChildren.ForEach((f) => totalFlex += f.flexValue);

            flexChildren.ForEach((f) =>
            {
                float height = (f.flexValue / totalFlex) * remainingHeight;
                var constraint = new BoxConstraints(
                    minWidth: context.constraints.minWidth,
                    maxWidth: context.constraints.maxWidth,
                    minHeight: height,
                    maxHeight: height); // TODO: Could give children the option to not fill the full space?

                widgetNodes[f] = node.AddChildWidget(f, new BuildContext(constraint));
            });

            float runningHeightOffset = 0;
            float maxWidth = 0;
            // Now set all the offsets.
            // TODO: Currently just drop all the widgets flush with the left edge, stacked one ontop of the other.
            //       There should be options to change the alignment and spacing.
            children.ForEach((c) =>
            {
                var n = widgetNodes[c];
                n.Offset = new Point(0, runningHeightOffset);

                runningHeightOffset += n.size.height;
                maxWidth = Math.Max(maxWidth, n.size.width);
            });

            return new BuildResult(maxWidth, runningHeightOffset);
        }
    }
}

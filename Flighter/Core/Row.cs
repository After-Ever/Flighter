using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Row : LayoutWidget
    {
        public readonly List<Widget> children;

        public Row(List<Widget> children)
        {
            this.children = children;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            if (float.IsPositiveInfinity(context.constraints.maxWidth))
                throw new Exception("Row cannot have unconstrained width.");

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
                    minHeight: context.constraints.minHeight,
                    maxHeight: context.constraints.maxHeight
                ));

            float totalAbsoluteWidth = 0;
            absoluteChildren.ForEach((c) =>
            {
                var n = widgetNodes[c] = node.AddChildWidget(c, absoluteBuildContext);
                totalAbsoluteWidth += n.size.width;
            });

            float remainingWidth = context.constraints.maxWidth - totalAbsoluteWidth;
            float totalFlex = 0;
            flexChildren.ForEach((f) => totalFlex += f.flexValue);

            flexChildren.ForEach((f) =>
            {
                float width = (f.flexValue / totalFlex) * remainingWidth;
                var constraint = new BoxConstraints(
                    minWidth: width,// TODO: Could give children the option to not fill the full space?
                    maxWidth: width,
                    minHeight: context.constraints.minHeight,
                    maxHeight: context.constraints.maxHeight); 

                widgetNodes[f] = node.AddChildWidget(f, new BuildContext(constraint));
            });

            float runningWidthOffset = 0;
            float maxHeight = 0;
            // Now set all the offsets.
            // TODO: Currently just drop all the widgets flush with the top edge, stacked one beside of the other.
            //       There should be options to change the alignment and spacing.
            children.ForEach((c) =>
            {
                var n = widgetNodes[c];
                n.Offset = new Point(runningWidthOffset, 0);

                runningWidthOffset += n.size.width;
                maxHeight = Math.Max(maxHeight, n.size.height);
            });

            return new BuildResult(runningWidthOffset, maxHeight);
        }
    }
}

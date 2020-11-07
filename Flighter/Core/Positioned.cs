using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Positioned : LayoutWidget
    {
        public readonly Widget child;
        public readonly Point offset;

        public Positioned(Widget child, Point offset)
        {
            this.child = child;
            this.offset = offset;
        }

        public override bool IsSame(Widget other)
        {
            return base.IsSame(other);
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            childNode.Offset = offset;

            return new BuildResult(childNode.size);
        }
    }
}

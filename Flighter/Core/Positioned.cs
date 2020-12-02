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

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            childNode.Offset = offset;
            
            // TODO: Better doc
            // If the offset is negative, we will end up returning a size less than that of the child.
            // This is *mostly* the expectation. Really, the expectation is for children to stay within 
            // the bounds of their parent, and we cannot account for when it goes outside.
            var size = childNode.size.ToPoint() + offset;
            size.x = Math.Max(0, size.x);
            size.y = Math.Max(0, size.y);

            return new BuildResult(size.ToSize());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Unconstrained : LayoutWidget
    {
        readonly Widget child;

        public Unconstrained(Widget child)
        {
            this.child = child;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.LayoutChild(child, BoxConstraints.Free);
            return new BuildResult(childNode.size);
        }
    }
}

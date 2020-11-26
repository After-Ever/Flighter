using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class BoxConstrained : LayoutWidget
    {
        public readonly Widget child;
        public readonly BoxConstraints constraints;

        public BoxConstrained(Widget child, BoxConstraints constraints)
        {
            this.child = child ?? throw new ArgumentNullException();
            this.constraints = constraints;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, new BuildContext(constraints));
            return new BuildResult(childNode.size);
        }
    }
}

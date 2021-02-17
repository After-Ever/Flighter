using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Unconstrained : LayoutWidget
    {
        readonly Widget child;

        public Unconstrained(Widget child, string key = null)
            : base(key)
        {
            this.child = child;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            var childNode = layout.LayoutChild(child, BoxConstraints.Free);
            return childNode.size;
        }
    }
}

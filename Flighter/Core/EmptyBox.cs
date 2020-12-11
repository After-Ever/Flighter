using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    /// <summary>
    /// Takes up as much space as possible, without adding anything additional to the widget or element tree.
    /// </summary>
    public class EmptyBox : LayoutWidget
    {
        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            return new BuildResult(context.constraints.MaxSize);
        }

        public override bool Equals(object obj) => obj is EmptyBox;

        public override int GetHashCode() => 0;
    }
}

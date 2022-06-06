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
        public readonly bool noSpace;

        public EmptyBox(bool noSpace = false)
        {
            this.noSpace = noSpace;
        }

        public override Size Layout(BuildContext context, ILayoutController node)
        {
            return noSpace ? Size.Zero : context.constraints.MaxSize;
        }

        public override bool Equals(object obj) => obj is EmptyBox;

        public override int GetHashCode() => 0;
    }
}

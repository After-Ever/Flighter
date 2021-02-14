using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Column : SequenceLayout
    {
        public Column(
            List<Widget> children,
            HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight,
            VerticalDirection verticalDirection = VerticalDirection.TopToBottom,
            MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
            CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Start,
            MainAxisSize mainAxisSize = MainAxisSize.Max,
            int crossAxisRestrictionIndex = -1)
            : base(
                  children,
                  Axis.Vertical,
                  horizontalDirection,
                  verticalDirection,
                  mainAxisAlignment,
                  crossAxisAlignment,
                  mainAxisSize,
                  crossAxisRestrictionIndex)
        { }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

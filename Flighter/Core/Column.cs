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
            int crossAxisRestrictionIndex = -1, 
            string key = null)
            : base(
                  children,
                  Axis.Vertical,
                  horizontalDirection,
                  verticalDirection,
                  mainAxisAlignment,
                  crossAxisAlignment,
                  mainAxisSize,
                  crossAxisRestrictionIndex,
                  key)
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

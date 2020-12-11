﻿using System;
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
            MainAxisSize mainAxisSize = MainAxisSize.Max)
            : base(
                  children,
                  Axis.Vertical,
                  horizontalDirection,
                  verticalDirection,
                  mainAxisAlignment,
                  crossAxisAlignment,
                  mainAxisSize)
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

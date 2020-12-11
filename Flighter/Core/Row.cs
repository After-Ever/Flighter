﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Row : SequenceLayout
    {
        public Row(
            List<Widget> children,
            HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight,
            VerticalDirection verticalDirection = VerticalDirection.TopToBottom,
            MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
            CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Start,
            MainAxisSize mainAxisSize = MainAxisSize.Max)
            : base(
                  children,
                  Axis.Horizontal,
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

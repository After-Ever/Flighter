﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Flex : StatelessWidget
    {
        public readonly Widget child;
        public readonly float flexValue;

        public Flex(Widget child, float flexValue = 1)
        {
            this.child = child ?? throw new ArgumentNullException("Flex must have a child.");
            if (flexValue <= 0)
                throw new Exception("FlexValue must be positive");

            this.flexValue = flexValue;
        }

        public override Widget Build(BuildContext context)
        {
            return child;
        }

        public override bool IsSame(Widget other)
        {
            return
                other is Flex f &&
                f.flexValue == flexValue &&
                f.child.IsSame(child);
        }
    }
}
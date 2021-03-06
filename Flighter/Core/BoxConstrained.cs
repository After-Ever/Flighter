﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class BoxConstrained : LayoutWidget
    {
        public readonly Widget child;
        public readonly BoxConstraints constraints;

        public BoxConstrained(Widget child, BoxConstraints constraints, string key = null)
            : base(key)
        {
            this.child = child ?? throw new ArgumentNullException();
            this.constraints = constraints;
        }

        public override bool Equals(object obj)
        {
            var constrained = obj as BoxConstrained;
            return constrained != null &&
                   EqualityComparer<Widget>.Default.Equals(child, constrained.child) &&
                   EqualityComparer<BoxConstraints>.Default.Equals(constraints, constrained.constraints);
        }

        public override int GetHashCode()
        {
            var hashCode = -1456531856;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<BoxConstraints>.Default.GetHashCode(constraints);
            return hashCode;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
            => layout.LayoutChild(child, constraints).size;
    }
}

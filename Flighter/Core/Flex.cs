using System;
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
            if (flexValue < 0)
                throw new Exception("FlexValue cannot be negative");

            this.flexValue = flexValue;
        }

        public override Widget Build(BuildContext context)
        {
            return child;
        }

        public override bool Equals(object obj)
        {
            var flex = obj as Flex;
            return flex != null &&
                   EqualityComparer<Widget>.Default.Equals(child, flex.child) &&
                   flexValue == flex.flexValue;
        }

        public override int GetHashCode()
        {
            var hashCode = 1614855968;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + flexValue.GetHashCode();
            return hashCode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public struct BuildContext
    {
        public readonly BoxConstraints constraints;

        public BuildContext(BoxConstraints constraints)
        {
            this.constraints = constraints;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BuildContext))
            {
                return false;
            }

            var context = (BuildContext)obj;
            return EqualityComparer<BoxConstraints>.Default.Equals(constraints, context.constraints);
        }

        public override int GetHashCode()
        {
            return -1144114365 + EqualityComparer<BoxConstraints>.Default.GetHashCode(constraints);
        }

        public override string ToString() => "Constraints: " + constraints;
    }
}

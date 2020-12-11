using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Spacer : Flex
    {
        public Spacer(float flexValue = 1)
            : base(new EmptyBox(), flexValue) { }

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

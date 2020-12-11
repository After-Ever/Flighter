using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Spacer : Flex
    {
        public Spacer(float flexValue = 1)
            : base(new EmptyBox(), flexValue) { }
    }
}

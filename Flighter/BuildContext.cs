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
    }
}

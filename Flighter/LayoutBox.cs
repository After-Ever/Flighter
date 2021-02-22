using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    internal class LayoutBox : DisplayBox
    {
        public override string Name => name;
        readonly string name;

        public LayoutBox(string name)
        {
            this.name = name;
        }

        protected override void _Init() { }

        protected override void _Update() { }
    }
}

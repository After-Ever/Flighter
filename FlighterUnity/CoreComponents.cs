using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Flighter.Core;

namespace FlighterUnity
{
    public class UnityTextComponent : TextComponent
    {
        public override string Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TextStyle Style { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TextAlign Alignment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TextOverflow Overflow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public class UnityColorComponent : ColorComponent
    {
        public override Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

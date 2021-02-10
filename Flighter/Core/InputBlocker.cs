using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    public class InputBlocker : InputWidget
    {
        public InputBlocker(Widget child)
            : base(child, absorbWholeEvent: true) { }
    }
}

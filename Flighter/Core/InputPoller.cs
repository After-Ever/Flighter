using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    /// <summary>
    /// Widget which has the option to poll input each frame.
    /// </summary>
    public class InputPoller : InputWidget
    {
        public InputPoller(Widget child)
            : base(child, false) { }

        // TODO: This
    }
}

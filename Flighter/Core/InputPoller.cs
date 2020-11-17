using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    public delegate void InputUpdateCallback(IInputPoller poller);

    /// <summary>
    /// Widget which has the option to poll input each frame.
    /// </summary>
    public class InputPoller : InputWidget
    {
        readonly InputUpdateCallback callback;

        public InputPoller(Widget child, InputUpdateCallback callback)
            : base(child, false)
        {
            this.callback = callback;
        }

        public override void OnUpdate(IInputPoller inputPoller)
        {
            callback?.Invoke(inputPoller);
        }
    }
}

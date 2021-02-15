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

        public InputPoller(Widget child, InputUpdateCallback callback, string key = null)
            : base(child, false, key: key)
        {
            this.callback = callback;
        }

        public override bool Equals(object obj)
        {
            var poller = obj as InputPoller;
            return poller != null &&
                   base.Equals(obj) &&
                   EqualityComparer<InputUpdateCallback>.Default.Equals(callback, poller.callback);
        }

        public override int GetHashCode()
        {
            var hashCode = -1390279012;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<InputUpdateCallback>.Default.GetHashCode(callback);
            return hashCode;
        }

        public override void OnUpdate(IInputPoller inputPoller)
        {
            callback?.Invoke(inputPoller);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    public delegate void KeyEventCallback(KeyEventFilter filter, IInputPoller inputPoller);

    /// <summary>
    /// Widget that triggers callbacks when
    /// specified key events occure.
    /// </summary>
    public class KeyListener : InputWidget
    {
        readonly KeyEventCallback onKeyEvent;

        public KeyListener(Widget child, List<KeyEventFilter> keyEvents, KeyEventCallback onKeyEvent, bool onlyWhileHovering = false)
            : base(child, onlyWhileHovering, keyEvents)
        {
            this.onKeyEvent = onKeyEvent;
        }

        public override void OnKeyEvent(KeyEventFilter filter, IInputPoller inputPoller)
        {
            onKeyEvent?.Invoke(filter, inputPoller);
        }
    }
}

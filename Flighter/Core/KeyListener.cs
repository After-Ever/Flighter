using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    public delegate void KeyEventCallback(KeyEventFilter filter);

    /// <summary>
    /// Widget that triggers callbacks when
    /// specified key events occur.
    /// </summary>
    public class KeyListener : InputWidget
    {
        readonly KeyEventCallback onKeyEvent;

        public KeyListener(
            Widget child, 
            List<KeyEventFilter> keyEvents, 
            KeyEventCallback onKeyEvent,
            bool absorbEvents = true,
            bool absorbWholeEvent = false,
            string key = null)
            : base(child, absorbEvents, absorbWholeEvent, keyEvents, key: key)
        {
            this.onKeyEvent = onKeyEvent;
        }

        public override bool Equals(object obj)
        {
            var listener = obj as KeyListener;
            return listener != null &&
                   base.Equals(obj) &&
                   EqualityComparer<KeyEventCallback>.Default.Equals(onKeyEvent, listener.onKeyEvent);
        }

        public override int GetHashCode()
        {
            var hashCode = 67157251;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<KeyEventCallback>.Default.GetHashCode(onKeyEvent);
            return hashCode;
        }

        public override void OnKeyEvent(KeyEventFilter filter)
        {
            onKeyEvent?.Invoke(filter);
        }
    }
}

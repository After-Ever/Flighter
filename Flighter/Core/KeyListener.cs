using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    /// <summary>
    /// Widget that triggers callbacks when
    /// specified key events occure.
    /// </summary>
    public class KeyListener : InputWidget
    {
        public readonly List<(KeyCode, KeyEventType)> keyEvents;
        public readonly KeyEventCallback onKeyEvent;

        public KeyListener(Widget child, List<(KeyCode, KeyEventType)> keyEvents, KeyEventCallback onKeyEvent, bool onlyWhileHovering = false)
            : base(child, onlyWhileHovering)
        {
            this.keyEvents = keyEvents;
            this.onKeyEvent = onKeyEvent;
        }
    }
}

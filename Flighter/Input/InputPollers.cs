using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public abstract class KeyInputPoller
    {
        public abstract bool GetKey(KeyCode key);
        public abstract bool GetKeyDown(KeyCode key);
        public abstract bool GetKeyUp(KeyCode key);
    }

    public abstract class MouseInputPoller
    {
        public enum MouseButton
        {
            Left,
            Right,
            Middle
        }

        public abstract float ScrollDelta { get; }
        public abstract Point Position { get; }

        public abstract bool GetButton(MouseButton button);
        public abstract bool GetButtonDown(MouseButton button);
        public abstract bool GetButtonUp(MouseButton button);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public enum MouseButton
    {
        None,
        Left,
        Right,
        Middle,
        Mouse3,
        Mouse4,
        Mouse5
    }

    public enum MouseEventType
    {
        /// <summary>
        /// The pointer was moved.
        /// </summary>
        Move,
        /// <summary>
        /// The pointer is not moving.
        /// </summary>
        Hover,
        /// <summary>
        /// A mouse button was pressed.
        /// </summary>
        Down,
        /// <summary>
        /// A mouse button was released.
        /// </summary>
        Up,
        /// <summary>
        /// The button is being held.
        /// </summary>
        Active,
        /// <summary>
        /// The wheel was scrolled.
        /// </summary>
        Scroll
    }

    public struct MouseEventFilter
    {
        public MouseButton button;
        public MouseEventType type;

        public MouseEventFilter(MouseEventType type, MouseButton button = MouseButton.None)
        {
            this.button = button;
            this.type = type;

            switch(type)
            {
                case MouseEventType.Down:
                case MouseEventType.Up:
                case MouseEventType.Active:
                    if (button == MouseButton.None)
                        throw new Exception("Mouse button can't be None with filter type: " + type);
                    break;
            }
        }
    }
}

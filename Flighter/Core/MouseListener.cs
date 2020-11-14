using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    /// <summary>
    /// Widget which listens for mouse events.
    /// </summary>
    public class MouseListener : InputWidget
    {
        public readonly MouseEventCallback onMouseEvent;

        public MouseListener(Widget child, MouseEventCallback onMouseEvent, bool onlyWhileHovering = true)
            : base(child, onlyWhileHovering)
        {
            this.onMouseEvent = onMouseEvent;
        }
    }
}

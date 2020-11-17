using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    public delegate void MouseEventCallback(MouseEventFilter filter, IInputPoller poller);

    /// <summary>
    /// Widget which listens for mouse events.
    /// </summary>
    public class MouseListener : InputWidget
    {
        readonly MouseEventCallback onMouseEvent;

        public MouseListener(Widget child, List<MouseEventFilter> mouseEvents, MouseEventCallback onMouseEvent, bool onlyWhileHovering = true)
            : base(child, onlyWhileHovering, mouseEventsToReceive: mouseEvents)
        {
            this.onMouseEvent = onMouseEvent;
        }

        public override void OnMouseEvent(MouseEventFilter filter, IInputPoller inputPoller)
        {
            onMouseEvent?.Invoke(filter, inputPoller);
        }
    }
}

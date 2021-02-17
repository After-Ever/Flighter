using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    public delegate void MouseEventCallback(MouseEventFilter filter);

    /// <summary>
    /// Widget which listens for mouse events.
    /// </summary>
    public class MouseListener : InputWidget
    {
        readonly MouseEventCallback onMouseEvent;

        public MouseListener(
            Widget child, 
            List<MouseEventFilter> mouseEvents,
            MouseEventCallback onMouseEvent,
            bool absorbEvents = true,
            bool absorbWholeEvent = false,
            string key = null)
            : base(
                  child, 
                  absorbEvents,
                  absorbWholeEvent,
                  mouseEventsToReceive: mouseEvents,
                  key: key)
        {
            this.onMouseEvent = onMouseEvent;
        }

        public override bool Equals(object obj)
        {
            var listener = obj as MouseListener;
            return listener != null &&
                   base.Equals(obj) &&
                   EqualityComparer<MouseEventCallback>.Default.Equals(onMouseEvent, listener.onMouseEvent);
        }

        public override int GetHashCode()
        {
            var hashCode = 446973263;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<MouseEventCallback>.Default.GetHashCode(onMouseEvent);
            return hashCode;
        }

        public override void OnMouseEvent(MouseEventFilter filter)
        {
            onMouseEvent?.Invoke(filter);
        }
    }
}

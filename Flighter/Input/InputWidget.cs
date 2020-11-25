using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public abstract class InputWidget : StatelessWidget, IInputSubscriber
    {
        public readonly Widget child;

        public IEnumerable<KeyEventFilter> KeyEventsToReceive { get; private set; }
        public IEnumerable<MouseEventFilter> MouseEventsToReceive { get; private set; }

        public readonly bool onlyWhileHovering;

        public InputWidget(
            Widget child,
            bool onlyWhileHovering = true,
            List<KeyEventFilter> keyEventsToReceive = null,
            List<MouseEventFilter> mouseEventsToReceive = null)
        {
            this.child = child;
            this.onlyWhileHovering = onlyWhileHovering;
            this.KeyEventsToReceive = keyEventsToReceive;
            this.MouseEventsToReceive = mouseEventsToReceive;
        }

        public sealed override Widget Build(BuildContext context)
        {
            return child;
        }

        public virtual void OnKeyEvent(KeyEventFilter filter, IInputPoller inputPoller) { }

        public virtual void OnMouseEvent(MouseEventFilter filter, IInputPoller inputPoller) { }

        public virtual void OnUpdate(IInputPoller inputPoller) { }
    }
}

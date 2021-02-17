using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Flighter.Input
{
    public abstract class InputWidget : StatelessWidget, IInputSubscriber
    {
        public readonly Widget child;

        public IEnumerable<KeyEventFilter> KeyEventsToReceive { get; private set; }
        public IEnumerable<MouseEventFilter> MouseEventsToReceive { get; private set; }
        public bool AbsorbEvents { get; private set; }
        public bool AbsorbWholeEvent { get; private set; }

        public InputWidget(
            Widget child,
            bool absorbEvents = true,
            bool absorbWholeEvent = false,
            List<KeyEventFilter> keyEventsToReceive = null,
            List<MouseEventFilter> mouseEventsToReceive = null,
            string key = null)
            : base (key)
        {
            this.child = child;
            this.AbsorbEvents = absorbEvents;
            this.AbsorbWholeEvent = absorbWholeEvent;
            this.KeyEventsToReceive = keyEventsToReceive
                ?? Enumerable.Empty<KeyEventFilter>();
            this.MouseEventsToReceive = mouseEventsToReceive
                ?? Enumerable.Empty<MouseEventFilter>();
        }

        public sealed override Widget Build(BuildContext context)
        {
            return child;
        }

        public virtual void OnKeyEvent(KeyEventFilter filter) { }

        public virtual void OnMouseEvent(MouseEventFilter filter) { }

        public virtual void OnUpdate(IInputPoller inputPoller) { }
    }
}

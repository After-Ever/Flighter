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

        public readonly bool onlyWhileHovering;

        public InputWidget(
            Widget child,
            bool onlyWhileHovering = true,
            bool absorbEvents = true,
            bool absorbWholeEvent = false,
            List<KeyEventFilter> keyEventsToReceive = null,
            List<MouseEventFilter> mouseEventsToReceive = null)
        {
            this.child = child;
            this.onlyWhileHovering = onlyWhileHovering;
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

        public override bool Equals(object obj)
        {
            var widget = obj as InputWidget;
            return widget != null &&
                   EqualityComparer<Widget>.Default.Equals(child, widget.child) &&
                   EqualityComparer<IEnumerable<KeyEventFilter>>.Default.Equals(KeyEventsToReceive, widget.KeyEventsToReceive) &&
                   EqualityComparer<IEnumerable<MouseEventFilter>>.Default.Equals(MouseEventsToReceive, widget.MouseEventsToReceive) &&
                   onlyWhileHovering == widget.onlyWhileHovering;
        }

        public override int GetHashCode()
        {
            var hashCode = 387291533;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<KeyEventFilter>>.Default.GetHashCode(KeyEventsToReceive);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<MouseEventFilter>>.Default.GetHashCode(MouseEventsToReceive);
            hashCode = hashCode * -1521134295 + onlyWhileHovering.GetHashCode();
            return hashCode;
        }
    }
}

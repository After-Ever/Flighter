using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public abstract class InputWidget : StatelessWidget
    {
        public readonly Widget child;

        public readonly IEnumerable<KeyEventFilter> keyEventsToReceive;
        public readonly IEnumerable<MouseEventFilter> mouseEventsToReceive;

        public readonly bool onlyWhileHovering;

        public InputWidget(
            Widget child,
            bool onlyWhileHovering = true,
            List<KeyEventFilter> keyEventsToReceive = null,
            List<MouseEventFilter> mouseEventsToReceive = null)
        {
            this.child = child;
            this.onlyWhileHovering = onlyWhileHovering;
            this.keyEventsToReceive = keyEventsToReceive;
            this.mouseEventsToReceive = mouseEventsToReceive;
        }

        public sealed override Widget Build(BuildContext context)
        {
            return child;
        }

        public virtual void OnKeyEvent(KeyEventFilter filter, IInputPoller inputPoller) { }

        public virtual void OnMouseEvent(MouseEventFilter filter, IInputPoller inputPoller) { }

        public virtual void OnUpdate(IInputPoller inputPoller) { }
    }

    public class InputWidgetSubscriber : IInputSubscriber
    {
        public IEnumerable<KeyEventFilter> KeyEventsToRecieve => throw new NotImplementedException();
        public IEnumerable<MouseEventFilter> MouseEventsToRecieve => throw new NotImplementedException();

        readonly InputWidget widget;
        readonly WidgetNode node;

        public InputWidgetSubscriber(WidgetNode node)
        {
            this.node = node ?? throw new ArgumentNullException();
            widget = node.widget as InputWidget ?? throw new Exception("Cannot create InputWidgetListener around a non InputWidget!");
        }

        public bool ShouldReceiveUpdate(IInputPoller inputPoller)
        {
            return !widget.onlyWhileHovering 
                || node.IsHovering(inputPoller.MousePoller.Position);
        }

        public void OnKeyEvent(KeyEventFilter filter, IInputPoller inputPoller)
        {
            widget.OnKeyEvent(filter, inputPoller);
        }

        public void OnMouseEvent(MouseEventFilter filter, IInputPoller inputPoller)
        {
            widget.OnMouseEvent(filter, inputPoller);
        }

        public void OnUpdate(IInputPoller inputPoller)
        {
            widget.OnUpdate(inputPoller);
        }
    }
}

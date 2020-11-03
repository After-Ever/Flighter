using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class StateElement : Element
    {
        public override string Name => "State";

        public readonly State State; 

        public StateElement(State state)
        {
            State = state;
        }

        public void StateSet()
        {
            setDirty?.Invoke();
        }

        // Nothing to do on init...
        protected override void _Init() { }

        protected override void _Update()
        {
            // Called when the state should be rebuilt.

            // First we let State carry out updates.
            State.Updated();

            // Then rebuild its widget.
            var context = WidgetNode.BuildContext;
            var widget = State.Build(context);

            WidgetNode.ReplaceChildren(new List<(Widget, BuildContext)> {
                (widget, context)
            });
        }
    }
}

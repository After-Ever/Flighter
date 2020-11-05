using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class StateElement : Element
    {
        public override string name => "State";

        public readonly State state; 

        public StateElement(State state)
        {
            this.state = state ?? throw new ArgumentNullException();
            this.state.SetStateElement(this);
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
            state.Updated();

            // Then rebuild its widget.
            var context = widgetNode.buildContext;
            var widget = state.Build(context);

            widgetNode.ReplaceChildren(new List<(Widget, BuildContext)> {
                (widget, context)
            });
        }
    }
}

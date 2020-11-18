using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class StateElement : Element
    {
        public override string Name => "State";

        public readonly State state;

        /// <summary>
        /// Provided so the state can perform an initial build.
        /// Otherwise, the state would have no way to access the widget.
        /// </summary>
        public readonly WidgetNodeBuilder builder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="builder">Only needed when the state is being
        /// created from a <see cref="WidgetNodeBuilder"/></param>
        public StateElement(State state, WidgetNodeBuilder builder = null)
        {
            this.state = state ?? throw new ArgumentNullException();
            this.builder = builder;
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

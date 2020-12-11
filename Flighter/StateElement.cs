using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    internal class StateElement : Element
    {
        public override string Name => "State";

        public readonly State state;

        /// <summary>
        /// Provided so the state can perform an initial build.
        /// Otherwise, the state would have no way to access the widget.
        /// </summary>
        public WidgetNodeBuilder Builder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="builder">Only needed when the state is being
        /// created from a <see cref="WidgetNodeBuilder"/></param>
        public StateElement(State state)
        {
            this.state = state ?? throw new ArgumentNullException();
            this.state.SetStateElement(this);
        }

        internal void StateSet() => RequestRebuild();
        
        // The state is initiated when built in WidgetNodeBuilder, so nothing to do here!
        protected override void _Init() { }
        
        protected override void _Update() { }

        protected override void _WidgetNodeChanged(WidgetNode oldNode)
        {
            // Once the widget node has been changed by an ElementNode it is safe to forget the builder.
            Builder = null;
            state.WidgetChanged();
        }

        protected override void _TearDown()
        {
            state.Dispose();
        }
    }
}

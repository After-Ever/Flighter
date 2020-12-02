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

        /// <summary>
        /// Called by the state object anytime the state changes.
        /// </summary>
        public void StateSet()
        {
            // TODO: Should not rebuild unless state has been modified.
            //       Currently just rebuilding every frame. This is THE location
            //       modifications enter.
        }
        
        protected override void _Init()
        {
            state.Init();
        }

        protected override void _Update()
        {
            state.Updated();
        }

        protected override void _WidgetNodeChanged()
        {
            state.WidgetChanged();
        }

        protected override void _TearDown()
        {
            state.Dispose();
        }
    }
}

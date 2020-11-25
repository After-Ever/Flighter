using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public abstract class State
    {
        public abstract Widget Build(BuildContext context);

        protected W GetWidget<W>() where W : Widget
        {
            return (stateElement?.GetWidget<W>() 
                ?? stateElement?.builder?.widget 
                ?? initWidget) as W
                ?? throw new Exception("Could not get widget!");
        }

        StateElement stateElement;
        Widget initWidget;

        readonly Queue<Action> updates = new Queue<Action>();

        bool isDirty = false;

        public void SetStateElement(StateElement stateElement)
        {
            this.stateElement = stateElement;
        }

        /// <summary>
        /// Set this State's widget.
        /// To be used when 
        /// </summary>
        /// <param name="widget"></param>
        public void SetInitWidget(StatefulWidget widget)
        {
            initWidget = widget;
        }

        /// <summary>
        /// Called once the state has been added to the element tree.
        /// The State's widget will be avalible at this point, but not before.
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// Called when this state's widget has changed. This will be called before Build.
        /// </summary>
        public virtual void WidgetChanged() { }
        /// <summary>
        /// Called when the state's element is removed from the element tree.
        /// </summary>
        public virtual void Dispose() { }

        public void Updated()
        {
            // Make a local copy incase updates queue new updates.
            var actions = updates.ToArray();
            updates.Clear();

            // Set as not dirty first incase any actions dirty it.
            isDirty = false;
            foreach (var action in actions)
                action();
        }

        protected void SetState(Action action)
        {
            if (action != null)
                updates.Enqueue(action);

            if (!isDirty && stateElement != null)
            {
                isDirty = true;
                stateElement?.StateSet();
            }
        }
    }
}

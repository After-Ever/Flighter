using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public abstract class State<W> where W : StatefulWidget
    {
        public abstract Widget Build(BuildContext context);

        protected W widget
            => stateElement?.widget 
                ?? (stateElement?.builder?.widget as W) 
                ?? initWidget as W
                ?? throw new Exception("Could not get widget!");

        StateElement<W> stateElement;
        Widget initWidget;

        readonly Queue<Action> updates = new Queue<Action>();

        bool isDirty = false;

        public void SetStateElement(StateElement<W> stateElement)
        {
            this.stateElement = stateElement;
        }
        
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

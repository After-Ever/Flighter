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
            return stateElement?.GetWidget<W>() ?? stateElement?.builder?.widget as W
                ?? throw new Exception("Could not get widget!");
        }

        StateElement stateElement;
        readonly Queue<Action> updates = new Queue<Action>();

        bool isDirty = false;

        public void SetStateElement(StateElement stateElement)
        {
            this.stateElement = stateElement;
        }

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

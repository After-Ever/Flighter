using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public abstract class State
    {
        public abstract Widget Build(BuildContext context);

        StateElement stateElement;
        readonly Queue<Action> updates = new Queue<Action>();

        bool isDirty = false;

        public void SetStateElement(StateElement stateElement)
        {
            this.stateElement = stateElement;
        }

        public void Updated()
        {
            while(true)
            {
                try
                {
                    updates.Dequeue()();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }

            isDirty = false;
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

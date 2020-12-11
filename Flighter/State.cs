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
                ?? stateElement?.Builder?.widget) as W
                ?? throw new Exception("Could not get widget!");
        }

        StateElement stateElement;

        readonly Queue<Action> updates = new Queue<Action>();

        bool isDirty = false;

        internal void SetStateElement(StateElement stateElement)
        {
            this.stateElement = stateElement;
        }

        /// <summary>
        /// Called once when the state has been added to the element tree.
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
        
        /// <summary>
        /// Call all the actions passed to <see cref="SetState(Action)"/> since the last time this was called.
        /// </summary>
        internal void InvokeUpdates()
        {
            // Make a local copy incase updates queue new updates.
            var actions = updates.ToArray();
            updates.Clear();

            // Set as not dirty first incase any actions dirty it.
            isDirty = false;
            foreach (var action in actions)
                action();
        }

        /// <summary>
        /// Mark this widget as needing to be rebuilt.
        /// </summary>
        /// <param name="action">The state changing action. Will be invoked right before the State is rebuilt.
        /// One can make changes outside this method, but the changes will not be displayed until the tree happens to rebuild.</param>
        protected void SetState(Action action)
        {
            if (action != null)
                updates.Enqueue(action);

            if (!isDirty)
            {
                if (stateElement == null)
                    throw new NullReferenceException("State's element cannot be null when setting state.");

                stateElement.StateSet();

                isDirty = true;
            }
        }
    }
}

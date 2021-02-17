using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public abstract class State<T> : State where T : StatefulWidget
    {
        protected new T widget => base.widget as T
            ?? throw new Exception("State<" + nameof(T) + "> got widget of mismatching type!");
    }

    public abstract class State
    {
        internal Widget widget;
        protected BuildContext context { get; private set; }

        Action<State> onSetStateCallback;
        readonly Queue<Action> updates = new Queue<Action>();

        public abstract Widget Build(BuildContext context);

        internal void _Init(
            Widget widget,
            BuildContext context,
            Action<State> onSetStateCallback)
        {
            this.widget = widget;
            this.context = context;
            this.onSetStateCallback = onSetStateCallback;
            
            Init();
        }

        internal void _ReBuilt(
            Widget widget,
            BuildContext context)
        {
            this.widget = widget;
            this.context = context;

            WidgetChanged();
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
        /// Mark this widget as needing to be rebuilt.
        /// </summary>
        /// <param name="action">The state changing action. Will be invoked right before the State is rebuilt.
        /// One can make changes outside this method, but the changes will not be displayed until the tree happens to rebuild.
        /// The action will not occur if they state is disposed before it rebuilds.</param>
        protected void SetState(Action action)
        {
            if (onSetStateCallback == null)
                throw new Exception("Cannot call SetState before Init is called!");

            if (action != null)
                updates.Enqueue(action);
            
            onSetStateCallback(this);
        }

        /// <summary>
        /// Call all the actions passed to <see cref="SetState(Action)"/> since the last time this was called.
        /// </summary>
        internal void InvokeUpdates()
        {
            // Make a local copy incase updates queue new updates.
            var actions = updates.ToArray();
            updates.Clear();

            foreach (var action in actions)
                action();
        }
    }
}

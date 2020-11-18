using System;
using System.Collections.Generic;

namespace Flighter.Input
{
    /// <summary>
    /// Provides an <see cref="Input"/>.
    /// This object is responsible for calling <see cref="Input.Update"/>.
    /// </summary>
    public interface IInputProvider
    {
        Input GetInput();
    }

    public interface IInputSubscriber
    {
        /// <summary>
        /// The key events for which this subscriber would like <see cref="OnKeyEvent(KeyEventData)"/>
        /// to be called.
        /// </summary>
        IEnumerable<KeyEventFilter> KeyEventsToRecieve { get; }

        IEnumerable<MouseEventFilter> MouseEventsToRecieve { get; }

        /// <summary>
        /// Whether or not this subscriber should receieve the given update.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool ShouldReceiveUpdate(IInputPoller inputPoller);

        /// <summary>
        /// Called each time a key event listed in <see cref="KeyEventsToRecieve"/> occurs.
        /// If multiple such events happen in a single frame, this will be called multiple times,
        /// in the order of <see cref="KeyEventsToRecieve"/>.
        /// </summary>
        /// <param name="filter">The filter this event fired for.</param>
        /// <param name="inputPoller"></param>
        void OnKeyEvent(KeyEventFilter filter, IInputPoller inputPoller);

        /// <summary>
        /// Called each time a mouse event occurs.
        /// </summary>
        /// <param name="filter">The filter this event fired for.</param>
        /// <param name="inputPoller"></param>
        void OnMouseEvent(MouseEventFilter filter, IInputPoller inputPoller);

        /// <summary>
        /// Called each frame <see cref="ShouldReceiveUpdate(EventData)"/> returns true.
        /// </summary>
        /// <param name="mousePoller"></param>
        /// <param name="keyPoller"></param>
        void OnUpdate(IInputPoller inputPoller);
    }
    
    public class Input
    {
        public readonly IInputPoller inputPoller;

        readonly HashSet<IInputSubscriber> subscribers = new HashSet<IInputSubscriber>();

        public Input(IInputPoller inputPoller)
        {
            this.inputPoller = inputPoller;
        }

        /// <summary>
        /// A frame has passed, and this should recheck input for events.
        /// </summary>
        public void Update()
        {
            foreach (var s in subscribers)
            {
                if (!s.ShouldReceiveUpdate(inputPoller))
                    continue;

                if (s.KeyEventsToRecieve != null)
                {
                    foreach(var keyEvent in s.KeyEventsToRecieve)
                    {
                        if (CheckForKeyEvent(keyEvent))
                            s.OnKeyEvent(keyEvent, inputPoller);
                    }
                }

                if (s.MouseEventsToRecieve != null)
                {
                    foreach (var mouseEvent in s.MouseEventsToRecieve)
                    {
                        if (CheckForMouseEvent(mouseEvent))
                            s.OnMouseEvent(mouseEvent, inputPoller);
                    }
                }

                s.OnUpdate(inputPoller);
            }
        }

        /// <summary>
        /// Add the specified subscriber.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns>True if subscriber was added. False if they were
        /// already present.</returns>
        public bool AddSubscriber(IInputSubscriber subscriber)
        {
            return subscribers.Add(subscriber);
        }

        /// <summary>
        /// Remove the specified subscriber.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns>True if the subscriber was found and removed, false otherwise.</returns>
        public bool RemoveSubscriber(IInputSubscriber subscriber)
        {
            return subscribers.Remove(subscriber);
        }

        bool CheckForKeyEvent(KeyEventFilter filter)
        {
            switch (filter.type)
            {
                case KeyEventType.Active:
                    return inputPoller.KeyPoller.GetKey(filter.key);
                case KeyEventType.Down:
                    return inputPoller.KeyPoller.GetKeyDown(filter.key);
                case KeyEventType.Up:
                    return inputPoller.KeyPoller.GetKeyUp(filter.key);
                default:
                    throw new NotSupportedException("KeyEventType not supported: " + filter.type);
            }
        }

        bool CheckForMouseEvent(MouseEventFilter filter)
        {
            switch(filter.type)
            {
                case MouseEventType.Active:
                    return inputPoller.MousePoller.GetButton(filter.button);
                case MouseEventType.Down:
                    return inputPoller.MousePoller.GetButtonDown(filter.button);
                case MouseEventType.Up:
                    return inputPoller.MousePoller.GetButtonUp(filter.button);
                case MouseEventType.Move:
                    return !inputPoller.MousePoller.PositionDelta.Equals(Point.Zero);
                case MouseEventType.Scroll:
                    return inputPoller.MousePoller.ScrollDelta != 0;
                default:
                    throw new NotSupportedException("MouseEventType not supported: " + filter.type);
            }
        }
    }
}

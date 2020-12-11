using System;
using System.Collections.Generic;

namespace Flighter.Input
{
    public interface IInputSubscriber
    {
        /// <summary>
        /// The key events for which this subscriber would like <see cref="OnKeyEvent(KeyEventData)"/>
        /// to be called.
        /// </summary>
        IEnumerable<KeyEventFilter> KeyEventsToReceive { get; }

        IEnumerable<MouseEventFilter> MouseEventsToReceive { get; }

        /// <summary>
        /// Called each time a key event listed in <see cref="KeyEventsToReceive"/> occurs.
        /// If multiple such events happen in a single frame, this will be called multiple times,
        /// in the order of <see cref="KeyEventsToReceive"/>.
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

    public struct InputContext
    {
        public Point mousePosition;
    }

    public class Input
    {
        public readonly IInputPoller inputPoller;

        public Input(IInputPoller inputPoller)
        {
            this.inputPoller = inputPoller;
        }

        public void DistributeUpdates(IEnumerable<IInputSubscriber> subscribers)
        {
            foreach (var s in subscribers)
            {
                if (s.KeyEventsToReceive != null)
                {
                    foreach (var keyEvent in s.KeyEventsToReceive)
                    {
                        if (inputPoller.CheckForKeyEvent(keyEvent))
                            s.OnKeyEvent(keyEvent, inputPoller);
                    }
                }

                if (s.MouseEventsToReceive != null)
                {
                    foreach (var mouseEvent in s.MouseEventsToReceive)
                    {
                        if (inputPoller.CheckForMouseEvent(mouseEvent))
                            s.OnMouseEvent(mouseEvent, inputPoller);
                    }
                }

                s.OnUpdate(inputPoller);
            }
        }
    }
}

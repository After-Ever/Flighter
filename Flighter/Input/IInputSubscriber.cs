using System;
using System.Collections.Generic;
using System.Numerics;

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
        /// Whether the specific events received by this should passthrough this
        /// to widgets bellow it, or be absorbed. Events not captured by this will still passthrough.
        /// </summary>
        bool AbsorbEvents { get; }
        /// <summary>
        /// Block widgets bellow this from receiving the entire event,
        /// regardless if we receive it.
        /// </summary>
        bool AbsorbWholeEvent { get; }

        /// <summary>
        /// Called each time a key event listed in <see cref="KeyEventsToReceive"/> occurs.
        /// If multiple such events happen in a single frame, this will be called multiple times,
        /// in the order of <see cref="KeyEventsToReceive"/>.
        /// </summary>
        /// <param name="filter">The filter this event fired for.</param>
        /// <param name="inputPoller"></param>
        void OnKeyEvent(KeyEventFilter filter);

        /// <summary>
        /// Called each time a mouse event occurs.
        /// </summary>
        /// <param name="filter">The filter this event fired for.</param>
        /// <param name="inputPoller"></param>
        void OnMouseEvent(MouseEventFilter filter);
    }
}

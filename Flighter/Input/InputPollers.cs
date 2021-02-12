using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter.Input
{
    public interface IInputPoller
    {
        IKeyInputPoller KeyPoller { get; }
        IMouseInputPoller MousePoller { get; }
    }

    public interface IKeyInputPoller
    {
        bool GetKey(KeyCode key);
        bool GetKeyDown(KeyCode key);
        bool GetKeyUp(KeyCode key);
    }

    public interface IMouseInputPoller
    {
        float ScrollDelta { get; }
        Vector2 PositionDelta { get; }
        Vector2 Position { get; }

        bool GetButton(MouseButton button);
        bool GetButtonDown(MouseButton button);
        bool GetButtonUp(MouseButton button);
    }

    public static class InputPollerQuery
    {
        public static bool CheckForKeyEvent(this IInputPoller inputPoller, KeyEventFilter filter)
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

        public static bool CheckForMouseEvent(this IInputPoller inputPoller, MouseEventFilter filter)
        {
            switch (filter.type)
            {
                case MouseEventType.Active:
                    return inputPoller.MousePoller.GetButton(filter.button);
                case MouseEventType.Down:
                    return inputPoller.MousePoller.GetButtonDown(filter.button);
                case MouseEventType.Up:
                    return inputPoller.MousePoller.GetButtonUp(filter.button);
                case MouseEventType.Move:
                    return !inputPoller.MousePoller.PositionDelta.Equals(Vector2.Zero);
                case MouseEventType.Hover:
                    return inputPoller.MousePoller.PositionDelta.Equals(Vector2.Zero);
                case MouseEventType.Scroll:
                    return inputPoller.MousePoller.ScrollDelta != 0;
                default:
                    throw new NotSupportedException("MouseEventType not supported: " + filter.type);
            }
        }
    }
}

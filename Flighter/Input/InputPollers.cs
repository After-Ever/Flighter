using System;
using System.Collections.Generic;
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
        Point PositionDelta { get; }
        Point Position { get; }

        bool GetButton(MouseButton button);
        bool GetButtonDown(MouseButton button);
        bool GetButtonUp(MouseButton button);
    }
}

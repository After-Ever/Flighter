using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter.Input
{
    public static class NoInput
    {
        public static Input Input
        {
            get
            {
                if (input != null)
                    return input;

                return input = new Input(new NoInputPoller());
            }
        }
        static Input input;
    }

    class NoInputPoller : IInputPoller, IKeyInputPoller, IMouseInputPoller
    {
        public IKeyInputPoller KeyPoller => this;
        public IMouseInputPoller MousePoller => this;

        public float ScrollDelta => throw new NotImplementedException();

        public Vector2 PositionDelta => throw new NotImplementedException();

        public Vector2 Position => throw new NotImplementedException();

        public bool GetButton(MouseButton button)
        {
            throw new NotImplementedException();
        }

        public bool GetButtonDown(MouseButton button)
        {
            throw new NotImplementedException();
        }

        public bool GetButtonUp(MouseButton button)
        {
            throw new NotImplementedException();
        }

        public bool GetKey(KeyCode key)
        {
            throw new NotImplementedException();
        }

        public bool GetKeyDown(KeyCode key)
        {
            throw new NotImplementedException();
        }

        public bool GetKeyUp(KeyCode key)
        {
            throw new NotImplementedException();
        }
    }
}

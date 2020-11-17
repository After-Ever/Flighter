using Flighter;
using Flighter.Input;
using KeyCode = Flighter.Input.KeyCode;
using Input = UnityEngine.Input;

namespace FlighterUnity
{
    public class InputPoller : IInputPoller, IKeyInputPoller, IMouseInputPoller
    {
        Point lastPosition = Point.Zero;

        // TODO: Accept a world ui thing and camera pair to make this a world poller. 
        public InputPoller()
        {

        }

        /// <summary>
        /// Should be called at the end of a frame, right before the next frame of 
        /// input is avalible. This updates <see cref="PositionDelta"/>.
        /// </summary>
        public void FramePassed()
        {
            lastPosition = Position;
        }

        // IInputPoller
        public IKeyInputPoller KeyPoller => this;
        public IMouseInputPoller MousePoller => this;

        // IMouseInputPoller

        public float ScrollDelta => Input.mouseScrollDelta.y;
        public Point PositionDelta => Position - lastPosition;
        // TODO: Get more complex if based on a world tree.
        public Point Position => Input.mousePosition.ToPoint();

        public bool GetButton(MouseButton button)
        {
            return Input.GetMouseButton(button.ToUnity());
        }

        public bool GetButtonDown(MouseButton button)
        {
            return Input.GetMouseButtonDown(button.ToUnity());
        }

        public bool GetButtonUp(MouseButton button)
        {
            return Input.GetMouseButtonUp(button.ToUnity());
        }

        // IKeyInputPoller

        public bool GetKey(KeyCode key)
        {
            return Input.GetKey(key.ToUnity());
        }

        public bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key.ToUnity());
        }

        public bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key.ToUnity());
        }
    }
}

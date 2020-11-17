using System;

using Flighter;
using Flighter.Input;
using KeyCode = Flighter.Input.KeyCode;
using Input = UnityEngine.Input;
using UnityEngine;

namespace FlighterUnity
{
    public class InputPoller : IInputPoller, IKeyInputPoller, IMouseInputPoller
    {
        readonly Camera camera;
        readonly RectTransform rootRect;

        Point lastPosition = Point.Zero;

        public InputPoller(Camera camera = null, RectTransform rootRect = null)
        {
            if ((camera == null) != (rootRect == null))
                throw new ArgumentNullException("Must provider either both camera and rootRect, or neither.");

            this.camera = camera;
            this.rootRect = rootRect;
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
        public Point Position => ScreenPosToDisplayRectPos(Input.mousePosition);

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

        /// <summary>
        /// Returns (-1, -1) when bad! TODO, make better description.
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        Point ScreenPosToDisplayRectPos(Vector3 screenPos)
        {
            if (camera == null)
            {
                var point = screenPos.ToPoint();
                point.y = Screen.height - point.y;
                return point;
            }

            var pointerRay = camera.ScreenPointToRay(screenPos);
            var rectOrigin = rootRect.position;
            var rectRight = rootRect.right;
            var rectDown = -rootRect.up;
            var rectNormal = rootRect.forward;

            var inDirection = Vector3.Dot(rectNormal, pointerRay.direction);

            if (inDirection <= 0)
                return new Point(-1, -1);

            var d = Vector3.Dot(rectNormal, pointerRay.origin - rectOrigin) / inDirection;

            var pointOnRect = (pointerRay.origin + pointerRay.direction * d) - rectOrigin;

            var x = Vector3.Dot(pointOnRect, rectRight);
            var y = Vector3.Dot(pointOnRect, rectDown);

            return new Point(x, y);
        }
    }
}

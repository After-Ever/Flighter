using System;

using Flighter;
using Flighter.Input;
using UnityEngine;

using FlighterVec = System.Numerics.Vector2;
using KeyCode = Flighter.Input.KeyCode;
using Input = UnityEngine.Input;

namespace FlighterUnity
{
    public class InputPoller : IInputPoller, IKeyInputPoller, IMouseInputPoller
    {
        readonly RectTransform rootRect;
        readonly float pixelsPerUnit;

        FlighterVec lastPosition = FlighterVec.Zero;

        // TODO make explicit what changes when root rect is supplied.
        public InputPoller(RectTransform rootRect = null, float pixelsPerUnit = 1)
        {
            this.rootRect = rootRect;
            this.pixelsPerUnit = pixelsPerUnit;

            // TODO: Does this have to be the case?
            if (rootRect == null && pixelsPerUnit != 1)
                throw new Exception("Pixel per unit must be 1 for screen space poller.");
        }

        /// <summary>
        /// Should be called at the end of a frame, right before the next frame of 
        /// input is available. This updates <see cref="PositionDelta"/>.
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
        public FlighterVec PositionDelta => Position - lastPosition;
        public FlighterVec Position => ScreenPosToDisplayRectPos(Input.mousePosition);

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
        /// Determine the UI position from a position on the screen.
        /// Throws an <see cref="Exception"/> if the screen ray is parallel to the display rect.
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        FlighterVec ScreenPosToDisplayRectPos(Vector3 screenPos)
        {
            var cam = Camera.main;

            if (rootRect == null || cam == null)
            {
                var point = screenPos.ToFlighter();
                point.Y = cam.pixelHeight - point.Y;
                return point;
            }
            
            var pointerRay = cam.ScreenPointToRay(screenPos);
            var rectOrigin = rootRect.position;
            
            var rectRight = rootRect.right * pixelsPerUnit;
            var rectDown = -rootRect.up * pixelsPerUnit;
            var rectNormal = rootRect.forward;

            var inDirection = Vector3.Dot(rectNormal, pointerRay.direction);

            // Not actually pointing at the rect's plane.
            if (inDirection <= 0)
                throw new Exception("The screen point does not intersect with the UI");

            var d = Vector3.Dot(rectNormal, rectOrigin - pointerRay.origin) / inDirection;

            var intersectPoint = pointerRay.origin + pointerRay.direction * d;
            var pointOnRect = intersectPoint - rectOrigin;

            var x = Vector3.Dot(pointOnRect, rectRight);
            var y = Vector3.Dot(pointOnRect, rectDown);
            
            return new FlighterVec(x, y);
        }
    }
}

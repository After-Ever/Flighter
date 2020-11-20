using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class DisplayHandle
    {
        /// <summary>
        /// Tearn down this display.
        /// </summary>
        public void TearDown()
        {
            throw new NotImplementedException();
        }
    }

    public static class Display
    {
        /// <summary>
        /// Instrument <paramref name="widget"/> on the screen.
        /// Multiple calls to this will stack the widgets on one another, in call order
        /// (recent ontop of older).
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="pixelScale">How many logical pixels per screen pixel.</param>
        /// <returns></returns>
        public static DisplayHandle OnScreen(Widget widget, float pixelScale = 1)
        {
            throw new NotImplementedException();
        }

        public static DisplayHandle InWorld(Widget widget, Vector3 position, Quaternion rotation, float pixelsPerUnit, bool facingCam = false)
        {
            throw new NotImplementedException();
        }

        public static DisplayHandle OnTransform(Widget widget, Transform transform, float pixelsPerUnit, bool facingCam = false)
        {
            throw new NotImplementedException();
        }

        public static DisplayHandle ToRenderTexture(Widget widget, Size size, RenderTexture target)
        {
            throw new NotImplementedException();
        }
    }
}

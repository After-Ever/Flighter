using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;
using UnityEngine;
using Input = Flighter.Input.Input;

namespace FlighterUnity
{
    public class InputProvider : MonoBehaviour, IInputProvider
    {
        readonly InputPoller poller;
        readonly Input input;

        /// <summary>
        /// </summary>
        /// <param name="camera">Include this and <paramref name="rootRect"/> to make positions relative to a world rect.
        /// Otherwise, screen space will be used.</param>
        /// <param name="rootRect">Include this and <paramref name="camera"/> to make positions relative to a world rect.
        /// Otherwise, screen space will be used.</param>
        public InputProvider(Camera camera, RectTransform rootRect)
        {
            poller = new InputPoller(camera, rootRect);
            input = new Input(poller);
        }

        public Input GetInput() => input;

        /// <summary>
        /// Called by Unity each frame.
        /// </summary>
        void Update()
        {
            input.Update();
            poller.FramePassed();
        }
    }
}

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
        public RectTransform rootRect;

        InputPoller poller;
        Input input;

        public Input GetInput() => input;

        void Awake()
        {
            poller = new InputPoller(rootRect);
            input = new Input(poller);
        }

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

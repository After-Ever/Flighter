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

        // TODO Accept world widget tree and camera!
        public InputProvider()
        {
            poller = new InputPoller();
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

using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;
using UnityEngine;
using Input = Flighter.Input.Input;

namespace FlighterUnity
{
    public class InputProvider : MonoBehaviour
    {
        InputPoller poller;
        Input input;

        public Input GetInput() => input;

        public void SetPoller(InputPoller poller)
        {
            if (poller == null)
                throw new ArgumentNullException("Poller cannot be null");
            if (input != null)
                throw new Exception("Cannot set the input poller more than once.");

            input = new Input(this.poller = poller);
        }

        /// <summary>
        /// Called by Unity each frame.
        /// </summary>
        void Update()
        {
            input?.Update();
            poller?.FramePassed();
        }
    }
}

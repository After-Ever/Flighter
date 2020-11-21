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
        InputPoller poller = new InputPoller();
        Input input;

        public Input GetInput() => input;

        public InputProvider()
        {
            input = new Input(poller);
        }

        /// <summary>
        /// Set the root of the tree for which to receive input.
        /// Do not set a root for screen space trees.
        /// </summary>
        /// <param name="root"></param>
        public void SetRootRect(DisplayRect root)
        {
            poller = new InputPoller(root);
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

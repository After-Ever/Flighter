using System;
using System.Collections.Generic;
using System.Text;
using Flighter;
using Flighter.Input;
using UnityEngine;

namespace FlighterUnity
{
    public class InputProvider : MonoBehaviour
    {
        InputPoller poller;
        readonly List<WidgetNode> roots = new List<WidgetNode>();
        readonly Dictionary<WidgetForest, int> forests = new Dictionary<WidgetForest, int>();

        /// <summary>
        /// Stores the last <see cref="InputEvent"/> this processed.
        /// 
        /// It is recommended to set this Script's execution order before
        /// anything else so this can be used for the current frame of input.
        /// This can be done following: https://docs.unity3d.com/Manual/class-MonoManager.html
        /// 
        /// If on a frame a script accesses this before this runs <see cref="Update"/>,
        /// the values will represent the last frame.
        /// </summary>
        public InputEvent lastEventProcessed { get; private set; }

        public void SetPoller(InputPoller poller)
        {
            if (poller == null)
                throw new ArgumentNullException("Poller cannot be null");
            if (this.poller != null)
                throw new Exception("Cannot set the input poller more than once.");

            this.poller = poller;
        }

        /// <summary>
        /// Add a root.
        /// 
        /// Newly added roots recieve events, and can absorb them, before
        /// older roots.
        /// </summary>
        /// <param name="node"></param>
        public void AddRoot(WidgetNode node)
        {
            roots.Add(node);
            var f = node.forest;
            if (forests.ContainsKey(f))
            {
                forests[f]++;
            }
            else
            {
                forests[f] = 1;
            }
        }

        public void RemoveRoot(WidgetNode node)
        {
            if (!roots.Remove(node))
                return;

            var f = node.forest;
            if (--forests[f] == 0)
                forests.Remove(f);
        }

        /// <summary>
        /// Called by Unity each frame.
        /// </summary>
        void Update()
        {
            if (poller == null)
                throw new Exception("No poller has been set!");

            // Make copies incase the collections change durring iteration.
            var rootsToUpdate = new List<WidgetNode>(roots);
            var forestsToUpdate = new List<WidgetForest>(forests.Keys);

            var inputEvent = new InputEvent(poller);

            for(int i = rootsToUpdate.Count - 1; i >= 0; --i)
                rootsToUpdate[i].DistributeInputEvent(inputEvent);

            foreach (var f in forestsToUpdate)
            {
                foreach (var w in f.ContextFreeInputWidgets)
                {
                    foreach (var k in w.KeyEventsToReceive)
                    {
                        if (poller.CheckForKeyEvent(k))
                            w.OnKeyEvent(k);
                    }

                    foreach (var m in w.MouseEventsToReceive)
                    {
                        if (poller.CheckForMouseEvent(m))
                            w.OnMouseEvent(m);
                    }
                }
            }

            poller.FramePassed();

            lastEventProcessed = inputEvent;
        }
    }
}

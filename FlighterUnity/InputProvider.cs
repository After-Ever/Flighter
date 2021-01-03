using System;
using System.Collections.Generic;
using System.Text;
using Flighter;
using Flighter.Input;
using UnityEngine;
using Input = Flighter.Input.Input;

namespace FlighterUnity
{
    public class InputProvider : MonoBehaviour
    {
        public Input GetInput() => input;
        
        Input input;
        readonly List<WidgetNode> roots = new List<WidgetNode>();
        readonly Dictionary<WidgetForest, int> forests = new Dictionary<WidgetForest, int>();

        public void SetPoller(InputPoller poller)
        {
            if (poller == null)
                throw new ArgumentNullException("Poller cannot be null");
            if (input != null)
                throw new Exception("Cannot set the input poller more than once.");

            input = new Input(poller);
        }

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
            if (input == null)
                throw new Exception("No poller has been set!");

            var poller = input.inputPoller as InputPoller;
            var context = new InputContext
            { mousePosition = poller.MousePoller.Position };

            // Make copies incase the collections change durring iteration.
            var rootsToUpdate = new List<WidgetNode>(roots);
            var forestsToUpdate = new List<WidgetForest>(forests.Keys);

            foreach(var n in rootsToUpdate)
            {
                var inputWidgets = n.GetContextDependentInputWidgets(context);
                input.DistributeUpdates(inputWidgets);
            }

            foreach (var f in forestsToUpdate)
                input.DistributeUpdates(f.ContextFreeInputWidgets);

            poller.FramePassed();
        }
    }
}

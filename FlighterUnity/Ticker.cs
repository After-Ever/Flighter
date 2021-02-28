using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Flighter.Core;

namespace FlighterUnity
{
    public class Ticker : MonoBehaviour
    {
        public TickProvider TickProvider { get; private set; }

        private void Awake()
        {
            TickProvider = new TickProvider();
        }

        private void Update()
        {
            TickProvider.TriggerTick(Time.time);
        }
    }
}

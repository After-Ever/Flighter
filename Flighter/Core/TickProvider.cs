using Flighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flighter.Core
{
    public delegate void OnTick(float time, float delta);

    public class TickProvider
    {
        public event OnTick Tick;

        float lastTriggerTime;

        public readonly float firstDelta;

        public TickProvider(float firstDelta = 0.12f)
        {
            this.firstDelta = firstDelta;
            lastTriggerTime = -1;
        }

        public void TriggerTick(float time)
        {
            Tick?.Invoke(time, lastTriggerTime == -1 ? firstDelta : time - lastTriggerTime);

            lastTriggerTime = time;
        }
    }
}

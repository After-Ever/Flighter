using Flighter;
using UnityEngine;

namespace FlighterUnity
{
    public abstract class FlighterWidgetProvider : ScriptableObject
    {
        public abstract Widget GetWidget();
    }
}

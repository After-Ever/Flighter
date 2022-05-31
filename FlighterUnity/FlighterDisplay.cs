using System;
using Flighter;
using Flighter.Core;
using UnityEngine;

namespace FlighterUnity
{
    public class FlighterDisplay : MonoBehaviour
    {
        public FlighterWidgetProvider widgetProvider;
        [Tooltip("Optionally provided to wrap the widget with a TickSource")]
        public Ticker ticker;
        [Tooltip("Optionally provided to provide input to the widget")]
        public InputProvider inputProvider;

        Vector2 lastSize;
        ChangeNotifier sizeChangeNotifier = new ChangeNotifier();

        void Start()
        {
            var w = (Widget)new ChangeBuilder(
                notifier: sizeChangeNotifier,
                builder: _ => widgetProvider.GetWidget());
            if (ticker != null)
                w = new TickSource(w, ticker.TickProvider);

            var rect = new DisplayRect(transform as RectTransform
                ?? throw new Exception("FlighterDisplay can only be instrumented " +
                	"as a child of a canvas (RectTransform)"));

            lastSize = rect.transform.rect.size;

            Display.InstrumentWidget(w, rect, inputProvider);
        }

        // TODO This isn't working because the root build context is unchanged.
        //      (So the rebuild happens, but with incorrect constraints)
        //      Also, could just use value change notifier...
        //
        //      Need to rework some stuff (RootController?, TreeController?) to 
        //      get this working.
        //
        //      For now, dynamic displays are not supported :(
        //void Update()
        //{
        //    var curSize = (transform as RectTransform).rect.size;
        //    if (curSize != lastSize)
        //    {
        //        lastSize = curSize;
        //        sizeChangeNotifier.NotifyChange();
        //    }
        //}
    }
}

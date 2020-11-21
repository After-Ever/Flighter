using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class RootController : MonoBehaviour
    {
        WidgetNode widgetRoot;
        ElementNode elementRoot;

        public void SetRoot(WidgetNode widgetRoot, ElementNode elementRoot)
        {
            this.widgetRoot = widgetRoot;
            this.elementRoot = elementRoot;
        }

        public void SetRoot((WidgetNode, ElementNode) root)
        {
            this.widgetRoot = root.Item1;
            this.elementRoot = root.Item2;
        }

        public void TearDown()
        {
            widgetRoot?.Prune();

            widgetRoot = null;
            elementRoot = null;

            Destroy(gameObject);
        }

        void Update()
        {
            elementRoot?.Update();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Flighter;
using UnityEngine;

namespace FlighterUnity
{
    public class DisplayRectProvider : IDisplayRectProvider
    {
        readonly RectTransform rootTransform;

        public DisplayRectProvider(RectTransform rootTransform)
        {
            this.rootTransform = rootTransform ?? throw new ArgumentNullException();
        }

        public IDisplayRect CreateDisplayRect()
        {
            return new DisplayRect(rootTransform);
        }
    }
}

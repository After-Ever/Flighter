using Flighter.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class FontHandle : IFontHandle
    {
        public readonly Font font;

        public FontHandle(Font font)
        {
            this.font = font;
        }
    }
}

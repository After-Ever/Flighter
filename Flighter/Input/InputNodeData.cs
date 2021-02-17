using System;
using System.Collections.Generic;
using System.Numerics;

namespace Flighter.Input
{
    internal class InputNodeData
    {
        public readonly InputWidget widget;

        public readonly Size size;
        public readonly Vector2 offset;

        public InputNodeData(InputWidget widget, Size size, Vector2 offset)
        {
            this.widget = widget;

            this.size = size;
            this.offset = offset;
        }
    }
}

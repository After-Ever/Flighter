using System;
using System.Collections.Generic;
using System.Numerics;

using Flighter.Input;

namespace Flighter
{
    internal class WidgetNodeData : IChildLayout
    {
        public readonly Widget widget;
        public Size size { get; internal set; }
        public readonly BuildContext context;
        public Vector2 offset { get; set; }

        public readonly DisplayBox displayBox;
        public readonly State state;

        public WidgetNodeData(
            Widget widget, 
            BuildContext context,
            DisplayBox displayBox = null,
            State state = null)
        {
            this.widget = widget;
            this.context = context;

            this.displayBox = displayBox;
            this.state = state;
        }

        public WidgetNodeData RebuildCopy()
            => new WidgetNodeData(
                widget,
                context,
                displayBox,
                state);
    }
}

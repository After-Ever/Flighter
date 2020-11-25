using Flighter.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class WidgetForest
    {
        /// <summary>
        /// Input widgets in the tree which recieve updates regardless of input context (mouse position).
        /// </summary>
        public IEnumerable<InputWidget> ContextFreeInputWidgets => contextFreeInputWidgets;
        readonly HashSet<InputWidget> contextFreeInputWidgets = new HashSet<InputWidget>();

        public void WidgetAdded(Widget w)
        {
            if (w is InputWidget i && !i.onlyWhileHovering)
                contextFreeInputWidgets.Add(i);
        }

        public void WidgetRemoved(Widget w)
        {
            if (w is InputWidget i && !i.onlyWhileHovering)
                contextFreeInputWidgets.Remove(i);
        }
    }
}

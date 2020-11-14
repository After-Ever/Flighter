using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public abstract class InputWidget : StatelessWidget
    {
        public readonly Widget child;

        /// <summary>
        /// True if this widget only cares about input while the mouse is hovering.
        /// </summary>
        public readonly bool onlyWhileHovering;

        public InputWidget(Widget child, bool onlyWhileHovering)
        {
            this.child = child;
            this.onlyWhileHovering = onlyWhileHovering;
        }

        public sealed override Widget Build(BuildContext context)
        {
            return child;
        }
    }
}

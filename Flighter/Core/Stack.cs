using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Stack : LayoutWidget
    {
        public readonly List<Widget> children;

        public Stack(List<Widget> children, string key = null)
            : base(key)
        {
            this.children = children;
        }

        public override bool Equals(object obj)
        {
            var stack = obj as Stack;
            return stack != null &&
                   EqualityComparer<List<Widget>>.Default.Equals(children, stack.children);
        }

        public override int GetHashCode()
        {
            return 1458441378 + EqualityComparer<List<Widget>>.Default.GetHashCode(children);
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            float maxWidth, maxHeight;
            maxWidth = maxHeight = 0;

            List<(Widget, int)> deferredSizeWidgets = new List<(Widget, int)>();

            int i = 0;
            foreach (var c in children)
            {
                if (c is DeferSize)
                {
                    deferredSizeWidgets.Add((c, i++));
                }
                else
                {
                    var childSize = layout.LayoutChild(c, context.constraints).size;
                    maxWidth = Math.Max(childSize.width, maxWidth);
                    maxHeight = Math.Max(childSize.height, maxHeight);
                }
            };

            foreach ((var widget, var index) in deferredSizeWidgets)
            {
                var constraint = BoxConstraints.Tight(maxWidth, maxHeight);
                layout.LayoutChild(widget, constraint, index);
            }

            return new Size(maxWidth, maxHeight);
        }
    }

    /// <summary>
    /// For use as a direct child of Stack, will cause this widget
    /// to match the size of the containing stack.
    /// </summary>
    public class DeferSize : StatelessWidget
    {
        readonly Widget child;

        public DeferSize(Widget child)
        {
            this.child = child;
        }

        public override Widget Build(BuildContext context)
            => child;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Stack : LayoutWidget
    {
        public readonly List<Widget> children;

        public Stack(List<Widget> children)
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

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            float maxWidth, maxHeight;
            maxWidth = maxHeight = 0;

            children?.ForEach((c) =>
            {
                var childSize = node.AddChildWidget(c, context).size;
                maxWidth = Math.Max(childSize.width, maxWidth);
                maxHeight = Math.Max(childSize.height, maxHeight);
            });

            return new BuildResult(maxWidth, maxHeight);
        }
    }
}

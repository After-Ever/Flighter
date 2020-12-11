using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public struct EdgetInsets
    {
        public float left, top, right, bottom;

        /// <summary>
        /// Set all the edges with the same value.
        /// </summary>
        /// <param name="all"></param>
        public EdgetInsets(float all)
        {
            left = top = right = bottom = all;
        }

        /// <summary>
        /// Set edges individually.
        /// </summary>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        public EdgetInsets(
            float left = 0,
            float top = 0, 
            float right = 0, 
            float bottom = 0)
        {
            this.left = left;
            this.top = top;
            this.bottom = bottom;
            this.right = right;
        }
    }

    public class Padding : LayoutWidget
    {
        public readonly Widget child;
        public readonly EdgetInsets edgetInsets;

        public Padding(Widget child, EdgetInsets edgetInsets)
        {
            this.child = child ?? throw new ArgumentNullException("Padding must have child.");
            this.edgetInsets = edgetInsets;
        }

        public override bool Equals(object obj)
        {
            var padding = obj as Padding;
            return padding != null &&
                   EqualityComparer<Widget>.Default.Equals(child, padding.child) &&
                   EqualityComparer<EdgetInsets>.Default.Equals(edgetInsets, padding.edgetInsets);
        }

        public override int GetHashCode()
        {
            var hashCode = -867541505;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<EdgetInsets>.Default.GetHashCode(edgetInsets);
            return hashCode;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var horizontal = edgetInsets.left + edgetInsets.right;
            var vertical = edgetInsets.top + edgetInsets.bottom;

            var constraints = context.constraints;
            var childConstraints = new BoxConstraints(
                minWidth: Math.Max(0, constraints.minWidth - horizontal),
                minHeight: Math.Max(0, constraints.minHeight - vertical),
                maxWidth: constraints.maxWidth - horizontal,
                maxHeight: constraints.maxHeight - vertical);

            var child = node.AddChildWidget(
                this.child, 
                new BuildContext(childConstraints));

            child.Offset = new Point(edgetInsets.left, edgetInsets.top);
            var childSize = child.size;

            return new BuildResult(childSize.width + horizontal, childSize.height + vertical);
        }
    }
}

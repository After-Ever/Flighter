using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter.Core
{
    public struct EdgeInsets
    {
        public float left, top, right, bottom;

        /// <summary>
        /// Set all the edges with the same value.
        /// </summary>
        /// <param name="all"></param>
        public EdgeInsets(float all)
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
        public EdgeInsets(
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
        public readonly EdgeInsets edgeInsets;

        public Padding(Widget child, EdgeInsets edgeInsets)
        {
            this.child = child ?? throw new ArgumentNullException("Padding must have child.");
            this.edgeInsets = edgeInsets;
        }

        public override bool Equals(object obj)
        {
            var padding = obj as Padding;
            return padding != null &&
                   EqualityComparer<Widget>.Default.Equals(child, padding.child) &&
                   EqualityComparer<EdgeInsets>.Default.Equals(edgeInsets, padding.edgeInsets);
        }

        public override int GetHashCode()
        {
            var hashCode = -867541505;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<EdgeInsets>.Default.GetHashCode(edgeInsets);
            return hashCode;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var horizontal = edgeInsets.left + edgeInsets.right;
            var vertical = edgeInsets.top + edgeInsets.bottom;

            var constraints = context.constraints;
            var childConstraints = new BoxConstraints(
                minWidth: Math.Max(0, constraints.minWidth - horizontal),
                minHeight: Math.Max(0, constraints.minHeight - vertical),
                maxWidth: constraints.maxWidth - horizontal,
                maxHeight: constraints.maxHeight - vertical);

            var child = node.LayoutChild(
                this.child, 
                childConstraints);

            child.Offset = new Vector2(edgeInsets.left, edgeInsets.top);
            var childSize = child.size;

            return new BuildResult(childSize.width + horizontal, childSize.height + vertical);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter.Core
{
    public class Positioned : LayoutWidget
    {
        public readonly Widget child;
        public readonly Vector2 offset;

        public Positioned(Widget child, Vector2 offset)
        {
            this.child = child;
            this.offset = offset;
        }

        public override bool Equals(object obj)
        {
            var positioned = obj as Positioned;
            return positioned != null &&
                   EqualityComparer<Widget>.Default.Equals(child, positioned.child) &&
                   EqualityComparer<Vector2>.Default.Equals(offset, positioned.offset);
        }

        public override int GetHashCode()
        {
            var hashCode = -1240603729;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(offset);
            return hashCode;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            childNode.Offset = offset;

            // If the offset is negative, we will end up returning a size less than that of the child.
            // This is *mostly* the expectation. Really, the expectation is for children to stay within 
            // the bounds of their parent, and we cannot account for when it goes outside.
            var size = childNode.size.ToVector2() + offset;
            size.X = Math.Max(0, size.X);
            size.Y = Math.Max(0, size.Y);

            return new BuildResult(size.ToSize());
        }
    }
}

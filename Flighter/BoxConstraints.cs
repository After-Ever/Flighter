using System;

namespace Flighter
{
    public struct BoxConstraints
    {
        public float minHeight, maxHeight, minWidth, maxWidth;

        /// <summary>
        /// Create a new <see cref="BoxConstraints"/> with no constraints.
        /// </summary>
        public static BoxConstraints Free => new BoxConstraints(0);
        /// <summary>
        /// Create a new <see cref="BoxConstraints"/> fully constrained.
        /// </summary>
        public static BoxConstraints Zero => new BoxConstraints(0, 0, 0, 0);
        /// <summary>
        /// Create a new <see cref="BoxConstraints"/> with the given values
        /// as both min and max.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BoxConstraints Tight(float width, float height)
            => new BoxConstraints(width, width, height, height);
        public static BoxConstraints Tight(Size size)
            => Tight(size.width, size.height);
        public static BoxConstraints Loose(float width, float height)
            => new BoxConstraints(minWidth: width, minHeight: height);
        public static BoxConstraints Loose(Size size)
            => Loose(size.width, size.height);

        public BoxConstraints(
            float minWidth = 0,
            float maxWidth = float.PositiveInfinity,
            float minHeight = 0,
            float maxHeight = float.PositiveInfinity)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            this.minWidth = minWidth;
            this.maxWidth = maxWidth;

            CheckConstraints();
        }

        /// <summary>
        /// The biggest size which satisfies the constraint, if they are bounded.
        /// If a dimension is unbounded, the min will be returned.
        /// </summary>
        public Size MaxSize
        {
            get
            {
                float width = float.IsPositiveInfinity(maxWidth) ? minWidth : maxWidth;
                float height = float.IsPositiveInfinity(maxHeight) ? minHeight : maxHeight;

                return new Size(width, height);
            }
        }

        /// <summary>
        /// Create a new BoxConstraints based on this one, changing only those values which are provided.
        /// </summary>
        /// <returns></returns>
        public BoxConstraints From(
            float? minWidth = null,
            float? maxWidth = null,
            float? minHeight = null,
            float? maxHeight = null)
            => new BoxConstraints(
                minWidth: minWidth ?? this.minWidth,
                maxWidth: maxWidth ?? this.maxWidth,
                minHeight: minHeight ?? this.minHeight,
                maxHeight: maxHeight ?? this.maxHeight);

        public bool IsUnconstrained => float.IsPositiveInfinity(maxHeight) || float.IsPositiveInfinity(maxWidth);

        /// <summary>
        /// Checks the guarantees of the constraints, and throws if
        /// they are not met.
        /// </summary>
        void CheckConstraints()
        {
            if (minHeight < 0 || maxHeight < minHeight ||
                minWidth < 0 || maxWidth < minWidth)
                throw new BoxConstrainstException();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoxConstraints))
            {
                return false;
            }

            var constraints = (BoxConstraints)obj;

            return minHeight == constraints.minHeight &&
                   maxHeight == constraints.maxHeight &&
                   minWidth == constraints.minWidth &&
                   maxWidth == constraints.maxWidth;
        }

        public override int GetHashCode()
        {
            var hashCode = 467709450;
            hashCode = hashCode * -1521134295 + minHeight.GetHashCode();
            hashCode = hashCode * -1521134295 + maxHeight.GetHashCode();
            hashCode = hashCode * -1521134295 + minWidth.GetHashCode();
            hashCode = hashCode * -1521134295 + maxWidth.GetHashCode();
            return hashCode;
        }

        public override string ToString()
            => "Min width:" + minWidth
            + ", Max width:" + maxWidth
            + ", Min height:" + minHeight
            + ", Max height:" + maxHeight;
    }
    public class BoxConstrainstException : Exception { }
}

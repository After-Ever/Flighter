using System;
using AEUtils;

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
            => new BoxConstraints(maxWidth: width, maxHeight: height);
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
                throw new BoxConstrainstException(this.ToString());
        }

        public static BoxConstraints Lerp(
            BoxConstraints a, 
            BoxConstraints b, 
            float t,
            bool allowUnconstrained = true)
        {
            if (!allowUnconstrained
             && (a.IsUnconstrained
              || b.IsUnconstrained))
                throw new BoxConstrainstException("Lerping unconstrained " +
                    "constaints, but \"allowUnconstrained\" is false.");

            if (allowUnconstrained
            &&    (float.IsInfinity(a.minWidth) != float.IsInfinity(b.minWidth)
                || float.IsInfinity(a.minHeight) != float.IsInfinity(b.minHeight)
                || float.IsInfinity(a.maxWidth) != float.IsInfinity(b.maxWidth)
                || float.IsInfinity(a.maxHeight) != float.IsInfinity(b.maxHeight)))
                throw new BoxConstrainstException("If a parameter in 'a' is infinity, the matching " +
                	"parameter in 'b' must be infinity, and vis versa.");

            return new BoxConstraints(
                minWidth: float.IsInfinity(a.minWidth)
                    ? float.PositiveInfinity
                    : MathUtils.Lerp(a.minWidth, b.minWidth, t),
                minHeight: float.IsInfinity(a.minHeight)
                    ? float.PositiveInfinity
                    : MathUtils.Lerp(a.minHeight, b.minHeight, t),
                maxWidth: float.IsInfinity(a.maxWidth)
                    ? float.PositiveInfinity
                    : MathUtils.Lerp(a.maxWidth, b.maxWidth, t),
                maxHeight: float.IsInfinity(a.maxHeight)
                    ? float.PositiveInfinity
                    : MathUtils.Lerp(a.maxHeight, b.maxHeight, t));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoxConstraints))
            {
                return false;
            }

            var constraints = (BoxConstraints)obj;

            return (float.IsInfinity(minHeight) == float.IsInfinity(constraints.minHeight)) &&
                   (float.IsInfinity(minWidth)  == float.IsInfinity(constraints.minWidth))  &&
                   (float.IsInfinity(maxWidth)  == float.IsInfinity(constraints.maxWidth))  &&
                   (float.IsInfinity(maxHeight) == float.IsInfinity(constraints.maxHeight)) &&
                   (float.IsInfinity(minHeight) || Math.Abs(minHeight - constraints.minHeight) < float.Epsilon) &&
                   (float.IsInfinity(minWidth)  || Math.Abs(minWidth  - constraints.minWidth)  < float.Epsilon) &&
                   (float.IsInfinity(maxWidth)  || Math.Abs(maxWidth  - constraints.maxWidth)  < float.Epsilon) &&
                   (float.IsInfinity(maxHeight) || Math.Abs(maxHeight - constraints.maxHeight) < float.Epsilon);
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
    public class BoxConstrainstException : Exception 
    {
        public BoxConstrainstException(string m = default)
            : base(m) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public struct BoxConstraints
    {
        public float minHeight, maxHeight, minWidth, maxWidth;

        /// <summary>
        /// Create a new <see cref="BoxConstraints"/> with no constraints.
        /// </summary>
        public static BoxConstraints Free => new BoxConstraints();
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
        /// Checks the guarantees of the constraints, and throws if
        /// they are not met.
        /// </summary>
        void CheckConstraints()
        {
            if (!(0 <= minHeight && minHeight <= maxHeight && maxHeight <= float.PositiveInfinity) &&
                !(0 <= minWidth && minWidth <= maxWidth && maxWidth <= float.PositiveInfinity))
                throw new BoxConstrainstException();
        }
    }

    public class BoxConstrainstException : Exception { }

    public struct BuildContext
    {
        public readonly BoxConstraints constraints;

        public BuildContext(BoxConstraints constraints)
        {
            this.constraints = constraints;
        }
    }
}

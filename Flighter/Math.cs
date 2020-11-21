using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public struct Point
    {
        public static Point Zero => new Point();

        public float x;
        public float y;

        public Size ToSize() => new Size(x, y);

        public override bool Equals(object obj)
        {
            if (!(obj is Point p))
                return false;
            
            return x == p.x &&
                   y == p.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public Point(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        public static Point operator -(Point p)
        {
            return new Point(-p.x, -p.y);
        }

        public static Point operator -(Point a, Point b)
        {
            return a + (-b);
        }

        public static Point operator *(Point p, float s)
        {
            return new Point(p.x * s, p.y * s);
        }

        public static Point operator *(float s, Point p) => p * s;

        public static Point operator /(Point p, float s) => p * (1 / s);

        public static float operator *(Point a, Point b) => a.x * b.x + a.y * b.y;
    }

    public struct Size
    {
        public static Size Zero => new Size();

        public float width;
        public float height;

        public Point ToPoint() => new Point(width, height);

        public Size(float width = 0, float height = 0)
        {
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return "Width: " + width + ", Height:" + height;
        }
    }

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
        public static BoxConstraints Tight(Size size)
            => BoxConstraints.Tight(size.width, size.height);

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
        /// The biggest size which satisfies the constrainst, if they are bounded.
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
        /// Checks the guarantees of the constraints, and throws if
        /// they are not met.
        /// </summary>
        void CheckConstraints()
        {
            if (minHeight < 0 || maxHeight < minHeight ||
                minWidth < 0 || maxWidth < minWidth)
                throw new BoxConstrainstException();
        }
    }

    public class BoxConstrainstException : Exception { }
}

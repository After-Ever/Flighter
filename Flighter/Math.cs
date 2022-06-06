using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Flighter
{
    public static class VectorUtils
    {
        public static Size ToSize(this Vector2 vector) => new Size(vector.X, vector.Y);
    }

    [Serializable]
    public struct Size : IEquatable<Size>
    {
        public static Size Zero => new Size();

        public float width;
        public float height;

        public Vector2 ToVector2() => new Vector2(width, height);

        public Size(float width = 0, float height = 0)
        {
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return "Width: " + width + ", Height:" + height;
        }

        public override bool Equals(object obj)
        {
            return obj is Size && Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return width == other.width &&
                   height == other.height;
        }

        public override int GetHashCode()
        {
            var hashCode = 1263118649;
            hashCode = hashCode * -1521134295 + width.GetHashCode();
            hashCode = hashCode * -1521134295 + height.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Equals(size2);
        }

        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }
    }
}

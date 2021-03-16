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
    public struct Size
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
            if (!(obj is Size))
            {
                return false;
            }

            var size = (Size)obj;
            return width == size.width &&
                   height == size.height;
        }

        public override int GetHashCode()
        {
            var hashCode = 1263118649;
            hashCode = hashCode * -1521134295 + width.GetHashCode();
            hashCode = hashCode * -1521134295 + height.GetHashCode();
            return hashCode;
        }
    }
}

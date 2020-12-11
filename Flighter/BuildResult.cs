using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public struct BuildResult
    {
        public readonly Size size;

        public BuildResult(float width, float height)
        {
            size = new Size(width, height);
        }

        public BuildResult(Size size)
        {
            this.size = size;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BuildResult))
            {
                return false;
            }

            var result = (BuildResult)obj;
            return EqualityComparer<Size>.Default.Equals(size, result.size);
        }

        public override int GetHashCode()
        {
            return -1803920452 + EqualityComparer<Size>.Default.GetHashCode(size);
        }
    }
}

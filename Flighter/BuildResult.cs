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
    }
}

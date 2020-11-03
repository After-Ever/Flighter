using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Flighter
{
    public struct BuildResult
    {
        public readonly Vector2 size;

        public BuildResult(float x, float y)
        {
            size = new Vector2(x, y);
        }

        public BuildResult(Vector2 size)
        {
            this.size = size;
        }
    }
}

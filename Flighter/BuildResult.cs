using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Flighter
{
    public struct BuildResult
    {
        public readonly Vector2 size;

        public BuildResult(float width, float height)
        {
            size = new Vector2(width, height);
        }

        public BuildResult(Vector2 size)
        {
            this.size = size;
        }
    }
}

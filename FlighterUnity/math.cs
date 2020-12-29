using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using FlighterVec = System.Numerics.Vector2;

namespace FlighterUnity
{
    public static class Math
    {
        public static Vector2 ToUnity(this FlighterVec point)
        {
            return new UnityEngine.Vector2(point.X, point.Y);
        }

        public static FlighterVec ToFlighter(this Vector2 vector2)
        {
            return new FlighterVec(vector2.x, vector2.y);
        }

        public static FlighterVec ToFlighter(this Vector3 vector3)
        {
            return new FlighterVec(vector3.x, vector3.y);
        }

        public static Color ToUnity(this Flighter.Core.Color color)
        {
            return new Color(color.r, color.g, color.b, color.a);
        }

        public static Flighter.Core.Color ToFlighter(this Color color)
        {
            return new Flighter.Core.Color(color.r, color.g, color.b, color.a);
        }
    }
}

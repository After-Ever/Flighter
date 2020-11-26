using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public static class Math
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.x, point.y);
        }

        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point(vector2.x, vector2.y);
        }

        public static Point ToPoint(this Vector3 vector3)
        {
            return new Point(vector3.x, vector3.y);
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

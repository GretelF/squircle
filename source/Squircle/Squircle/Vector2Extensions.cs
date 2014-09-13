using Box2D.XNA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework
{
    public static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 vec, float angle)
        {
            var temp = (DVector2)vec;
            return (Vector2)temp.Rotate(angle);
        }

        public static Point ToPoint(this Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }
        public static bool EpsilonCompare(this Vector2 lhs, Vector2 rhs, float e = Settings.b2_epsilon)
        {
            return Math.Abs(lhs.X - rhs.X) <= e
                && Math.Abs(lhs.Y - rhs.Y) <= e;
        }

        public static bool IsInSameQuadrant(this Vector2 lhs, Vector2 rhs)
        {
            return Math.Sign(lhs.X) == Math.Sign(rhs.X)
                && Math.Sign(lhs.Y) == Math.Sign(rhs.Y);
        }
    }
}

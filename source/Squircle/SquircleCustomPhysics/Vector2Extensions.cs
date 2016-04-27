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

        public static int GetQuadrant(this Vector2 vec)
        {
            if (vec.X >= 0) // Right-hand side, i.e. 0 or 3
            {
                if (vec.Y >= 0) return 0;
                else            return 3;
            }
            else // Left-hand side, i.e. 1 or 2
            {
                if (vec.Y >= 0) return 1;
                else            return 2;
            }
        }

        public static bool IsInSameQuadrant(this Vector2 lhs, Vector2 rhs)
        {
            return lhs.GetQuadrant() == rhs.GetQuadrant();
        }
    }
}

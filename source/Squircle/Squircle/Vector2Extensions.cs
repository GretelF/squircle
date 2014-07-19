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
    }
}

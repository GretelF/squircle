using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public interface scShape
    {

    }

    class scCircleShape : scShape
    {
        public float radius;
        public Vector2 localPosition;
    }

    class scRectangleShape : scShape
    {
        public float width;
        public float height;
        public Vector2 localPosition;
    }

    /// <summary>
    /// The normal can be constructed by taking the direction from start to end and rotate it 90 degrees counterclockwise.
    /// start and end positions are in local space of the body they are contained in.
    /// </summary>
    class scEdgeShape : scShape
    {
        public Vector2 start;
        public Vector2 end;
    }
}

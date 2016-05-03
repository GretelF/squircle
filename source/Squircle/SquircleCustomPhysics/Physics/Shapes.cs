using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public interface scShape
    {
        scBoundingBox getBoundingBox(scTransform transform);
    }

    class scCircleShape : scShape
    {
        public float radius;
        public Vector2 localPosition;

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            return new scBoundingBox() { position = localPosition + transform.position, halfExtents = new Vector2(radius) };
        }
    }

    class scRectangleShape : scShape
    {
        public Vector2 localPosition;
        public Vector2 halfExtents;

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            var boundingBox = new scBoundingBox();
            boundingBox.position = localPosition;
            boundingBox.halfExtents = halfExtents;
            return boundingBox;
        }
    }

    /// <summary>
    /// The normal can be constructed by taking the direction from start to end and rotate it 90 degrees counterclockwise.
    /// start and end positions are in local space of the body they are contained in.
    /// </summary>
    class scEdgeShape : scShape
    {
        public Vector2 start;
        public Vector2 end;

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            return scBoundingUtils.createFromBoundingVertices(start, end);
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public enum scShapeType
    {
        Circle,
        Rectangle,
        Edge
    }

    public interface scShape
    {
        scShapeType ShapeType { get; }
        scBoundingBox getBoundingBox(scTransform transform);
    }

    class scCircleShape : scShape
    {
        public scShapeType ShapeType { get { return scShapeType.Circle; } }
        public float radius;
        public Vector2 localPosition;

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            return new scBoundingBox() { position = localPosition + transform.position, halfExtents = new Vector2(radius) };
        }
    }

    class scRectangleShape : scShape
    {
        public scShapeType ShapeType { get { return scShapeType.Rectangle; } }
        public Vector2 localPosition;
        public Vector2 halfExtents;

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            var boundingBox = new scBoundingBox();
            boundingBox.position = localPosition;
            boundingBox.halfExtents = halfExtents;
            return boundingBox;
        }

        public Rectangle asXNARectangle()
        {
            var rectangle = new Rectangle();
            rectangle.Offset(localPosition.ToPoint());
            rectangle.Width = (int)(2 * halfExtents.X);
            rectangle.Height = (int)(2 * halfExtents.Y);
            return rectangle;
        }
    }

    /// <summary>
    /// The normal can be constructed by taking the direction from start to end and rotate it 90 degrees counterclockwise.
    /// start and end positions are in local space of the body they are contained in.
    /// </summary>
    class scEdgeShape : scShape
    {
        public scShapeType ShapeType { get { return scShapeType.Edge; } }
        public Vector2 start;
        public Vector2 end;

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            return scBoundingUtils.createFromBoundingVertices(start, end);
        }
    }
}

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
        public readonly Vector2[] vertices = new Vector2[4];

        public scBoundingBox getBoundingBox(scTransform transform)
        {
            var lower = new Vector2();
            var upper = new Vector2();

            var rotated = new Vector2[4];
            rotated[0] = transform.position + vertices[0].Rotate(transform.rotation.radians);
            rotated[1] = transform.position + vertices[1].Rotate(transform.rotation.radians);
            rotated[2] = transform.position + vertices[2].Rotate(transform.rotation.radians);
            rotated[3] = transform.position + vertices[3].Rotate(transform.rotation.radians);
            
            lower.X = rotated.Min(v => v.X);
            lower.Y = rotated.Min(v => v.Y);
            upper.X = rotated.Max(v => v.X);
            upper.Y = rotated.Max(v => v.Y);

            return scBoundingUtils.createFromBoundingVertices(lower, upper);
        }

        static public scRectangleShape fromLocalPositionAndHalfExtents(Vector2 localPosition, Vector2 halfExtents)
        {
            var rectangleShape = new scRectangleShape();
            rectangleShape.vertices[0].X = localPosition.X - halfExtents.X;
            rectangleShape.vertices[0].Y = localPosition.Y - halfExtents.Y;
            rectangleShape.vertices[1].X = localPosition.X + halfExtents.X;
            rectangleShape.vertices[1].Y = localPosition.Y - halfExtents.Y;
            rectangleShape.vertices[2].X = localPosition.X + halfExtents.X;
            rectangleShape.vertices[2].Y = localPosition.Y + halfExtents.Y;
            rectangleShape.vertices[3].X = localPosition.X - halfExtents.X;
            rectangleShape.vertices[3].Y = localPosition.Y + halfExtents.Y;

            return rectangleShape;
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

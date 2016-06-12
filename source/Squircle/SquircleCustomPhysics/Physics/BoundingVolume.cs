using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public struct scBoundingBox
    {
        public Vector2 position;
        public Vector2 halfExtents;

        public Vector2 upperLeft { get { return position - halfExtents; } }
        public Vector2 lowerRight { get { return position + halfExtents; } }
        public Vector2 upperRight { get { return position + new Vector2(+halfExtents.X, -halfExtents.Y); } }
        public Vector2 lowerLeft { get { return position + new Vector2(-halfExtents.X, +halfExtents.Y); } }

        public float leftBorder { get { return position.X - halfExtents.X; } }
        public float rightBorder { get { return position.X + halfExtents.X; } }
        public float lowerBorder { get { return position.Y + halfExtents.Y; } }
        public float upperBorder { get { return position.Y - halfExtents.Y; } }



        public bool contains(scBoundingBox other)
        {
            var deltaUpperLeft = other.upperLeft - upperLeft;
            var deltaLowerRight = lowerRight - other.lowerRight;

            return deltaUpperLeft.X >= 0 && deltaUpperLeft.Y >= 0 && deltaLowerRight.X >= 0 && deltaLowerRight.Y >= 0;
        }
    }

    public static class scBoundingUtils
    {
        public static scBoundingBox union(scBoundingBox first, scBoundingBox second)
        {
            Vector2 firstUpper = first.upperLeft;
            Vector2 firstLower = first.lowerRight;
            Vector2 secondUpper = second.upperLeft;
            Vector2 secondLower = second.lowerRight;

            var newUpper = new Vector2(Math.Min(firstUpper.X, secondUpper.X), Math.Min(firstUpper.Y, secondUpper.Y));
            var newLower = new Vector2(Math.Max(firstLower.X, secondLower.X), Math.Max(firstLower.Y, secondLower.Y));
            return createFromBoundingVertices(newUpper, newLower);
        }

        public static scBoundingBox createFromBoundingVertices(Vector2 upperLeft, Vector2 lowerRight)
        {
            var boundingBox = new scBoundingBox();
            boundingBox.halfExtents = (lowerRight - upperLeft) / 2.0f;
            boundingBox.position = upperLeft + boundingBox.halfExtents;

            return boundingBox;
        }

        public static scBoundingBox createFromPositionAndHalfExtents(Vector2 position, Vector2 halfExtents)
        {
            var boundingBox = new scBoundingBox();
            boundingBox.position = position;
            boundingBox.halfExtents = halfExtents;

            return boundingBox;
        }

        public static Rectangle toXNARectangle(scBoundingBox boundingBox)
        {
            var rectangle = new Rectangle();
            Vector2 upperLeft = boundingBox.upperLeft;
            Vector2 lowerRight = boundingBox.lowerRight;
            Vector2 size = lowerRight - upperLeft;

            rectangle.Offset((int)upperLeft.X, (int)upperLeft.Y);
            rectangle.Width = (int)size.X;
            rectangle.Height = (int)size.Y;

            return rectangle;
        }
    }
}

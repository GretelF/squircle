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

        public Vector2 lowerLeft { get { return position - halfExtents; } }
        public Vector2 upperRight { get { return position + halfExtents; } }
        public Vector2 lowerRight { get { return position + new Vector2(+halfExtents.X, -halfExtents.Y); } }
        public Vector2 upperLeft { get { return position + new Vector2(-halfExtents.X, +halfExtents.Y); } }

        public float leftBorder { get { return position.X - halfExtents.X; } }
        public float rightBorder { get { return position.X + halfExtents.X; } }
        public float upperBorder { get { return position.Y + halfExtents.Y; } }
        public float lowerBorder { get { return position.Y - halfExtents.Y; } }



        public bool contains(scBoundingBox other)
        {
            var deltaLowerLeft = other.lowerLeft - lowerLeft;
            var deltaUpperRight = upperRight - other.upperRight;

            return deltaLowerLeft.X >= 0 && deltaLowerLeft.Y >= 0 && deltaUpperRight.X >= 0 && deltaUpperRight.Y >= 0;
        }

    }

    public static class scBoundingUtils
    {
        public static scBoundingBox union(scBoundingBox first, scBoundingBox second)
        {
            Vector2 firstLower;
            Vector2 firstUpper;
            Vector2 secondLower;
            Vector2 secondUpper;
            getBoundingVertices(first, out firstLower, out firstUpper);
            getBoundingVertices(second, out secondLower, out secondUpper);

            var newLower = new Vector2(Math.Min(firstLower.X, secondLower.X), Math.Min(firstLower.Y, secondLower.Y));
            var newUpper = new Vector2(Math.Min(firstUpper.X, secondUpper.X), Math.Min(firstUpper.Y, secondUpper.Y));
            return createFromBoundingVertices(newLower, newUpper);
        }

        public static void getBoundingVertices(scBoundingBox boundingBox, out Vector2 lower, out Vector2 upper)
        {
            lower = boundingBox.position - boundingBox.halfExtents;
            upper = boundingBox.position + boundingBox.halfExtents;
        }

        public static scBoundingBox createFromBoundingVertices(Vector2 lower, Vector2 upper)
        {
            var boundingBox = new scBoundingBox();
            boundingBox.halfExtents = (upper - lower) / 2.0f;
            boundingBox.position = lower + boundingBox.halfExtents;

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
            Vector2 lower;
            Vector2 upper;
            getBoundingVertices(boundingBox, out lower, out upper);
            Vector2 size = upper - lower;

            rectangle.Offset((int)lower.X, (int)lower.Y);
            rectangle.Width = (int)size.X;
            rectangle.Height = (int)size.Y;

            return rectangle;

        }
    }
}

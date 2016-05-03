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

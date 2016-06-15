using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public class Collision
    {
        public bool detectCircleCircle(scTransform aTransform, scCircleShape aShape, scTransform bTransform, scCircleShape bShape)
        {
            var aCenter = aTransform.position + aShape.localPosition;
            var bCenter = bTransform.position + bShape.localPosition;

            var deltaVector = aCenter - bCenter;

            return deltaVector.Length() < aShape.radius + bShape.radius;
        }

        public bool detectCircleRectangle(scTransform circleTransform, scCircleShape circle, scTransform rectangleTransform, scRectangleShape rectangle)
        {
            return false;
        }

        public bool detectRectangleRectangle(scTransform aTransform, scRectangleShape aShape, scTransform bTransform, scRectangleShape bShape)
        {
            return false;
        }

        public bool detectCircleEdge(scTransform circleTransform, scCircleShape circle, scTransform edgeTransform, scEdgeShape edge)
        {
            return false;
        }

        public bool detectRectangleEdge(scTransform rectangleTransform, scRectangleShape rectangle, scTransform edgeTransform, scEdgeShape edge)
        {
            return false;
        }

        public bool detectEdgeEdge(scTransform aTransform, scEdgeShape aShape, scTransform bTransform, scEdgeShape bShape)
        {
            return false;
        }
    }
}

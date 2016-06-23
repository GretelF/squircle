using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public static class scCollision
    {
        public static bool detectCircleCircle(scTransform aTransform, scCircleShape aShape, scTransform bTransform, scCircleShape bShape)
        {
            var aCenter = aTransform.position + aShape.localPosition;
            var bCenter = bTransform.position + bShape.localPosition;

            var deltaVector = aCenter - bCenter;

            return deltaVector.Length() < aShape.radius + bShape.radius;
        }

        public static bool detectCircleRectangle(scTransform circleTransform, scCircleShape circle, scTransform rectangleTransform, scRectangleShape rectangle)
        {
            var transformedRectangleVertices = new List<Vector2>();
            foreach (var vertex in rectangle.vertices)
            {
                transformedRectangleVertices.Add(scTransformUtils.applyTransform(rectangleTransform, vertex));
            }

            var axes = new List<Vector2>();
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[0], transformedRectangleVertices[1]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[1], transformedRectangleVertices[2]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[2], transformedRectangleVertices[3]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[3], transformedRectangleVertices[0]));
            var transformedCircleCenter = scTransformUtils.applyTransform(circleTransform, circle.localPosition);

            //get nearest edge vertex to circle center
            var distancesToVertices = new List<Vector2>();

            distancesToVertices.Add(transformedRectangleVertices[0] - transformedCircleCenter);
            distancesToVertices.Add(transformedRectangleVertices[1] - transformedCircleCenter);
            distancesToVertices.Add(transformedRectangleVertices[2] - transformedCircleCenter);
            distancesToVertices.Add(transformedRectangleVertices[3] - transformedCircleCenter);

            var circleAxis = distancesToVertices[0];
            foreach (var distance in distancesToVertices)
            {
                if (distance.Length() < circleAxis.Length())
                {
                    circleAxis = distance;
                }
            }

            circleAxis.Normalize();
            axes.Add(circleAxis);

            foreach (var axis in axes)
            {
                var lineSegmentRectangleA = scCollisionHelpers.projectCircleOnAxis(transformedCircleCenter, circle.radius, axis);
                var lineSegmentRectangleB = scCollisionHelpers.projectShapeOnAxis(transformedRectangleVertices, axis);
                if (!scCollisionHelpers.overlapsOnSameAxis(lineSegmentRectangleA, lineSegmentRectangleB))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool detectRectangleRectangle(scTransform aTransform, scRectangleShape aShape, scTransform bTransform, scRectangleShape bShape)
        {
            var transformedRectangleVerticesA = new List<Vector2>();
            var transformedRectangleVerticesB = new List<Vector2>();
            foreach (var vertex in aShape.vertices)
            {
                transformedRectangleVerticesA.Add(scTransformUtils.applyTransform(aTransform, vertex));
            }
            foreach (var vertex in bShape.vertices)
            {
                transformedRectangleVerticesB.Add(scTransformUtils.applyTransform(bTransform, vertex));
            }

            var axes = new List<Vector2>();
            // add axes of aShape
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesA[0], transformedRectangleVerticesA[1]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesA[1], transformedRectangleVerticesA[2]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesA[2], transformedRectangleVerticesA[3]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesA[3], transformedRectangleVerticesA[0]));
            // add axes of bShape
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesB[0], transformedRectangleVerticesB[1]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesB[1], transformedRectangleVerticesB[2]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesB[2], transformedRectangleVerticesB[3]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVerticesB[3], transformedRectangleVerticesB[0]));

            foreach (var axis in axes)
            {
                var lineSegmentRectangleA = scCollisionHelpers.projectShapeOnAxis(transformedRectangleVerticesA, axis);
                var lineSegmentRectangleB = scCollisionHelpers.projectShapeOnAxis(transformedRectangleVerticesB, axis);
                if (!scCollisionHelpers.overlapsOnSameAxis(lineSegmentRectangleA, lineSegmentRectangleB))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool detectCircleEdge(scTransform circleTransform, scCircleShape circle, scTransform edgeTransform, scEdgeShape edge)
        {
            var transformedEdgeVertices = new List<Vector2>();
            transformedEdgeVertices.Add(scTransformUtils.applyTransform(edgeTransform, edge.start));
            transformedEdgeVertices.Add(scTransformUtils.applyTransform(edgeTransform, edge.end));

            var transformedCircleCenter = scTransformUtils.applyTransform(circleTransform, circle.localPosition);

            //get nearest edge vertex to circle center
            var distanceToFirstVertex = transformedEdgeVertices[0] - transformedCircleCenter;
            var distanceToSecondVertex = transformedEdgeVertices[1] - transformedCircleCenter;

            var circleAxis = distanceToFirstVertex.Length() <= distanceToSecondVertex.Length() ? distanceToFirstVertex : distanceToSecondVertex;
            circleAxis.Normalize();
            var edgeAxis = scCollisionHelpers.getNormal(transformedEdgeVertices[0], transformedEdgeVertices[1]);

            {
                var lineSegmentCircle = scCollisionHelpers.projectCircleOnAxis(transformedCircleCenter, circle.radius, circleAxis);
                var lineSegmentEdge = scCollisionHelpers.projectShapeOnAxis(transformedEdgeVertices, circleAxis);
                if (!scCollisionHelpers.overlapsOnSameAxis(lineSegmentCircle, lineSegmentEdge))
                {
                    return false;
                }
            }
            {
                var lineSegmentCircle = scCollisionHelpers.projectCircleOnAxis(transformedCircleCenter, circle.radius, edgeAxis);
                var lineSegmentEdge = scCollisionHelpers.projectShapeOnAxis(transformedEdgeVertices, edgeAxis);
                if (!scCollisionHelpers.overlapsOnSameAxis(lineSegmentCircle, lineSegmentEdge))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool detectRectangleEdge(scTransform rectangleTransform, scRectangleShape rectangle, scTransform edgeTransform, scEdgeShape edge)
        {
            var transformedRectangleVertices = new List<Vector2>();
            foreach (var vertex in rectangle.vertices)
            {
                transformedRectangleVertices.Add(scTransformUtils.applyTransform(rectangleTransform, vertex));
            }

            var edgeStart = scTransformUtils.applyTransform(edgeTransform, edge.start);
            var edgeEnd = scTransformUtils.applyTransform(edgeTransform, edge.end);

            var transformedEdgeVertices = new List<Vector2>();
            transformedEdgeVertices.Add(edgeStart);
            transformedEdgeVertices.Add(edgeEnd);

            var axes = new List<Vector2>();
            axes.Add(scCollisionHelpers.getNormal(edgeStart, edgeEnd));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[0], transformedRectangleVertices[1]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[1], transformedRectangleVertices[2]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[2], transformedRectangleVertices[3]));
            axes.Add(scCollisionHelpers.getNormal(transformedRectangleVertices[3], transformedRectangleVertices[0]));

            foreach (var axis in axes)
            {
                var lineSegmentRectangle = scCollisionHelpers.projectShapeOnAxis(transformedRectangleVertices, axis);
                var lineSegmentEdge = scCollisionHelpers.projectShapeOnAxis(transformedEdgeVertices, axis);
                if (!scCollisionHelpers.overlapsOnSameAxis(lineSegmentRectangle, lineSegmentEdge))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool detectEdgeEdge(scTransform aTransform, scEdgeShape aShape, scTransform bTransform, scEdgeShape bShape)
        {
            return false;
        }
    }

    public static class scCollisionHelpers
    {
        public static Vector2 getNormal(Vector2 source, Vector2 target)
        {
            var diff = target - source;
            var result = new Vector2(-diff.Y, diff.X);
            result.Normalize();
            return result;
        }

        public static Vector2 projectOntoAxis(Vector2 axis, Vector2 point)
        {
            var projectionScale = Vector2.Dot(axis, point);
            return axis * projectionScale;
        }

        public static scLineSegment projectShapeOnAxis(IEnumerable<Vector2> vertices, Vector2 axis)
        {
            float startScale = float.MaxValue;
            float endScale = float.MinValue;
            foreach (Vector2 point in vertices)
            {
                var scale = Vector2.Dot(axis, point);
                startScale = Math.Min(startScale, scale);
                endScale = Math.Max(endScale, scale);
            }
            return new scLineSegment() { axis = axis, start = startScale, end = endScale };
        }

        public static scLineSegment projectCircleOnAxis(Vector2 center, float radius, Vector2 axis)
        {
            var projectedCenter = projectOntoAxis(axis, center);
     
            var first = Vector2.Dot(axis, projectedCenter + axis * radius);
            var second = Vector2.Dot(axis, projectedCenter - axis * radius);
            var startScale = Math.Min(first, second);
            var endScale = Math.Max(first, second);

            return new scLineSegment() { axis = axis, start = startScale, end = endScale };
        }

        public static bool overlapsOnSameAxis(scLineSegment first, scLineSegment second)
        {
            Debug.Assert(first.axis == second.axis, "LineSegments are not on the same axis!");

            return second.start > first.start && second.start < first.end ||
                   second.end > first.start && second.end < first.end ||
                   first.start > second.start && first.start < second.end ||
                   first.end > second.start && first.end < second.end;
        }
    }

    public struct scLineSegment
    {
        public Vector2 axis;
        public float start;
        public float end;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle.Physics
{
    public class scPhysicsWorldDebugRenderer
    {
        public scPhysicsWorld world;
        public IDictionary<scBody, scDebugData> bodyMap = new Dictionary<scBody, scDebugData>();
        public readonly scDebugData defaultDebugData = new scDebugData();

        /// <summary>
        /// The color used to draw the view bounds of the camera.
        /// </summary>
        public Color ViewBoundsColor = Color.Gray;

        scDebugData GetDebugDataForBody(scBody body)
        {
            if(bodyMap.ContainsKey(body))
            {
                return bodyMap[body];
            }
            return defaultDebugData;
        }

        public scDebugData GetOrCreateDebugDataForBody(scBody body)
        {
            if(bodyMap.ContainsKey(body))
                return bodyMap[body];
            var debugData = new scDebugData();
            bodyMap.Add(body, debugData);
            return debugData;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (world == null)
            {
                return;
            }

            foreach (var body in world.bodies)
            {
                var debugData = GetDebugDataForBody(body);
                foreach (var bodyPart in body.bodyParts)
                {
                    DrawShape(debugData, spriteBatch, bodyPart.shape, body.transform);
                }
            }

            foreach (var body in world.bodies)
            {
                var debugData = GetDebugDataForBody(body);

                var boundingBox = body.calculateBoundingBox();
                spriteBatch.DrawRectangle(scBoundingUtils.toXNARectangle(boundingBox), debugData.BodyBoundingBoxColor);

                // Draw rotation as a line.
                var rotated = new Vector2(debugData.BodyTransformRotationRadius, 0).Rotate(body.transform.rotation.radians);
                spriteBatch.DrawLine(body.transform.position, body.transform.position + rotated, debugData.BodyTransformRotationColor);

                // Draw position.
                var center = new Rectangle();
                center.Width = debugData.BodyTransformPositionExtents;
                center.Height = debugData.BodyTransformPositionExtents;
                center.X = (int)(body.transform.position.X - center.Width / 2.0f);
                center.Y = (int)(body.transform.position.Y - center.Height / 2.0f);
                spriteBatch.FillRectangle(center, debugData.BodyTransformPositionColor);

                // Draw linear velocity

                spriteBatch.DrawLine(body.transform.position, body.transform.position + body.linearVelocity, debugData.BodyLinearVelocityColor);
            }

            // Draw view bounds
            spriteBatch.DrawRectangle(scBoundingUtils.toXNARectangle(world.viewBounds), ViewBoundsColor);

            bodyMap.Clear();
        }

        public void DrawShape(scDebugData debugData, SpriteBatch spriteBatch, scShape shape, scTransform transform)
        {
            switch (shape.ShapeType)
            {
                case scShapeType.Circle:
                {
                    var circle = (scCircleShape)shape;
                    spriteBatch.DrawCircle(transform.position + circle.localPosition, circle.radius, debugData.CircleShapeSides, debugData.CircleShapeColor);
                    break;
                }
                case scShapeType.Rectangle:
                {
                    var rectangle = (scRectangleShape)shape;

                    var A = transform.position + rectangle.vertices[0].Rotate(transform.rotation.radians);
                    var B = transform.position + rectangle.vertices[1].Rotate(transform.rotation.radians);
                    var C = transform.position + rectangle.vertices[2].Rotate(transform.rotation.radians);
                    var D = transform.position + rectangle.vertices[3].Rotate(transform.rotation.radians);

                    spriteBatch.DrawLine(A, B, debugData.RectangleShapeColor);
                    spriteBatch.DrawLine(B, C, debugData.RectangleShapeColor);
                    spriteBatch.DrawLine(C, D, debugData.RectangleShapeColor);
                    spriteBatch.DrawLine(D, A, debugData.RectangleShapeColor);

                    break;
                }
                case scShapeType.Edge:
                {
                    var edge = (scEdgeShape)shape;
                    var start = scTransformUtils.applyTransform(transform, edge.start);
                    var end = scTransformUtils.applyTransform(transform, edge.end);
                    spriteBatch.DrawLine(start, end, debugData.EdgeShapeColor);
                    break;
                }
            }
        }
    }

    public class scDebugData
    {
        //
        // Transformation Position
        //

        /// <summary>
        /// The width and height of the filled rectangle to draw as the position of a transform.
        /// </summary>
        public int BodyTransformPositionExtents = 5;
        public Color BodyTransformPositionColor = Color.GreenYellow;

        //
        // Transformation Rotation
        //

        /// <summary>
        /// The radius of the arc to draw for the rotation.
        /// </summary>
        public float BodyTransformRotationRadius = 20.0f;

        /// <summary>
        /// The color used to draw the body rotation arc.
        /// </summary>
        public Color BodyTransformRotationColor = Color.Magenta;

        /// <summary>
        /// The color used to draw the body's linear velocity.
        /// </summary>
        public Color BodyLinearVelocityColor = Color.Cyan;


        //
        // Bounding Box
        //

        /// <summary>
        /// The color used for drawing bounding boxes.
        /// </summary>
        public Color BodyBoundingBoxColor = Color.Beige;

        //
        // Circle Shape
        //

        /// <summary>
        /// The number of lines to use when drawing a circle, i.e. the resolution of a circle. The higher this value, the smoother the circle and less performance.
        /// </summary>
        public int CircleShapeSides = 24;

        /// <summary>
        /// The color used for drawing circle shapes.
        /// </summary>
        public Color CircleShapeColor = Color.CornflowerBlue;

        //
        // Rectangle Shape
        //

        /// <summary>
        /// The color used to draw rectangle shapes.
        /// </summary>
        public Color RectangleShapeColor = Color.Coral;

        //
        // Edge Shape
        //

        /// <summary>
        /// The color used to draw edge shapes.
        /// </summary>
        public Color EdgeShapeColor = Color.Honeydew;
    }
}

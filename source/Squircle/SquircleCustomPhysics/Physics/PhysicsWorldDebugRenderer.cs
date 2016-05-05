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
        #region Debug Drawing Constants

        //
        // Transformation Position
        //

        /// <summary>
        /// The width and height of the filled rectangle to draw as the position of a transform.
        /// </summary>
        static public int BodyTransformPositionExtents = 5;
        static public Color BodyTransformPositionColor { get { return Color.GreenYellow; } }

        //
        // Transformation Rotation
        //

        /// <summary>
        /// The radius of the arc to draw for the rotation.
        /// </summary>
        static public float BodyTransformRotationArcRadius = 15.0f;
        static public float BodyTransformRotationArcAngle = 0.2f * (float)Math.PI;
        static public int   BodyTransformRotationArcSides = 90;

        /// <summary>
        /// The color used to draw the body rotation arc.
        /// </summary>
        static public Color BodyTransformRotationColor { get { return Color.Magenta; } }

        //
        // Bounding Box
        //

        /// <summary>
        /// The color used for drawing bounding boxes.
        /// </summary>
        static public Color BodyBoundingBoxColor { get { return Color.Beige; } }

        //
        // Circle Shape
        //

        /// <summary>
        /// The number of lines to use when drawing a circle, i.e. the resolution of a circle. The higher this value, the smoother the circle and less performance.
        /// </summary>
        static public int CircleShapeSides = 24;

        /// <summary>
        /// The color used for drawing circle shapes.
        /// </summary>
        static public Color CircleShapeColor { get { return Color.CornflowerBlue; } }

        //
        // Rectangle Shape
        //

        /// <summary>
        /// The color used to draw rectangle shapes.
        /// </summary>
        static public Color RectangleShapeColor { get { return Color.Coral; } }

        //
        // Edge Shape
        //

        /// <summary>
        /// The color used to draw edge shapes.
        /// </summary>
        static public Color EdgeShapeColor { get { return Color.Honeydew; } }

        #endregion Debug Drawing Constants

        public scPhysicsWorld world;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (world == null)
            {
                return;
            }

            foreach (var body in world.bodies)
            {
                foreach (var bodyPart in body.bodyParts)
                {
                    DrawShape(spriteBatch, bodyPart.shape, body.transform);
                }
            }

            foreach (var body in world.bodies)
            {
                var boundingBox = body.calculateBoundingBox();
                spriteBatch.DrawRectangle(scBoundingUtils.toXNARectangle(boundingBox), BodyBoundingBoxColor);

                // Draw rotation as a rotating arc.
                scAngle startRotation = body.transform.rotation - scAngle.FromDegrees(0.5f * BodyTransformRotationArcAngle);
                spriteBatch.DrawArc(body.transform.position, BodyTransformRotationArcRadius, BodyTransformRotationArcSides, startRotation.radians, BodyTransformRotationArcAngle, BodyTransformRotationColor);

                // Draw position.
                var center = new Rectangle();
                center.Width = BodyTransformPositionExtents;
                center.Height = BodyTransformPositionExtents;
                center.X = (int)(body.transform.position.X - center.Width/2.0f);
                center.Y = (int)(body.transform.position.Y - center.Height/2.0f);
                spriteBatch.FillRectangle(center, BodyTransformPositionColor);
            }
        }

        public void DrawShape(SpriteBatch spriteBatch, scShape shape, scTransform transform)
        {
            switch (shape.ShapeType)
            {
                case scShapeType.Circle:
                {
                    var circle = (scCircleShape)shape;
                    spriteBatch.DrawCircle(transform.position + circle.localPosition, circle.radius, CircleShapeSides, CircleShapeColor);
                    break;
                }
                case scShapeType.Rectangle:
                {
                    var rectangle = (scRectangleShape)shape;
                    spriteBatch.DrawRectangle(rectangle.asXNARectangle(), RectangleShapeColor);
                    break;
                }
                case scShapeType.Edge:
                {
                    var edge = (scEdgeShape)shape;
                    spriteBatch.DrawLine(edge.start, edge.end, EdgeShapeColor);
                    break;
                }

            }
        }
    }
}

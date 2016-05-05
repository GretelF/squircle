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
                spriteBatch.DrawRectangle(scBoundingUtils.toXNARectangle(boundingBox), Color.Beige);

                //draw position
                var center = new Rectangle();
                center.Width = 5;
                center.Height = 5;
                center.X = (int)(body.transform.position.X - center.Width/2.0f);
                center.Y = (int)(body.transform.position.Y - center.Height/2.0f);
                spriteBatch.FillRectangle(center, Color.GreenYellow);
            }
        }

        public void DrawShape(SpriteBatch spriteBatch, scShape shape, scTransform transform)
        {
            switch (shape.ShapeType)
            {
                case scShapeType.Circle:
                {
                    var circle = (scCircleShape)shape;
                    spriteBatch.DrawCircle(transform.position + circle.localPosition, circle.radius, 12, Color.CornflowerBlue);
                    break;
                }
                case scShapeType.Rectangle:
                {
                    var rectangle = (scRectangleShape)shape;
                    spriteBatch.DrawRectangle(rectangle.asXNARectangle(), Color.Coral);
                    break;
                }
                case scShapeType.Edge:
                {
                    var edge = (scEdgeShape)shape;
                    spriteBatch.DrawLine(edge.start, edge.end, Color.Honeydew);
                    break;
                }

            }
        }
    }
}

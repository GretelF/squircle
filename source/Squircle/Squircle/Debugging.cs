using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    /// <summary>
    /// Supply this to Box2D to debug-draw the physical world.
    /// </summary>
    public class PhysicsDebugDraw : DebugDraw
    {
        public SpriteBatch spriteBatch { get; set; }

        public override void DrawPolygon(ref FixedArray8<Vector2> vertices, int count, Color color)
        {
            for (int i = 1; i < count; i++)
            {
                var start = vertices[i - 1];
                var end = vertices[i];

                spriteBatch.DrawLine(start, end, color);
            }

            if (count > 1)
            {
                spriteBatch.DrawLine(vertices[0], vertices[count - 1], color);
            }
        }

        public override void DrawSolidPolygon(ref FixedArray8<Vector2> vertices, int count, Color color)
        {
            DrawPolygon(ref vertices, count, color);
        }

        public override void DrawCircle(Vector2 center, float radius, Color color)
        {
            spriteBatch.DrawCircle(center, radius, 20, color);
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
        {
            spriteBatch.DrawCircle(center, radius, 20, color);
            spriteBatch.DrawLine(center, new Vector2(center.X, center.Y - radius), color);
        }

        public override void DrawSegment(Vector2 p1, Vector2 p2, Color color)
        {
            spriteBatch.DrawLine(p1, p2, color);
        }

        public override void DrawTransform(ref Transform xf)
        {
            var drawSize = new Vector2(4, 4);
            spriteBatch.FillRectangle(xf.Position - drawSize / 2, drawSize, new Color(1.0f, 1.0f, 0.0f), xf.GetAngle());
        }
    }
}

using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    public enum DebugLevelType
    {
        None,
        Normal,
        Verbose
    }

    public class DebugLevel
    {
        public DebugLevelType Value { get; set; }

        public bool IsNone { get { return Value == DebugLevelType.None; } }
        public bool IsNormal { get { return Value == DebugLevelType.Normal; } }
        public bool IsVerbose { get { return Value == DebugLevelType.Verbose; } }

        public void SetNone() { Value = DebugLevelType.None; }
        public void SetNormal() { Value = DebugLevelType.Normal; }
        public void SetVerbose() { Value = DebugLevelType.Verbose; }

        public void CycleForward()
        {
            var max = Enum.GetNames(typeof(DebugLevelType)).Length;

            var val = (int)Value + 1;
            if (val >= max) { val = 0; }
            Value = (DebugLevelType)val;
        }

        public void CycleBackward()
        {
            var max = Enum.GetNames(typeof(DebugLevelType)).Length;

            var val = (int)Value - 1;
            if (val < 0) { val = max - 1; }
            Value = (DebugLevelType)val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// Supply this to Box2D to debug-draw the physical world.
    /// </summary>
    public class PhysicsDebugDraw : DebugDraw
    {
        public SpriteBatch spriteBatch { get; set; }
        public Level Level { get; set; }

        public override void DrawPolygon(ref FixedArray8<Vector2> vertices, int count, Color color)
        {
            for (int i = 1; i < count; i++)
            {
                var start = Level.ConvertFromBox2D(vertices[i - 1]);
                var end = Level.ConvertFromBox2D(vertices[i]);

                spriteBatch.DrawLine(start, end, color);
            }

            if (count > 1)
            {
                spriteBatch.DrawLine(Level.ConvertFromBox2D(vertices[0]), Level.ConvertFromBox2D(vertices[count - 1]), color);
            }
        }

        public override void DrawSolidPolygon(ref FixedArray8<Vector2> vertices, int count, Color color)
        {
            DrawPolygon(ref vertices, count, color);
        }

        public override void DrawCircle(Vector2 center, float radius, Color color)
        {
            spriteBatch.DrawCircle(Level.ConvertFromBox2D(center), Level.ConvertFromBox2D(radius), 20, color);
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
        {
            spriteBatch.DrawCircle(Level.ConvertFromBox2D(center), Level.ConvertFromBox2D(radius), 20, color);
            spriteBatch.DrawLine(Level.ConvertFromBox2D(center), Level.ConvertFromBox2D(new Vector2(center.X, center.Y - radius)), color);
        }

        public override void DrawSegment(Vector2 p1, Vector2 p2, Color color)
        {
            spriteBatch.DrawLine(Level.ConvertFromBox2D(p1), Level.ConvertFromBox2D(p2), color);
        }

        public override void DrawTransform(ref Transform xf)
        {
            var drawSize = new Vector2(4, 4);
            spriteBatch.FillRectangle(Level.ConvertFromBox2D(xf.Position) - drawSize / 2, drawSize, new Color(1.0f, 1.0f, 0.0f), xf.GetAngle());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Configuration;

namespace Squircle
{
    public class LevelGenerator
    {
        private List<Body> bodyList = new List<Body>();
        public ConfigFile LevelConfig { get; set; }
        protected Game Game { get; private set; }

        public LevelGenerator(Game Game)
        {
            this.Game = Game;
        }


        public List<Body> generateLevel(String level)
        {
            var LevelConfigs = ConfigFile.FromFile("Content/level/levels.cfg");

            var LevelConfig = ConfigFile.FromFile(LevelConfigs["Levels"][level]);

            //Vector2 circlePos = LevelConfig["Players"]["circle"];
            //Vector2 squarePos = LevelConfig["Players"]["square"];

            String pathToLevelFile = LevelConfig["Level"]["path"];



            var vertices = new Vector2[4];

            var startPointsStatic = new List<Vector2>();
            var startPointsDynamic = new List<Vector2>();

            var grays = new List<Vector2>();

            var map = new Bitmap(pathToLevelFile);

            for (int x = 0; x < map.Size.Width; ++x)
            {
                for (int y = 0; y < map.Size.Height; ++y)
                {
                    var pixel = map.GetPixel(x, y);
                    if (isRed(pixel))
                    {
                        startPointsStatic.Add(new Vector2(x, y));
                    }
                    if (isGreen(pixel))
                    {
                        startPointsDynamic.Add(new Vector2(x, y));
                    }
                }
            }

            foreach (var startPoint in startPointsDynamic)
            {
                vertices = GetVertices(map, startPoint);
                var bodyDef = new BodyDef();
                bodyDef.type = BodyType.Dynamic;

                bodyDef.angle = 0;
                bodyDef.position = startPoint;
                bodyDef.inertiaScale = 1.0f;

                var body = Game.World.CreateBody(bodyDef);

                var polygon = new PolygonShape();
                polygon.Set(vertices, vertices.Length);

                var fixture = new FixtureDef();
                fixture.restitution = 0.7f;
                fixture.shape = polygon;
                body.CreateFixture(fixture);

                bodyList.Add(body);
            }


            foreach (var startPoint in startPointsStatic)
            {
                vertices = GetVertices(map, startPoint);
                var bodyDef = new BodyDef();
                bodyDef.type = BodyType.Static;

                bodyDef.angle = 0;
                bodyDef.position = startPoint;

                var Body = Game.World.CreateBody(bodyDef);

                var edges = new List<EdgeShape>();

                if (vertices.Length < 2)
                {
                    throw new Exception();
                }

                for (int i = 1; i < vertices.Length; ++i)
                {
                    var edge = new EdgeShape();
                    edge.Set(vertices[i - 1], vertices[i]);
                    edges.Add(edge);
                }

                foreach (var edge in edges)
                {
                    var fixture = new FixtureDef();
                    fixture.shape = edge;
                    Body.CreateFixture(fixture);
                }

                bodyList.Add(Body);
            }

            return bodyList;
        }

        private bool isRed(System.Drawing.Color pixel)
        {
            return pixel.R > pixel.B && pixel.R > pixel.G;
        }

        private bool isGreen(System.Drawing.Color pixel)
        {
            return pixel.G > pixel.B && pixel.G > pixel.R;
        }

        private bool isGray(System.Drawing.Color pixel)
        {
            return !isWhite(pixel) && !isBlack(pixel) && pixel.R == pixel.B && pixel.R == pixel.G;
        }

        private bool isBlack(System.Drawing.Color pixel)
        {
            return pixel.R == 0 && pixel.B == 0 && pixel.G == 0;
        }

        private bool isWhite(System.Drawing.Color pixel)
        {
            return pixel.R == 255 && pixel.G == 255 && pixel.B == 255;
        }

        private System.Drawing.Color getPixel(Bitmap map, Vector2 pos)
        {
            var x = (int)pos.X;
            var y = (int)pos.Y;
            return map.GetPixel(x, y);
        }

        private Vector2[] GetVertices(Bitmap map, Vector2 start)
        {
            var vertices = new List<Vector2>();
            var visited = new List<Vector2>();

            vertices.Add(new Vector2(0.0f, 0.0f));              // local origin of the body.
            var current = start;

            var offsets = new Vector2[]
            {
                new Vector2( 0, -1),
                new Vector2( 1, -1),
                new Vector2( 1,  0),
                new Vector2( 1,  1),
                new Vector2( 0,  1),
                new Vector2(-1,  1),
                new Vector2(-1,  0),
                new Vector2(-1, -1),
            };

            do
            {
                var next = current;
                foreach (var offset in offsets)
                {
                    var neighbor = current + offset;
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }
                    if (isGray(getPixel(map, neighbor)))
                    {
                        next = neighbor;
                        visited.Add(neighbor);
                        break;
                    }
                    if (isBlack(getPixel(map, neighbor)))
                    {
                        next = neighbor;
                        vertices.Add(next - start);                 // subtract start, to transform to local space. 
                        visited.Add(neighbor);
                        break;
                    }
                    if (isRed(getPixel(map, neighbor)))
                    {
                        next = neighbor;
                        visited.Add(neighbor);
                        break;
                    }
                }
                if (next == current)
                {
                    break;          // nothing was found.
                }
                current = next;
            }
            while (!isRed(getPixel(map, current)));
            return vertices.ToArray();
        }


    }
}

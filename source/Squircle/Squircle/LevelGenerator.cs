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
    public enum LevelElementType
    {
        Wall,
        Ground
    }

    public class LevelElementInfo
    {
        public LevelElementType type { get; set; }
    }

    public class LevelGenerator
    {
        private List<Body> bodyList = new List<Body>();
        public Level level { get; set; }

        private struct EdgeInfo
        {
            public EdgeShape shape;
            public bool isVertical;
        }

        public LevelGenerator(Level level)
        {
            this.level = level;
        }

        public List<Body> generateLevel()
        {

            String pathToCollisionFile = level.levelConfig["Level"]["collision"];

            var vertices = new Vector2[4];

            var startPointsStatic = new List<Vector2>();
            var startPointsDynamic = new List<Vector2>();

            var grays = new List<Vector2>();

            var map = new Bitmap(pathToCollisionFile);

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

                var body = level.World.CreateBody(bodyDef);

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

                var body = level.World.CreateBody(bodyDef);

                var edges = new List<EdgeInfo>();

                if (vertices.Length < 2)
                {
                    throw new Exception();
                }

                for (int i = 1; i < vertices.Length; ++i)
                {
                    var vertexA = vertices[i - 1];
                    var vertexB = vertices[i];

                    var edgeInfo = new EdgeInfo();
                    edgeInfo.shape = new EdgeShape();
                    edgeInfo.isVertical = vertexA.X == vertexB.X;
                    edgeInfo.shape.Set(vertexA, vertexB);
                    edges.Add(edgeInfo);
                    
                }

                foreach (var edge in edges)
                {
                    var fixture = new FixtureDef();
                    var elementInfo = new LevelElementInfo();
                    fixture.shape = edge.shape;
                    if (edge.isVertical)
                    {
                        fixture.friction = 0.0f;
                        elementInfo.type = LevelElementType.Wall;
                    }
                    else
                    {
                        fixture.friction = 4.0f;
                        elementInfo.type = LevelElementType.Ground;
                    }
                    fixture.userData = elementInfo;
                    body.CreateFixture(fixture);
                }

                bodyList.Add(body);
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

        private List<Vector2> GetValidNeighbors(Vector2 vertex, Vector2[] offsets, Bitmap map)
        {
            var neighbors = new List<Vector2>();

            var width = map.Size.Width;
            var heigth = map.Size.Height;
            foreach (var offset in offsets)
            {
                var possibleNeighbor = vertex + offset;
                if (possibleNeighbor.X >= width || possibleNeighbor.X < 0 || possibleNeighbor.Y >= heigth || possibleNeighbor.Y < 0)
                {
                    continue;
                }
                else
                {
                    neighbors.Add(possibleNeighbor);
                }
            }
            return neighbors;
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

                var neighbors = GetValidNeighbors(next, offsets, map);

                foreach (var neighbor in neighbors)
                {
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

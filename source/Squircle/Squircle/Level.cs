using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Box2D.XNA;
//using C3.XNA;
using System.Drawing;
using Configuration;

namespace Squircle
{
    public class Level
    {
        private Game game;

        public World World { get; set; }
        LevelGenerator LevelGenerator;
        List<Body> bodyList;
        Square square { get; set; }
        public ConfigFile levelConfig { get; private set; }


        public Level(Game game)
        {
            this.game = game;
        }

        public void Initialize(ConfigOption option)
        {
            levelConfig = ConfigFile.FromFile(option.Value);
            World = new Box2D.XNA.World(new Vector2(0.0f, 100.0f), false);
            LevelGenerator = new LevelGenerator(this);
            bodyList = LevelGenerator.generateLevel();

            square = new Square(game, this);
            square.Pos = levelConfig["Players"]["square"].AsVector2();
            square.Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            square.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            square.PrePhysicsUpdate(gameTime);
            World.Step(deltaTime, 20, 10);
            square.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            //spriteBatch.DrawCircle(body.Position, 50.0f, 50, Microsoft.Xna.Framework.Color.Red);

            var body = World.GetBodyList();
            while (body != null)
            {
                var fixture = body.GetFixtureList();
                while (fixture != null)
                {
                    var shape = fixture.GetShape();
                    var position = body.GetPosition();
                    var rotation = body.GetAngle();
                    switch (shape.ShapeType)
                    {
                        case ShapeType.Circle:
                            break;
                        case ShapeType.Edge:
                            {
                                var edge = (EdgeShape)shape;

                                var from = position + edge._vertex1;
                                var to = position + edge._vertex2;
                                spriteBatch.DrawLine(from, to, Microsoft.Xna.Framework.Color.Red);
                            }
                            break;
                        case ShapeType.Polygon:
                            {
                                var polygon = (PolygonShape)shape;
                                var vertices = new List<Vector2>();
                                for (int i = 1; i < polygon.GetVertexCount(); i++)
                                {
                                    var from = position + polygon.GetVertex(i - 1).Rotate(rotation);
                                    var to = position + polygon.GetVertex(i).Rotate(rotation);
                                    spriteBatch.DrawLine(from, to, Microsoft.Xna.Framework.Color.Lime);
                                }
                                var startPoint = position + polygon.GetVertex(0).Rotate(rotation);
                                var endPoint = position + polygon.GetVertex(polygon.GetVertexCount() - 1).Rotate(rotation);
                                spriteBatch.DrawLine(startPoint, endPoint, Microsoft.Xna.Framework.Color.Lime);
                            }
                            break;
                        case ShapeType.Loop:
                            break;
                        default:
                            break;
                    }
                    fixture = fixture.GetNext();
                }
                body = body.GetNext();
            }

            spriteBatch.Draw(square.Texture, square.Pos, Microsoft.Xna.Framework.Color.White);

            spriteBatch.End();
        }
    }
}

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
    public enum FixtureType
    {
        A, 
        B,
    }
    public struct ContactInfo
    {
        public Contact contact { get; set; }
        public FixtureType fixtureType { get; set; }
    }


    public class Level : IContactListener
    {
        private Game game;

        public World World { get; set; }
        LevelGenerator LevelGenerator;
        List<Body> bodyList;
        Texture2D background;
        Square square { get; set; }
        Circle circle { get; set; }
        public ConfigFile levelConfig { get; private set; }
        public Camera2D camera { get; set; }



        public Level(Game game)
        {
            this.game = game;
        }

        public void Initialize(ConfigOption option)
        {
            levelConfig = ConfigFile.FromFile(option.Value);
            World = new Box2D.XNA.World(new Vector2(0.0f, 100.0f), false);
            World.ContactListener = this;
            LevelGenerator = new LevelGenerator(this);
            bodyList = LevelGenerator.generateLevel();

            square = new Square(game, this);
            square.Pos = levelConfig["Players"]["square"].AsVector2();
            square.Initialize();

            circle = new Circle(game, this);
            circle.Pos = levelConfig["Players"]["circle"].AsVector2();
            circle.Initialize();

            camera = new Camera2D(game);
            camera.Initialize();
            camera.Focus = new PhantomObject(game);
            camera.Focus.Pos = new Vector2(0.0f,255.0f);
            camera.ViewBounds = new Microsoft.Xna.Framework.Rectangle(0, 0, 1977, 400);
        }

        public void LoadContent(ContentManager content)
        {
            square.LoadContent(content);
            circle.LoadContent(content);

            background = content.Load < Texture2D > (levelConfig["Level"]["background"]);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            square.PrePhysicsUpdate(gameTime);
            circle.PrePhysicsUpdate(gameTime);
            World.Step(deltaTime, 20, 10);
            square.Update(gameTime);
            circle.Update(gameTime);

            var center = circle.Pos + (square.Pos - circle.Pos) / 2;                // calculate center between circle and square.
            camera.Focus.Pos = new Vector2(center.X , camera.Focus.Pos.Y);


            camera.Update(gameTime);
        }

        public void DrawPhysicalObjects(SpriteBatch spriteBatch)
        {
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
                        case ShapeType.Circle:
                            {
                                var circleShape = (CircleShape)shape;
                                spriteBatch.DrawCircle(position + circleShape._p, circleShape._radius, 20, Microsoft.Xna.Framework.Color.White);
                            }
                            break;
                        default:
                            break;
                    }
                    fixture = fixture.GetNext();
                }
                body = body.GetNext();
            }

            var contact = World.GetContactList();
            var drawingSize = new Vector2(3.0f, 3.0f);
            while (contact != null)
            {
                Manifold manifold;
                contact.GetManifold(out manifold);

                WorldManifold worldManifold;
                contact.GetWorldManifold(out worldManifold);

                for (int i = 0; i < manifold._pointCount; i++)
                {
                    var point = worldManifold._points[i];
                    spriteBatch.FillRectangle(point, drawingSize, Microsoft.Xna.Framework.Color.Pink);
                }

                contact = contact.GetNext();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(background, new Vector2(0.0f, 0.0f), Microsoft.Xna.Framework.Color.White);
            square.Draw(spriteBatch);
            circle.Draw(spriteBatch);

            spriteBatch.DrawCircle(camera.Position, 5, 5, Microsoft.Xna.Framework.Color.Coral);

            //DrawPhysicalObjects(spriteBatch);
        }

        #region IContactListener interface

        public void BeginContact(Contact contact)
        {
            var contactInfo = new ContactInfo();

            while (contact != null)
            {
                contactInfo.contact = contact;

                var fixtureA = contact.GetFixtureA();
                var go = fixtureA.GetBody().GetUserData() as GameObject;
                if (go != null)
                {
                    contactInfo.fixtureType = FixtureType.A;
                    go.BeginContact(contactInfo);
                }
                var fixtureB = contact.GetFixtureB();
                go = fixtureB.GetBody().GetUserData() as GameObject;
                if (go != null)
                {
                    contactInfo.fixtureType = FixtureType.B;
                    go.BeginContact(contactInfo);
                }
                contact = contact.GetNext();
            }
        }

        public void EndContact(Contact contact)
        {
            var contactInfo = new ContactInfo();

            while (contact != null)
            {
                contactInfo.contact = contact;

                var fixtureA = contact.GetFixtureA();
                var go = fixtureA.GetBody().GetUserData() as GameObject;
                if (go != null)
                {
                    contactInfo.fixtureType = FixtureType.A;
                    go.EndContact(contactInfo);
                }
                var fixtureB = contact.GetFixtureB();
                go = fixtureB.GetBody().GetUserData() as GameObject;
                if (go != null)
                {
                    contactInfo.fixtureType = FixtureType.B;
                    go.EndContact(contactInfo);
                }
                contact = contact.GetNext();
            }
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
        }

        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {
        }

        #endregion
    }
}

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
using Configuration;
using System.Text;

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
        public GameObject other { get; set; }
    }

    public class Level : IContactListener
    {
        private Game game;

        public World World { get; set; }
        LevelGenerator LevelGenerator;
        List<Body> bodyList;
        Texture2D background;
        public Square square { get; set; }
        public Circle circle { get; set; }
        public ConfigFile levelConfig { get; private set; }
        public Camera2D camera { get; set; }
        public IList<GameObject> GameObjects { get; set; }
        public IList<GameObject> ToUpdate { get; set; }
        public IList<GameObject> ToDraw { get; set; }
        public Body playerBounds { get; set; }

        public Level(Game game)
        {
            this.game = game;
            GameObjects = new List<GameObject>();
            ToUpdate = new List<GameObject>();
            ToDraw = new List<GameObject>();
        }

        public void Initialize(ConfigOption option)
        {
            levelConfig = ConfigFile.FromFile(option.Value);
            World = new Box2D.XNA.World(new Vector2(0.0f, 100.0f), false);
            World.ContactListener = this;
            World.DebugDraw = game.PhysicsDebugDrawer;
            LevelGenerator = new LevelGenerator(this);
            bodyList = LevelGenerator.generateLevel();

            var viewport = game.GraphicsDevice.Viewport;

            square = new Square(game, this);
            square.Pos = levelConfig["Players"]["square"].AsVector2();
            square.Initialize();

            circle = new Circle(game, this);
            circle.Pos = levelConfig["Players"]["circle"].AsVector2();
            circle.Initialize();

            camera = new Camera2D(game);
            camera.Initialize();
            camera.Focus = new PhantomObject(game);
            camera.Position = levelConfig["Camera"]["startPos"].AsVector2();
            camera.ViewBounds = levelConfig["Camera"]["viewBounds"].AsRectangle();
            camera.MaxMoveSpeed = levelConfig["Camera"]["maxMoveSpeed"];

            playerBounds = CreatePhysicalViewBounds();

            var gameObjectsConfig = ConfigFile.FromFile(levelConfig.GlobalSection["objects"]);

            foreach (var section in gameObjectsConfig.Sections)
            {
                if (section.Value == gameObjectsConfig.GlobalSection)
                {
                    // Skip the global section because it can not contain any useful info in this case.
                    continue;
                }
                var go = GameObject.Create(game, section.Key, section.Value);
                GameObjects.Add(go);
            }

            if (!levelConfig.Sections.ContainsKey("Debug"))
            {
                return;
            }

            var debugSection = levelConfig["Debug"];

            if (debugSection.Options.ContainsKey("drawPhysics"))
            {
                game.drawPhysics = debugSection.Options["drawPhysics"].AsBool();
            }

            if (debugSection.Options.ContainsKey("drawVisualHelpers"))
            {
                game.drawVisualHelpers = debugSection.Options["drawVisualHelpers"].AsBool();
            }

            if (debugSection.Options.ContainsKey("drawDebugData"))
            {
                game.drawDebugData = debugSection.Options["drawDebugData"].AsBool();
            }

            if (debugSection.Options.ContainsKey("drawMoreDebugData"))
            {
                game.drawMoreDebugData = debugSection.Options["drawMoreDebugData"].AsBool();
            }
        }

        public void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>(levelConfig.GlobalSection["background"]);

            square.LoadContent(content);
            circle.LoadContent(content);

            foreach (var go in GameObjects)
            {
                go.LoadContent(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            square.PrePhysicsUpdate(gameTime);
            circle.PrePhysicsUpdate(gameTime);

            foreach (var go in GameObjects)
            {
                go.PrePhysicsUpdate(gameTime);
            }

            World.Step(deltaTime, 20, 10);

            square.Update(gameTime);
            circle.Update(gameTime);

            foreach (var go in GameObjects)
            {
                go.Update(gameTime);
            }

            var center = circle.Pos + (square.Pos - circle.Pos) / 2;                // calculate center between circle and square.
            camera.Focus.Pos = new Vector2(center.X , center.Y);

            camera.Update(gameTime);

            playerBounds.Position = camera.Position;
        }

        public void DrawPhysicalContacts(SpriteBatch spriteBatch)
        {
            var numContacts = 0;
            var contact = World.GetContactList();
            var drawingSize = new Vector2(4.0f, 4.0f);
            while (contact != null)
            {
                ++numContacts;

                Manifold manifold;
                contact.GetManifold(out manifold);

                WorldManifold worldManifold;
                contact.GetWorldManifold(out worldManifold);

                for (int i = 0; i < manifold._pointCount; i++)
                {
                    var point = worldManifold._points[i];
                    spriteBatch.FillRectangle(point - drawingSize / 2, drawingSize, Microsoft.Xna.Framework.Color.Lime);
                }

                contact = contact.GetNext();
            }

            if (game.drawMoreDebugData)
            {
                game.DrawOnScreen("Physical contacts: " + numContacts.ToString());
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(background, new Vector2(0.0f, 0.0f), Microsoft.Xna.Framework.Color.White);

            foreach (var go in GameObjects)
            {
                go.Draw(spriteBatch);
            }

            square.Draw(spriteBatch);
            circle.Draw(spriteBatch);

            if (game.drawPhysics)
            {
                World.DrawDebugData();
                DrawPhysicalContacts(spriteBatch);
            }
        }

        private Body CreatePhysicalViewBounds()
        {
            var bodyDef = new BodyDef();
            bodyDef.type = BodyType.Static;
            var body = World.CreateBody(bodyDef);
            var edges = new EdgeShape[]{new EdgeShape(), new EdgeShape(), new EdgeShape(), new EdgeShape()};

            var viewport = game.GraphicsDevice.Viewport;

            var X = viewport.Width / 2;
            var Y = viewport.Height / 2;
            edges[0].Set(new Vector2(-X, -Y), new Vector2(+X, -Y));
            edges[1].Set(new Vector2(+X, -Y), new Vector2(+X, +Y));
            edges[2].Set(new Vector2(+X, +Y), new Vector2(-X, +Y));
            edges[3].Set(new Vector2(-X, +Y), new Vector2(-X, -Y));

            foreach (var edge in edges)
            {
                var fixtureDef = new FixtureDef();
                fixtureDef.shape = edge;
                fixtureDef.friction = 0.0f;
                body.CreateFixture(fixtureDef);
            }

            return body;
        }


        #region IContactListener interface

        public void BeginContact(Contact contact)
        {
            while (contact != null)
            {
                var lhsInfo = new ContactInfo();
                var rhsInfo = new ContactInfo();

                lhsInfo.contact = contact;
                rhsInfo.contact = contact;

                lhsInfo.fixtureType = FixtureType.A;
                rhsInfo.fixtureType = FixtureType.B;

                var lhsGo = contact.GetFixtureA().GetBody().GetUserData() as GameObject;
                var rhsGo = contact.GetFixtureB().GetBody().GetUserData() as GameObject;

                lhsInfo.other = rhsGo;
                rhsInfo.other = lhsGo;

                if (lhsGo != null) { lhsGo.BeginContact(lhsInfo); }
                if (rhsGo != null) { rhsGo.BeginContact(rhsInfo); }

                contact = contact.GetNext();
            }
        }

        public void EndContact(Contact contact)
        {
            while (contact != null)
            {
                var lhsInfo = new ContactInfo();
                var rhsInfo = new ContactInfo();

                lhsInfo.contact = contact;
                rhsInfo.contact = contact;

                lhsInfo.fixtureType = FixtureType.A;
                rhsInfo.fixtureType = FixtureType.B;

                var lhsGo = contact.GetFixtureA().GetBody().GetUserData() as GameObject;
                var rhsGo = contact.GetFixtureB().GetBody().GetUserData() as GameObject;

                lhsInfo.other = rhsGo;
                rhsInfo.other = lhsGo;

                if (lhsGo != null) { lhsGo.EndContact(lhsInfo); }
                if (rhsGo != null) { rhsGo.EndContact(rhsInfo); }

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

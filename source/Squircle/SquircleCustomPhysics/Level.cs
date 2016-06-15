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
using Configuration;
using System.Text;
using Squircle.Physics;

namespace Squircle
{
    public class Level
    {
        private Game game;

        public string Name { get; set; }
        public scPhysicsWorld World { get; set; }
        public scPhysicsWorldDebugRenderer physicsWorldDebugRenderer;
        LevelGenerator LevelGenerator;
        Texture2D background;
        public ConfigFile levelConfig { get; private set; }
        public Camera2D camera { get; set; }
        public Square square;
        public Circle circle;
        public IList<GameObject> GameObjects { get; set; }
        public UserInterface.MainWindow Menu { get; set; }
        public float PhysicsScale { get; set; }
        public float GroundFriction { get; set; }

        public string AmbientMusicCue { get; set; }

        public Level(Game game)
        {
            this.game = game;
            GameObjects = new List<GameObject>();
            PhysicsScale = 1.0f;
            Menu = new UserInterface.MainWindow(game);
        }

        public void Initialize(ConfigOption option)
        {
            levelConfig = ConfigFile.FromFile(option.Value);

            {
                var physicsSection = levelConfig["Physics"];

                PhysicsScale = physicsSection["scale"];
                GroundFriction = physicsSection["groundFriction"];
                World = new scPhysicsWorld();
                World.game = game;

                physicsSection.IfOptionExists("gravity", o => World.gravity = o.AsVector2());
                //                physicsSection["gravity"].AsVector2(), physicsSection["doSleep"].AsBool()
                //                World.ContinuousPhysics = physicsSection["continuousPhysics"].AsBool();

                physicsWorldDebugRenderer = new scPhysicsWorldDebugRenderer();
                physicsWorldDebugRenderer.world = World;
            }

            LevelGenerator = new LevelGenerator(this);
            //            bodyList = LevelGenerator.generateLevel();

            var viewport = game.GraphicsDevice.Viewport;

            InitializePlayers();

            camera = new Camera2D(game);
            camera.Initialize();
            camera.Position = levelConfig["Camera"]["startPos"].AsVector2();
            camera.ViewBounds = levelConfig["Camera"]["viewBounds"].AsRectangle();
            camera.MaxMoveSpeed = levelConfig["Camera"]["maxMoveSpeed"];

            CreatePhysicalViewBounds();

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

            Menu.InitializeFromConfigFile(levelConfig.GlobalSection["userInterface"].AsConfigFile());

            levelConfig.IfSectionExists("Audio",
                section =>
                {
                    section.IfOptionExists("ambientCue", opt => AmbientMusicCue = opt);
                });

            if (!levelConfig.Sections.ContainsKey("Debug"))
            {
                return;
            }

            var debugSection = levelConfig["Debug"];

            debugSection.IfOptionExists("drawPhysics", opt => game.drawPhysics = opt.AsBool());
            debugSection.IfOptionExists("drawVisualHelpers", opt =>
                {
                    if (opt.AsBool())
                        game.drawVisualHelpers.SetNormal();
                    else
                        game.drawVisualHelpers.SetNone();
                });
            debugSection.IfOptionExists("drawDebugData", opt =>
            {
                if (opt.AsBool())
                    game.drawDebugData.SetNormal();
                else
                    game.drawDebugData.SetNone();
            });

            {
                var circleshape = new scCircleShape();
                circleshape.radius = 75;
                var bodyPartDescription = new scBodyPartDescription();
                bodyPartDescription.shape = circleshape;
                var bodyDescription = new scBodyDescription();
                bodyDescription.bodyType = scBodyType.Static;
                bodyDescription.transform.position = new Vector2(-150, 0);
                var bodyPartDescriptions = new List<scBodyPartDescription>();
                bodyPartDescriptions.Add(bodyPartDescription);
                var body = World.createBody(bodyDescription, bodyPartDescriptions);
            }

            {
                var rectangleshape = scRectangleShape.fromLocalPositionAndHalfExtents(new Vector2(0, 0), new Vector2(75, 30));
                var bodyPartDescription = new scBodyPartDescription();
                bodyPartDescription.shape = rectangleshape;
                var bodyDescription = new scBodyDescription();
                bodyDescription.bodyType = scBodyType.Static;
                bodyDescription.transform.position = new Vector2(50, 150);
                var bodyPartDescriptions = new List<scBodyPartDescription>();
                bodyPartDescriptions.Add(bodyPartDescription);
                var body = World.createBody(bodyDescription, bodyPartDescriptions);
            }

        }

        private void InitializePlayers()
        {
            var playersSection = levelConfig["Players"];

            foreach (var player in playersSection.Options)
            {
                var playerConfig = ConfigFile.FromFile(player.Value);
                var section = playerConfig.GlobalSection;
                var playerName = (string)player.Key;

                if (!section.Options.ContainsKey("type"))
                {
                    section.Options.Add("type", playerName);
                }

                var go = GameObject.Create(game, playerName, section);
                GameObjects.Add(go);
            }

            square = (Square)GetGameObject("square");
            circle = (Circle)GetGameObject("circle");
        }

        public void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>(levelConfig.GlobalSection["background"]);

            if (game.Audio != null)
            {
                if (AmbientMusicCue != null)
                {
                    game.Audio.PlayCueAndStopAllOthers(AmbientMusicCue);
                }
                else
                {
                    game.Audio.StopAllCues();
                }
            }

            foreach (var go in GameObjects)
            {
                go.LoadContent(content);
            }

            Menu.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var go in GameObjects)
            {
                go.PrePhysicsUpdate(gameTime);
            }

            World.simulate(gameTime, square, circle);

            foreach (var go in GameObjects)
            {
                go.Update(gameTime);
            }

            UpdateCameraFocus();

            camera.Update(gameTime);

            World.viewBounds.position = camera.Position;

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var body in World.bodies)
            {
                var rotationAmountPerSecond = scAngle.FromDegrees(45);

                if (game.InputHandler.IsDown(Keys.Left))
                {
                    body.transform.rotation -= rotationAmountPerSecond * dt;
                }
                if (game.InputHandler.IsDown(Keys.Right))
                {
                    body.transform.rotation += rotationAmountPerSecond * dt;
                }
            }

        }

        private void UpdateCameraFocus()
        {
            if (square != null && circle != null)
            {
                var center = circle.Pos + (square.Pos - circle.Pos) / 2;                // calculate center between circle and square.
                camera.Focus = center;
            }
            else if (square != null)
            {
                camera.Focus = square.Pos;
            }
            else if (circle != null)
            {
                camera.Focus = circle.Pos;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(background, new Vector2(0.0f, 0.0f), Microsoft.Xna.Framework.Color.White);

            foreach (var go in GameObjects.OrderBy(go => go.DrawOrder))
            {
                go.Draw(spriteBatch);
            }

            if (game.drawPhysics)
            {
                physicsWorldDebugRenderer.Draw(spriteBatch);
            }
        }

        public void DrawUserInterface(SpriteBatch spriteBatch)
        {
            if (game.GameState.IsInMenu)
            {
                Menu.Draw(spriteBatch);
            }
        }

        private void CreatePhysicalViewBounds()
        {
            var viewport = game.GraphicsDevice.Viewport;
            var halfwidth = viewport.Width / 2;
            var halfheight = viewport.Height / 2;

            World.viewBounds = scBoundingUtils.createFromPositionAndHalfExtents(camera.Position, new Vector2(halfwidth, halfheight));
        }

        public GameObject GetGameObject(string name)
        {
            try
            { return GameObjects.First(go => go.Name == name); }
            catch (System.InvalidOperationException)
            {
                return null;
            }

        }
    }
}

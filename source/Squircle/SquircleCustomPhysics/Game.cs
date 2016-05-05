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
using Configuration;
using System.Text;
using System.Diagnostics;

using Squircle.Physics;

namespace Squircle
{

    /// <summary>
    /// Declare some debug infos about a class or a specific property of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DebugData : System.Attribute
    {
        public bool Ignore { get; set; }

        public DebugData()
        {
            Ignore = false;
        }
    }

    public enum GameStateType
    {
        Invalid,

        Loading,
        Menu,
        Running,
    }

    public class GameState
    {
        public GameStateType PreviousValue { get; private set; }
        public GameStateType Value { get; set; }

        public bool WasLoading { get { return PreviousValue == GameStateType.Loading; } }
        public bool WasInMenu { get { return PreviousValue == GameStateType.Menu; } }
        public bool WasRunning { get { return PreviousValue == GameStateType.Running; } }

        public bool IsLoading { get { return Value == GameStateType.Loading; } }
        public bool IsInMenu { get { return Value == GameStateType.Menu; } }
        public bool IsRunning { get { return Value == GameStateType.Running; } }

        public GameState()
        {
            PreviousValue = GameStateType.Invalid;
            Value = GameStateType.Menu;
        }

        public void SetLoading() { PreviousValue = Value; Value = GameStateType.Loading; }
        public void SetInMenu() { PreviousValue = Value; Value = GameStateType.Menu; }
        public void SetRunning() { PreviousValue = Value; Value = GameStateType.Running; }

        public void ToggleRunningAndInMenu()
        {
            Debug.Assert(IsRunning || IsInMenu,
                "Cannot toggle between Running and InMenu if we are in another state.");

            if (IsRunning)
            {
                SetInMenu();
            }
            else
            {
                SetRunning();
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Level level;
        public ConfigFile gameConfig { get; set; }
        public bool drawPhysics { get; set; }                   ///< Draws the physical world.
        public DebugLevel drawDebugData { get; set; }           ///< Draws textual game object data above them.
        public DebugLevel drawVisualHelpers { get; set; }       ///< Draws some visual helpers for game objects.
        public SpriteFont debugFont { get; set; }
        public EventSystem Events { get; set; }
        public InputHandler InputHandler { get; set; }
        public GameState GameState { get; set; }

        public PhysicsDebugDraw PhysicsDebugDrawer { get; set; }

        public StringBuilder DebugInfo { get; set; }

        public bool LoadingScreenDrawn { get; set; }

        public Event ToggleRunningAndInMenuEvent { get; set; }

        public AudioManager Audio { get; set; }

        public Vector2 ViewportDimensions { get; set; }

        public scPhysicsWorld physicsWorld;
        public scPhysicsWorldDebugRenderer physicsWorldDebugRenderer;
        
        public Vector2 DebugCameraPosition;
        public bool UseDebugCamera = true;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            drawPhysics = false;
            drawVisualHelpers = new DebugLevel();
            drawDebugData = new DebugLevel();

            GameState = new GameState();
            GameState.SetLoading();

            ViewportDimensions = new Vector2(800, 480);
            DebugCameraPosition = -0.5f * ViewportDimensions;

            level = new Level(this);
            level.Name = "level_01";
            level.Menu.InitialWindowName = "mainWindow";

            gameConfig = ConfigFile.FromFile("Content/level/game_experiments.cfg");
            gameConfig.IfSectionExists("Audio", section =>
            {
                Audio = new AudioManager(this);
                Audio.Initialize(section);
            });

            physicsWorld = new scPhysicsWorld();
            physicsWorldDebugRenderer = new scPhysicsWorldDebugRenderer();
            physicsWorldDebugRenderer.world = physicsWorld;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ViewportDimensions = new Vector2(GraphicsDevice.Viewport.Width,
                                             GraphicsDevice.Viewport.Height);

            DebugInfo = new StringBuilder();

            InputHandler = new InputHandler();

            Events = new EventSystem();
            Events["endLevel"].addListener(onEndLevel);
            Events["exit"].addListener(onExit);
            Events["ui.show"].addListener(OnUIShow);
            Events["ui.close"].addListener(OnUIClose);

            PhysicsDebugDrawer = new PhysicsDebugDraw();
            PhysicsDebugDrawer.Level = level;

            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.AABB);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.CenterOfMass);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.Joint);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.Pair);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.Shape);

            level.Initialize(gameConfig["Levels"][level.Name]);

            base.Initialize();

            GameState.SetInMenu();

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
                var body = physicsWorld.createBody(bodyDescription, bodyPartDescriptions);
            }

            {
                var rectangleshape = scRectangleShape.fromLocalPositionAndHalfExtents(new Vector2(0, 0), new Vector2(75, 30));
                var bodyPartDescription = new scBodyPartDescription();
                bodyPartDescription.shape = rectangleshape;
                var bodyDescription = new scBodyDescription();
                bodyDescription.bodyType = scBodyType.Static;
                bodyDescription.transform.position = new Vector2(150, 0);
                var bodyPartDescriptions = new List<scBodyPartDescription>();
                bodyPartDescriptions.Add(bodyPartDescription);
                var body = physicsWorld.createBody(bodyDescription, bodyPartDescriptions);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            PhysicsDebugDrawer.spriteBatch = spriteBatch;
            debugFont = Content.Load<SpriteFont>(gameConfig.GlobalSection["debugFont"]);

            if (Audio != null)
                Audio.LoadContent(Content);

            level.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (Audio != null)
            {
                Audio.CleanUp();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        protected override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);

            InputHandler.Update(gameTime);

            if (Audio != null)
                Audio.Update(gameTime);

            if (GameState.IsLoading)
            {
                if (LoadingScreenDrawn)
                {
                    EndLoadLevel();
                }
                return;
            }
            else if (GameState.IsInMenu)
            {
                level.Menu.Update(gameTime);
                return;
            }

            if (InputHandler.WasTriggered(Keys.Escape) || InputHandler.WasTriggered(Buttons.Start))
            {
                // Show the main menu.
                Events["ui.show"].trigger("mainWindow");
            }

            if (InputHandler.WasTriggered(Keys.R) || InputHandler.WasTriggered(Buttons.Back))
            {
                Events["endLevel"].trigger(level.Name);
                return;
            }

            if (InputHandler.WasTriggered(Keys.F9))
            {
                drawPhysics = !drawPhysics;
            }

            if (InputHandler.WasTriggered(Keys.F10))
            {
                drawDebugData.CycleForward();
            }

            if (InputHandler.WasTriggered(Keys.F11))
            {
                drawVisualHelpers.CycleForward();
            }

            foreach (var body in physicsWorld.bodies)
            {
                var rotationAmountPerSecond = scAngle.FromDegrees(45);

                if(InputHandler.IsDown(Keys.Left))
                {
                    body.transform.rotation -= rotationAmountPerSecond * dt;
                }
                if(InputHandler.IsDown(Keys.Right))
                {
                    body.transform.rotation += rotationAmountPerSecond * dt;
                }
            }

            level.Update(gameTime);

            if(UseDebugCamera)
            {
                var speed = 100.0f;
                if(InputHandler.IsDown(Keys.W))
                {
                    DebugCameraPosition.Y -= dt * speed;
                }
                if(InputHandler.IsDown(Keys.A))
                {
                    DebugCameraPosition.X -= dt * speed;
                }
                if(InputHandler.IsDown(Keys.S))
                {
                    DebugCameraPosition.Y += dt * speed;
                }
                if(InputHandler.IsDown(Keys.D))
                {
                    DebugCameraPosition.X += dt * speed;
                }
            }

            if (InputHandler.IsDown(Keys.Add))
            {
                level.camera.Scale += 0.01f;
            }

            if (InputHandler.IsDown(Keys.Subtract))
            {
                level.camera.Scale -= 0.01f;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (GameState.IsLoading)
            {
                spriteBatch.Begin();
                var toDraw = "Loading...";
                var measurements = debugFont.MeasureString(toDraw);
                spriteBatch.DrawString(debugFont,
                    toDraw,
                    new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2) - measurements / 2,
                    Color.White);
                spriteBatch.End();
                LoadingScreenDrawn = true;
                return;
            }

            Matrix transform;
            if (UseDebugCamera)
            {
                transform = Matrix.CreateTranslation(-DebugCameraPosition.X, -DebugCameraPosition.Y, 0);
            }
            else
            {
                transform = level.camera.Transform;
            }
            //transform.Translation = new Vector3(ViewportDimensions / 2.0f, 0.0f);
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                null,
                null,
                null,
                null,
                transform);

            //level.Draw(spriteBatch, gameTime);

            base.Draw(gameTime);

            //if (drawPhysics)
            {
                physicsWorldDebugRenderer.Draw(spriteBatch);
                DrawOnScreen("Drawing physical world");
            }

            if (!drawVisualHelpers.IsNone)
            {
                DrawOnScreen("Drawing visual helpers");

                if (drawVisualHelpers.IsVerbose)
                {
                    DrawBoundingBoxes(gameTime);
                }
            }

            if (!drawDebugData.IsNone)
            {
                DrawDebugData(gameTime);
                DrawOnScreen("Drawing debug data");
                DrawOnScreen(string.Format("Game state: {0}", GameState));

                if (drawDebugData.IsVerbose)
                {
                    // TODO draw more verbose data.
                }
            }

            spriteBatch.End();

            // Draw user interface related stuff

            spriteBatch.Begin();

            level.DrawUserInterface(spriteBatch);

            spriteBatch.DrawString(debugFont, DebugInfo, new Vector2(20, 20), Color.White);
            DebugInfo.Clear();

            spriteBatch.End();
        }

        private void DrawBoundingBoxes(GameTime gameTime)
        {
            var drawSize = new Vector2(4, 4);

            foreach (var go in level.GameObjects)
            {
                spriteBatch.FillRectangle(go.Pos - drawSize / 2, drawSize, Color.Red);
                spriteBatch.DrawRectangle((Microsoft.Xna.Framework.Rectangle)go.CalculateBoundingBox(), Color.Red);
            }
        }

        private void DrawDebugData(GameTime gameTime)
        {
            foreach (var go in level.GameObjects)
            {
                var debugMessage = new StringBuilder();
                debugMessage.AppendFormat("[{0}]", go.Name);

                var type = go.GetType();
                var goDebugData = (DebugData)Attribute.GetCustomAttribute(type, typeof(DebugData), true);
                if (goDebugData == null || goDebugData.Ignore)
                { continue; }

                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    var debugData = (DebugData)Attribute.GetCustomAttribute(prop, typeof(DebugData), true);

                    if (debugData == null)
                    {
                        if (!drawDebugData.IsVerbose)
                            continue; // No debug data and we are not verbose today.
                    }
                    else
                    {
                        if (debugData.Ignore)
                            continue; // Debug data says, we should ignore it.
                    }

                    var key = prop.Name;
                    var value = prop.GetValue(go, null);

                    debugMessage.AppendFormat("\n{0}: {1}", key, value);
                }

                var dimensions = debugFont.MeasureString(debugMessage);
                var position = go.Pos - go.Dimensions / 2;
                position.Y -= dimensions.Y;

                spriteBatch.DrawString(debugFont, debugMessage, position, Color.White);
            }
        }

        public void DrawOnScreen(string message, Vector2? position = null)
        {
            if (position == null)
            {
                DebugInfo.AppendLine(message);
                return;
            }

            var pos = position.Value;

            var upperLeft = Vector2.Zero;

            if (level.camera != null)
            {
                upperLeft = level.camera.Position - new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            }

            spriteBatch.DrawString(debugFont, message, upperLeft + pos, Color.White);
        }

        private void StartLoadingLevel(String name)
        {
            LoadingScreenDrawn = false;
            GameState.SetLoading();

            level = new Level(this);
            level.Name = name;
        }

        private void EndLoadLevel()
        {
            // Keep the reference to the current event system because the call to this.Ininitialize() will create a new one.
            Initialize();
            GameState.SetRunning();
            LoadingScreenDrawn = false;
            Events["levelInitialized"].trigger(level.Name);
        }

        private void onEndLevel(String data)
        {
            physicsWorld.bodies.Clear();
            StartLoadingLevel(data);
        }

        private void onExit(String data)
        {
            Exit();
        }

        private void OnUIShow(String data)
        {
            GameState.SetInMenu();
        }

        private void OnUIClose(String data)
        {
            GameState.SetRunning();
        }
    }
}

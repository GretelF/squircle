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

namespace Squircle
{
    /// <summary>
    /// Prevents a property of a game object to be used as debug data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IgnoreDebugData : System.Attribute
    {
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
        public bool drawDebugData { get; set; }                 ///< Draws textual game object data above them.
        public bool drawMoreDebugData { get; set; }             ///< Up to the specific game object to use.
        public bool drawVisualHelpers { get; set; }             ///< Draws some visual helpers for game objects.
        public SpriteFont debugFont { get; set; }
        public EventSystem EventSystem { get; set; }
        public InputHandler InputHandler { get; set; }

        public PhysicsDebugDraw PhysicsDebugDrawer { get; set; }

        public IList<Vector2> DebugScreenDataStack { get; set; }

        public bool LoadingNextLevel { get; set; }
        public bool LoadingScreenDrawn { get; set; }

        public uint NumFramesLoading { get; set; }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            drawPhysics = false;
            drawVisualHelpers = false;
            drawMoreDebugData = false;
            drawDebugData = false;

            DebugScreenDataStack = new List<Vector2>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InputHandler = new InputHandler();

            EventSystem = new EventSystem();
            EventSystem.getEvent("endLevel").addListener(onEndLevel);

            PhysicsDebugDrawer = new PhysicsDebugDraw();

            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.AABB);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.CenterOfMass);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.Joint);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.Pair);
            PhysicsDebugDrawer.AppendFlags(DebugDrawFlags.Shape);

            gameConfig = ConfigFile.FromFile("Content/level/game.cfg");
            level = new Level(this);
            level.Initialize(gameConfig["Levels"]["level_01"]);

            base.Initialize();
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

            level.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        protected override void Update(GameTime gameTime)
        {
            DebugScreenDataStack.Clear();

            if (LoadingNextLevel)
            {
                if (LoadingScreenDrawn)
                {
                    level.Initialize(gameConfig["Levels"][level.Name]);
                    level.LoadContent(Content);
                    LoadingNextLevel = false;
                    LoadingScreenDrawn = false;
                }
                return;
            }

            InputHandler.Update(gameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || InputHandler.IsDown(Keys.Escape))
            {
                this.Exit();
            }

            if (InputHandler.WasTriggered(Keys.F9))
            {
                drawPhysics = !drawPhysics;
            }

            if (InputHandler.WasTriggered(Keys.F10))
            {
                drawDebugData = !drawDebugData;
            }

            if (InputHandler.WasTriggered(Keys.F11))
            {
                drawMoreDebugData = !drawMoreDebugData;
            }

            if (InputHandler.WasTriggered(Keys.F12))
            {
                drawVisualHelpers = !drawVisualHelpers;
            }

            level.Update(gameTime);

            if (InputHandler.IsDown(Keys.Add))
            {
                level.camera.Scale += 0.01f;
            }

            if (InputHandler.IsDown(Keys.Subtract))
            {
                level.camera.Scale -= 0.01f;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (LoadingNextLevel)
            {
                spriteBatch.Begin();
                DrawOnScreen("Loading...");
                spriteBatch.End();
                LoadingScreenDrawn = true;
                ++NumFramesLoading;
                return;
            }

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                null, 
                null,
                null,
                null,
                level.camera.Transform);

            level.Draw(spriteBatch, gameTime);
            
            base.Draw(gameTime);

            if (drawPhysics)
            {
                DrawOnScreen("Drawing physical world");
            }

            if (drawVisualHelpers)
            {
                DrawBoundingBoxes(gameTime);
                DrawOnScreen("Drawing visual helpers");
            }

            if (drawDebugData)
            {
                DrawDebugData(gameTime);
                DrawOnScreen("Drawing debug data");
            }

            if (drawMoreDebugData)
            {
                DrawOnScreen("Drawing more debug data");
            }

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
            spriteBatch.DrawRectangle((Microsoft.Xna.Framework.Rectangle)level.circle.CalculateBoundingBox(), Color.Red);
            spriteBatch.DrawRectangle((Microsoft.Xna.Framework.Rectangle)level.square.CalculateBoundingBox(), Color.Red);
        }

        private void DrawDebugData(GameTime gameTime)
        {
            foreach (var go in level.GameObjects)
            {
                var debugMessage = new StringBuilder();
                debugMessage.AppendFormat("[{0}]", go.Name);

                var type = go.GetType();
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    var propIsIgnored = Attribute.GetCustomAttribute(prop, typeof(IgnoreDebugData), true) != null;
                    if (propIsIgnored)
                    {
                        continue;
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
            Vector2 pos;
            if (position == null)
            {
                if (DebugScreenDataStack.Count == 0)
                {
                    pos = new Vector2(20, 20);
                }
                else
                {
                    pos = DebugScreenDataStack.Last();
                    pos.Y += 20;
                }
                DebugScreenDataStack.Add(pos);
            }
            else
            {
                pos = position.Value;
            }

            var upperLeft = Vector2.Zero;

            if (level.camera != null)
            {
                upperLeft = level.camera.Position - new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            }

            spriteBatch.DrawString(debugFont, message, upperLeft + pos, Color.White);
        }

        private void onEndLevel(String data)
        {
            if (data == "credits")
            {
                // TODO show credits.
                throw new NotImplementedException();
            }

            LoadingScreenDrawn = false;
            LoadingNextLevel = true;
            NumFramesLoading = 0;

            level = new Level(this);
            level.Name = data;
        }

    }
}

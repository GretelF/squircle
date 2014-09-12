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
        public bool debugDrawingEnabled { get; set; }
        public SpriteFont debugFont { get; set; }
        public EventSystem EventSystem { get; set; }
        public InputHandler InputHandler { get; set; }

        public PhysicsDebugDraw PhysicsDebugDrawer { get; set; }

        public IList<Func<object>> DebugData { get; set; }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            debugDrawingEnabled = false;
            DebugData = new List<Func<object>>();
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
            InputHandler.Update(gameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || InputHandler.IsDown(Keys.Escape))
            {
                this.Exit();
            }

            if (InputHandler.WasTriggered(Keys.F9))
            {
                debugDrawingEnabled = !debugDrawingEnabled;
            }

            level.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

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

            if (debugDrawingEnabled)
            {
                DrawDebugData(gameTime);
            }

            spriteBatch.End();
        }

        private void DrawDebugData(GameTime gameTime)
        {
            foreach (var go in level.gameObjects)
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

                    var nestedGo = value as GameObject;

                    if (nestedGo != null)
                    {
                        debugMessage.AppendFormat("\n{0}: {1}", key, nestedGo.Name);
                    }
                    else
                    {
                        debugMessage.AppendFormat("\n{0}: {1}", key, value);
                    }

                }

                var dimensions = debugFont.MeasureString(debugMessage);
                var position = go.Pos - go.Dimensions / 2;
                position.Y -= dimensions.Y;

                spriteBatch.DrawString(debugFont, debugMessage, position, Microsoft.Xna.Framework.Color.White);
            }
        }

        public void DrawOnScreen(string message, Vector2 position)
        {
            spriteBatch.DrawString(debugFont, message, position, Microsoft.Xna.Framework.Color.White);
        }

        private void onEndLevel(String data)
        {
            throw new NotImplementedException();
        }

    }
}

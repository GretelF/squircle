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
        public EventSystem EventSystem { get; set; }
        public InputHandler InputHandler { get; set; }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            gameConfig = ConfigFile.FromFile("Content/level/game.cfg");
            level = new Level(this);
            level.Initialize(gameConfig["Levels"]["level0"]);

            debugDrawingEnabled = false;

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
                SpriteSortMode.BackToFront, 
                BlendState.AlphaBlend, 
                null, 
                null,
                null,
                null,
                level.camera.Transform);

            level.Draw(spriteBatch, gameTime);
            
            base.Draw(gameTime);
            
            spriteBatch.End();
        }

        private void onEndLevel(String data)
        {
            throw new NotImplementedException();
        }

    }
}

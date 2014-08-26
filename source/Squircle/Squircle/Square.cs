using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    class Square : Player
    {
        private Texture2D squareTexture;
        private Vector2 squarePos;

        public override Texture2D Texture
        {
            get
            {
                return squareTexture;
            }
        }

        public override Vector2 Pos
        {
            get { return squarePos; }
            set { squarePos = value; }
        }

        public Square(Game game) :  base(game)
        {

        }


        public override void LoadContent(ContentManager content)
        {
            squareTexture = content.Load<Texture2D>("player/square");
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();


            Vector2 tempPos = squarePos;
            float speed = 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state.IsKeyDown(Keys.Right))
                tempPos.X += speed;
            if (state.IsKeyDown(Keys.Left))
                tempPos.X -= speed;
            if (state.IsKeyDown(Keys.Down))
                tempPos.Y += speed;
            if (state.IsKeyDown(Keys.Up))
                tempPos.Y -= speed;

            squarePos = tempPos;

            base.Update(gameTime);
        }
    }
}

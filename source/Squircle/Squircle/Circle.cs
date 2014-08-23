using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    class Circle : Player
    {
        private Texture2D circleTexture;
        private Vector2 circlePos;

         public override Texture2D Texture
        {
            get
            {
                return circleTexture;
            }
        }

        public override Vector2 Pos
        {
            get { return circlePos; }
            set { circlePos = value; }
        }

        public Circle(Game game) : base(game)
        {

        }
        


        public override void LoadContent(ContentManager content)
        {
            circleTexture = content.Load<Texture2D>("circle");
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}

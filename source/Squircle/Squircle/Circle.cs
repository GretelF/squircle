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


        public override void LoadContent(ContentManager content)
        {
            circleTexture = content.Load<Texture2D>("circle");
        }
    }
}

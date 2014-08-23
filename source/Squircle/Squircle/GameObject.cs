using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Squircle
{
    public abstract class GameObject
    {
        protected Game Game { get; private set; }

        public abstract Texture2D Texture { get; }
        public abstract Vector2 Pos { get; set; }

        public GameObject(Game game)
        {
            Game = game;
        }

        public abstract void Initialize();
        public abstract void LoadContent(ContentManager content);

        public virtual void Update(GameTime gameTime)
        {
            
        }
    }
}

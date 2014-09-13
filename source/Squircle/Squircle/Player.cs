using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace Squircle
{
    public enum PlayerType
    {
        None,
        Square,
        Circle,
        All
    }

    public abstract class Player : GameObject
    {
        [IgnoreDebugData]
        public virtual Body Body { get; protected set; }

        public override Vector2 Pos
        {
            get { return Game.level.ConvertFromBox2D(Body.Position); }
            set { Body.Position = Game.level.ConvertToBox2D(value); }
        }

        public Player(Game game) : base(game)
        {
            DrawOrder = 10;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Box2D.XNA;
using Configuration;

namespace Squircle
{

    public class TriggerObject : GameObject
    {
        private Vector2 _pos;
        private Texture2D _texture;
        private string _textureName;

        public override Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public TriggerObject(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureName);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
            _textureName = section["texture"];
            Pos = section["position"].AsVector2();
            var dim = section["dimensions"].AsVector2();

            var bodyDef = new BodyDef();
            var fixtureDef = new FixtureDef();
            var shape = new PolygonShape();
            shape.SetAsBox(dim.X / 2, dim.Y / 2);
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground };
            bodyDef.position = Pos;
            Game.level.World.CreateBody(bodyDef).CreateFixture(fixtureDef);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Microsoft.Xna.Framework.Color.White);
        }
    }
    
    public class ButtonObject : GameObject
    {
        private Vector2 _pos;
        private Texture2D _texture;
        private string _textureName;

        public override Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value;}
        }
        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public ButtonObject(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureName);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
            _textureName = section["texture"];
            Pos = section["position"].AsVector2();
            var dim = section["dimensions"].AsVector2();

            var bodyDef = new BodyDef();
            var fixtureDef = new FixtureDef();
            var shape = new PolygonShape();
            shape.SetAsBox(dim.X / 2, dim.Y / 2);
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground};
            bodyDef.position = Pos;
            Game.level.World.CreateBody(bodyDef).CreateFixture(fixtureDef);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Microsoft.Xna.Framework.Color.White);
        }
    }
}

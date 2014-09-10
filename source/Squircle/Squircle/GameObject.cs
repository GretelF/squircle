using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using Configuration;

namespace Squircle
{
    public abstract class GameObject
    {
        public static GameObject Create(Game game, ConfigSection section)
        {
            var typeName = (string)section["type"];
            GameObject resultObject;

            switch (typeName)
            {
                case "platform":
                    resultObject = new PlatformObject(game);
                    break;
                case "trigger":
                    throw new NotImplementedException();
                case "button":
                    throw new NotImplementedException();
                default:
                {
                    var message = new StringBuilder();
                    message.AppendFormat("Unsupported GameObject type: {0}", typeName);
                    throw new NotSupportedException(message.ToString());
                }
            }

            resultObject.InitializeFromConfig(section);

            return resultObject;
        }

        protected Game Game { get; private set; }

        public abstract Texture2D Texture { get; }
        public abstract Vector2 Pos { get; set; }

        public bool IsEnabled { get; set; }

        public GameObject(Game game)
        {
            Game = game;
            IsEnabled = true;
        }

        public abstract void Initialize();
        public abstract void LoadContent(ContentManager content);
        public abstract void InitializeFromConfig(ConfigSection section);

        
        public virtual void PrePhysicsUpdate(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void BeginContact(ContactInfo contactInfo)
        {
        }

        public virtual void EndContact(ContactInfo contactInfo)
        {
        }
    }

    public class PhantomObject : GameObject
    {

        public override Texture2D Texture { get { return null; } }
        public override Vector2 Pos { get; set; }

        public PhantomObject(Game game):base(game)
        {
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(ContentManager content)
        {
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
        }
    }

    public class PlatformObject : GameObject
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

        public PlatformObject(Game game) : base(game)
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
            var bounds = section["bounds"].AsRectangle();

            Pos = new Vector2(bounds.X, bounds.Y);

            // TODO physics
            //Game.level.World

            var bodyDef = new BodyDef();
            var fixtureDef = new FixtureDef();
            var shape = new PolygonShape();
            shape.SetAsBox(bounds.Width/2, bounds.Height/2);
            fixtureDef.shape = shape;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground};
            bodyDef.type = BodyType.Static;
            bodyDef.position = Pos;
            Game.level.World.CreateBody(bodyDef).CreateFixture(fixtureDef);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Pos, Microsoft.Xna.Framework.Color.White);
        }
    }
}

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
    public enum State
    {
        Active,
        Inactive
    }

    public abstract class GameObject
    {
        public static GameObject Create(Game game, string gameObjectName, ConfigSection section)
        {
            var typeName = (string)section["type"];
            GameObject resultObject;

            switch (typeName)
            {
                case "platform":
                    resultObject = new PlatformObject(game);
                    break;
                case "trigger":
                    resultObject = new TriggerObject(game);
                    break;
                case "button":
                    resultObject = new ButtonObject(game);
                    break;
                default:
                {
                    var message = new StringBuilder();
                    message.AppendFormat("Unsupported GameObject type: {0}", typeName);
                    throw new NotSupportedException(message.ToString());
                }
            }

            resultObject.Name = gameObjectName;
            resultObject.InitializeFromConfig(section);

            return resultObject;
        }

        protected Game Game { get; private set; }

        [IgnoreDebugData]
        public string Name { get; set; }

        [IgnoreDebugData]
        public abstract Texture2D Texture { get; }
        public abstract Vector2 Pos { get; set; }

        /// <summary>
        /// X => Width, Y => Height
        /// </summary>
        public abstract Vector2 Dimensions { get; }

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
        public override Vector2 Dimensions { get { return Vector2.Zero; } }

        public PhantomObject(Game game) : base(game)
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

}

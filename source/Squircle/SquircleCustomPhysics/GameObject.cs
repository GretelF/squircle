using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using Configuration;
using Squircle.Physics;
using System.Diagnostics;

namespace Squircle
{
    public enum StateType
    {
        Active,
        Inactive,
    }

    public class State
    {
        public StateType Value { get; set; }

        public bool IsActive
        {
            get { return Value == StateType.Active; }
        }

        public bool IsInactive
        {
            get { return Value == StateType.Inactive; }
        }

        public void toggle()
        {
            if (IsActive)
            {
                setInactive();
            }
            else
            {
                setActive();
            }
        }

        public void setActive()
        {
            Value = StateType.Active;
        }

        public void setInactive()
        {
            Value = StateType.Inactive;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DebugData]
    public abstract class GameObject
    {
        public static GameObject Create(Game game, string gameObjectName, ConfigSection section)
        {
            var typeName = (string)section["type"];
            GameObject resultObject;

            switch (typeName)
            {
                case "square":
                    resultObject = new Square(game);
                    break;
                case "circle":
                    resultObject = new Circle(game);
                    break;
                case "platform":
                    resultObject = new PlatformObject(game);
                    break;
                case "trigger":
                    resultObject = new TriggerObject(game);
                    break;
                case "toggleButton":
                    resultObject = new ToggleButtonObject(game);
                    break;
                case "holdButton":
                    resultObject = new HoldButtonObject(game);
                    break;
                default:
                {
                    var message = new StringBuilder();
                    message.AppendFormat("Unsupported GameObject type: {0}", typeName);
                    throw new NotSupportedException(message.ToString());
                }
            }

            resultObject.Name = gameObjectName;
            resultObject.Initialize(section);

            Debug.Assert(resultObject.Body != null, "Game objects have to have a valid physics body after initialization.");
            
            return resultObject;
        }

        [DebugData(Ignore = true)]
        protected Game Game { get; private set; }

        [DebugData(Ignore = true)]
        public scBody Body { get; protected set; }

        [DebugData(Ignore = true)]
        public string Name { get; set; }

        [DebugData(Ignore = true)]
        public abstract Texture2D Texture { get; }

        public Vector2 Pos
        {
            get { return Body != null ? Body.transform.position : Vector2.Zero; }
        }

        [DebugData(Ignore = true)]
        public int DrawOrder { get; set; }

        public GameObject(Game game)
        {
            Game = game;
        }

        public virtual void Initialize(ConfigSection section)
        {
            section.IfOptionExists("name", opt => Name = opt);
            section.IfOptionExists("drawOrder", opt => DrawOrder = opt);

        }

        public abstract void LoadContent(ContentManager content);

        public virtual void PrePhysicsUpdate(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

#if false
        public virtual void BeginContact(ContactInfo contactInfo)
        {
        }

        public virtual void EndContact(ContactInfo contactInfo)
        {
        }
#endif

        public override string ToString()
        {
            return string.Format("{0}@{1}", Name, Pos);
        }
    }
}

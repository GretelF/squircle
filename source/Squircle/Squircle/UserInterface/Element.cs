using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Squircle.UserInterface
{
    public class Element
    {
        public static Element Create(Window parent, string name, ConfigSection section)
        {
            var typeName = (string)section["type"];
            Element result;

            switch (typeName)
            {
                case "button":
                    result = new Button(parent.Game);
                    break;
                case "window":
                    result = new Window(parent.Game);
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("Unsupported Element type: {0}",
                                      typeName));
            }

            result.Type = typeName;
            result.ParentWindow = parent;
            result.Name = name;
            result.Initialize(section);

            return result;
        }

        public Game Game { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public Vector2 Position { get; set; }

        public virtual Vector2 PositionAbsolute
        {
            get { return ParentWindow == null ? Position : Position + ParentWindow.PositionAbsolute; }
        }

        public Vector2 Dimensions { get; set; }

        public Window ParentWindow { get; set; }

        public Element(Game game)
        {
            Game = game;
        }

        public Element(Element other)
        {
            Game = other.Game;
            Type = other.Type;
            Name = other.Name;
            Position = other.Position;
            Dimensions = other.Dimensions;
            ParentWindow = other.ParentWindow;
        }

        public virtual void Initialize(ConfigSection section)
        {
            Position = section["position"].AsVector2();
            Dimensions = section["dimensions"].AsVector2();
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

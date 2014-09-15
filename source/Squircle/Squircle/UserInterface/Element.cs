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
        public static Element Create(Element parent, string name, ConfigSection section)
        {
            var typeName = (string)section["type"];
            Element result;

            switch (typeName)
            {
                case "button":
                    result = new Button(parent.Game);
                    break;
                case "screen":
                    result = new Screen(parent.Game);
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("Unsupported Element type: {0}",
                                      typeName));
            }

            result.Parent = parent;
            result.Name = name;
            result.Initialize(section);

            return result;
        }
        public Game Game { get; set; }

        public string Name { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 PositionAbsolute
        {
            get { return Parent == null ? Position : Position + Parent.PositionAbsolute; }
        }

        public Vector2 Dimensions { get; set; }

        public Element Parent { get; set; }

        public IList<Element> Children { get; set; }

        public Element(Game game)
        {
            Game = game;
            Children = new List<Element>();
        }
        
        public virtual void Initialize(ConfigSection section)
        {
            Position = section["position"].AsVector2();
            Dimensions = section["dimensions"].AsVector2();

            if (!section.Options.ContainsKey("children"))
            {
                return;
            }

            var childrenConfigName = section["children"];
            var childrenConfig = ConfigFile.FromFile(childrenConfigName);
            InitializeChildren(childrenConfig.Sections, childrenConfig.GlobalSection);
        }

        public void InitializeChildren(IDictionary<string, ConfigSection> sections, ConfigSection globalSection)
        {
            foreach (var section in sections)
            {
                if (section.Value == globalSection)
                {
                    continue;
                }

                var child = Element.Create(this, section.Key, section.Value);
                Children.Add(child);
            }
        }

        public virtual void LoadContent(ContentManager content)
        {
            foreach (var child in Children)
            {
                child.LoadContent(content);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var child in Children)
            {
                child.Update(gameTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in Children)
            {
                child.Draw(spriteBatch);
            }
        }

        public T GetChild<T>(string name) where T : Element
        {
            return Children.First(ui => ui.Name == name) as T;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

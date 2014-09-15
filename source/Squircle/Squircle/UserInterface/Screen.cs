using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Squircle.UserInterface
{
    public class Screen : Element
    {
        public Texture2D Texture { get; set; }
        public string TextureName { get; set; }

        public Screen(Game game) : base(game) { }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            TextureName = section["texture"];
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>(TextureName);

            base.LoadContent(content);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, PositionAbsolute - Dimensions / 2, Color.White);

            base.Draw(spriteBatch);
        }
    }
}

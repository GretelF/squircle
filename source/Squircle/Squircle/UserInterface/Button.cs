using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Squircle.UserInterface
{
    public class Button : Element
    {
        public Texture2D TextureOn { get; set; }
        public string TextureOnName { get; set; }

        public Texture2D TextureOff { get; set; }
        public string TextureOffName { get; set; }

        public Event EventActivate { get; set; }
        public string EventActivateData { get; set; }

        public Texture2D Texture
        {
            get { return OnOffState.IsActive ? TextureOn : TextureOff; }
        }

        public State OnOffState { get; set; }

        public Button(Game game)
            : base(game)
        {
            OnOffState = new State();
            OnOffState.setInactive();
        }

        public void ToggleOnOff()
        {
            OnOffState.toggle();
        }

        public void Activate()
        {
            if (EventActivate == null) { return; }

            EventActivate.trigger(EventActivateData);
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            TextureOnName = section["textureOn"];
            TextureOffName = section["textureOff"];

            if (section.Options.ContainsKey("onActivate"))
            {
                EventActivate = Game.EventSystem.getEvent(section["onActivate"]);
            }

            if (section.Options.ContainsKey("onActivateData"))
            {
                EventActivateData = section["onActivateData"];
            }
        }

        public override void LoadContent(ContentManager content)
        {
            TextureOn = content.Load<Texture2D>(TextureOnName);
            TextureOff = content.Load<Texture2D>(TextureOffName);

            base.LoadContent(content);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, PositionAbsolute - Dimensions / 2, Color.White);

            base.Draw(spriteBatch);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, OnOffState);
        }
    }
}

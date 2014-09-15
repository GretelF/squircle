using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Squircle.UserInterface
{
    public enum Direction
    {
        Up,
        Down
    }

    public class Window : Element
    {
        public Texture2D BackgroundTexture { get; set; }
        public string BackgroundTextureName { get; set; }

        public IList<Button> Buttons { get; set; }

        public RingIndex SelectedButtonIndex { get; set; }
        public Button SelectedButton { get { return Buttons[SelectedButtonIndex]; } }

        public Window(Game game) : base(game)
        {
            SelectedButtonIndex = new RingIndex();
            Buttons = new List<Button>();
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            BackgroundTextureName = section["background"];
            var buttonsConfig = section["buttons"].AsConfigFile();
            foreach (var buttonSection in buttonsConfig.Sections)
            {
                if (buttonSection.Value == buttonsConfig.GlobalSection) { continue; }

                var button = Element.Create(this, buttonSection.Key, buttonSection.Value) as Button;
                Debug.Assert(button != null, "Invalid UI child type.");
                Buttons.Add(button);
            }

            Reset();
        }

        public void Reset()
        {
            Debug.Assert(Buttons.Count > 0, "A windows needs at least 1 button!");
            SelectedButtonIndex.UpperBound = Buttons.Count - 1;
            SelectedButtonIndex.Value = 0;

            foreach (var button in Buttons)
            {
                button.SetOnOff(false);
            }
            SelectedButton.ToggleOnOff();
        }

        public override void LoadContent(ContentManager content)
        {
            BackgroundTexture = content.Load<Texture2D>(BackgroundTextureName);

            foreach (var button in Buttons)
            {
                button.LoadContent(content);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackgroundTexture, PositionAbsolute - Dimensions / 2, Color.White);

            foreach (var button in Buttons)
            {
                button.Draw(spriteBatch);
            }
        }

        public virtual void Navigate(Direction dir)
        {
            SelectedButton.ToggleOnOff();
            switch (dir)
            {
            case Direction.Up:
                SelectedButtonIndex.Decrement();
                break;
            case Direction.Down:
                SelectedButtonIndex.Increment();
                break;
            }
            SelectedButton.ToggleOnOff();
        }

        public virtual void ActivateSelectedButton()
        {
            SelectedButton.Activate();
        }
    }
}

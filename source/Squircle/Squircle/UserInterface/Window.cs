using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public Window(Window other) : base(other)
        {
            SelectedButtonIndex = new RingIndex();
            SelectedButtonIndex.UpperBound = other.SelectedButtonIndex.UpperBound;
            SelectedButtonIndex.Value = other.SelectedButtonIndex.Value;

            Buttons = new List<Button>();
            foreach (var button in other.Buttons)
            {
                // Add a copy
                Buttons.Add(new Button(button));
            }

            BackgroundTexture = other.BackgroundTexture;
            BackgroundTextureName = other.BackgroundTextureName;
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

        public override void Update(GameTime gameTime)
        {
            var input = Game.InputHandler;

            if (input.WasTriggered(Keys.Enter)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.A))
            {
                ActivateSelectedButton();
            }

            if (input.WasTriggered(Keys.Down)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.DPadDown))
            {
                Navigate(Direction.Down);
            }

            if (input.WasTriggered(Keys.Up)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.DPadUp))
            {
                Navigate(Direction.Up);
            }

            if (input.WasTriggered(Keys.Back)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.B))
            {
                Game.Events["ui.hide"].trigger(Name);
            }

            if (input.WasTriggered(Keys.Escape)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.Start))
            {
                Game.Events["ui.close"].trigger(null);
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

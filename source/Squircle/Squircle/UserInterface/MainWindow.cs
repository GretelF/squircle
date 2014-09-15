using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Squircle.UserInterface
{
    public class MainWindow : Element
    {
        public Texture2D Background { get; set; }
        public string BackgroundName { get; set; }

        public IList<Button> PressButtons { get; set; }

        public Screen ActiveScreen { get; set; }
        public Event ShowScreenEvent { get; set; }
        public Event HideScreenEvent { get; set; }

        public int SelectedButtonIndex { get; set; }
        public Button SelectedButton
        {
            get { return PressButtons[SelectedButtonIndex]; }
        }

        public MainWindow(Game game)
            : base(game)
        {
            PressButtons = new List<Button>();
        }

        public override void Initialize(ConfigSection section)
        {
            Name = "root";
            Dimensions = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            Position = Vector2.Zero;

            BackgroundName = section["background"];

            Game.EventSystem.getEvent("toggleRunningAndInMenu").addListener(onToggleRunningAndInMenu);

            ShowScreenEvent = Game.EventSystem.getEvent("showScreen");
            ShowScreenEvent.addListener(OnShowScreen);

            HideScreenEvent = Game.EventSystem.getEvent("hideScreen");
            HideScreenEvent.addListener(OnHideScreen);
        }

        public void InitializeFromConfigFile(ConfigFile config)
        {
            Initialize(config.GlobalSection);
            InitializeChildren(config.Sections, config.GlobalSection);

            foreach (var child in Children)
            {
                var button = child as Button;
                if (button == null)
                {
                    continue;
                }

                PressButtons.Add(button);
            }

            if (PressButtons.Count > 0)
            {
                SelectedButtonIndex = 0;
                SelectedButton.OnOffState.setActive();
            }
        }

        private void onToggleRunningAndInMenu(string data)
        {
            SelectedButton.ToggleOnOff();
            SelectedButtonIndex = 0;
            SelectedButton.ToggleOnOff();
            HideScreenEvent.trigger(null);
        }

        public override void LoadContent(ContentManager content)
        {
            Background = content.Load<Texture2D>(BackgroundName);

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            var input = Game.InputHandler;

            if (input.WasTriggered(Keys.Enter) || input.WasTriggered(Buttons.A))
            {
                if (ActiveScreen == null)
                {
                    SelectedButton.Activate();
                }
                else
                {
                    HideScreenEvent.trigger(null);
                }
            }

            if (ActiveScreen != null)
            {
                return;
            }

            if (input.WasTriggered(Keys.Down) || input.WasTriggered(Buttons.DPadDown))
            {
                SelectNextButton();
            }

            if (input.WasTriggered(Keys.Up) || input.WasTriggered(Buttons.DPadUp))
            {
                SelectPreviousButton();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            if (ActiveScreen != null)
            {
                ActiveScreen.Draw(spriteBatch);
                return;
            }

            foreach (var button in PressButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        private void SelectNextButton()
        {
            if (PressButtons.Count < 2)
            {
                return;
            }

            SelectedButton.OnOffState.toggle();
            ++SelectedButtonIndex;
            if (SelectedButtonIndex >= PressButtons.Count)
                SelectedButtonIndex = 0;
            SelectedButton.OnOffState.toggle();
        }

        private void SelectPreviousButton()
        {
            if (PressButtons.Count < 2)
            {
                return;
            }

            SelectedButton.OnOffState.toggle();
            --SelectedButtonIndex;
            if (SelectedButtonIndex < 0)
                SelectedButtonIndex = PressButtons.Count - 1;
            SelectedButton.OnOffState.toggle();
        }

        private void OnShowScreen(string data)
        {
            ActiveScreen = GetChild<Screen>(data);
        }

        private void OnHideScreen(string data)
        {
            ActiveScreen = null;
        }
    }
}

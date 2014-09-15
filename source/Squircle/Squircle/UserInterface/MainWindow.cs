using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Squircle.UserInterface
{
    public class MainWindow : Window
    {
        public override Vector2 PositionAbsolute
        {
            get { return Position; }
        }

        public IList<Window> Windows { get; set; }

        public Stack<Window> ActiveWindows { get; set; }

        public Window ActiveWindow
        {
            get { return ActiveWindows.Count == 0 ? this : ActiveWindows.Peek(); }
        }

        public MainWindow(Game game)
            : base(game)
        {
            Windows = new List<Window>();
            ActiveWindows = new Stack<Window>();
        }

        public void InitializeFromConfigFile(ConfigFile cfg)
        {
            Name = "mainWindow";

            foreach (var opt in cfg["Windows"].Options)
            {
                if (opt.Key == Name)
                {
                    Initialize(opt.Value.AsConfigFile().GlobalSection);
                }
                else
                {
                    var window = Element.Create(this, opt.Key, opt.Value.AsConfigFile().GlobalSection) as Window;
                    Debug.Assert(window != null,
                        "non-window in [Windows] section of user interface definition!");
                    Windows.Add(window);
                }
            }

            Game.EventSystem.getEvent("toggleRunningAndInMenu").addListener(onToggleRunningAndInMenu);
            Game.EventSystem.getEvent("ui.show").addListener(OnShowScreen);
            Game.EventSystem.getEvent("ui.hide").addListener(OnHideScreen);
        }

        private void onToggleRunningAndInMenu(string data)
        {
            //SelectedButton.ToggleOnOff();
            //SelectedButtonIndex = 0;
            //SelectedButton.ToggleOnOff();
            //HideScreenEvent.trigger(null);
            foreach (var window in ActiveWindows)
            {
                window.Reset();
            }
            ActiveWindows.Clear();
            Reset();
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            foreach (var window in Windows)
            {
                window.LoadContent(content);
            }
        }

        public new void Update(GameTime gameTime)
        {
            var input = Game.InputHandler;

            if (input.WasTriggered(Keys.Back)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.B))
            {
                Game.EventSystem.getEvent("ui.hide").trigger(null);
            }

            if (input.WasTriggered(Keys.Enter)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.A))
            {
                ActiveWindow.ActivateSelectedButton();
            }

            if (input.WasTriggered(Keys.Down)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.DPadDown))
            {
                ActiveWindow.Navigate(Direction.Down);
            }

            if (input.WasTriggered(Keys.Up)
             || input.WasTriggered(Microsoft.Xna.Framework.Input.Buttons.DPadUp))
            {
                ActiveWindow.Navigate(Direction.Up);
            }

            ActiveWindow.Update(gameTime);
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            ActiveWindow.Draw(spriteBatch);
        }

        private void OnShowScreen(string data)
        {
            if (ActiveWindow.Name == data) { return; }

            var window = Windows.Single(w => w.Name == data);
            ActiveWindows.Push(window);
        }

        private void OnHideScreen(string data)
        {
            if (ActiveWindows.Count > 0)
            {
                ActiveWindows.Pop();
            }
        }
    }
}

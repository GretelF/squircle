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

        /// <summary>
        /// A list of unique window instances.
        /// </summary>
        public IList<Window> Windows { get; set; }

        /// <summary>
        /// A stack of copies of the active windows.
        /// <remarks>The windows need to be copy in order to preserve their state.</remarks>
        /// </summary>
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
            if (data == ActiveWindow.Name) { return; }

            var window = data == Name ? this : Windows.Single(w => w.Name == data);

            // Make a copy
            ActiveWindows.Push(new Window(window));
        }

        private void OnHideScreen(string data)
        {
            if (ActiveWindows.Count > 0)
            {
                ActiveWindow.Reset();
                ActiveWindows.Pop();
            }
        }
    }
}

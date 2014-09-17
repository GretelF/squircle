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
    public class RootElement : Element
    {
        public override Vector2 PositionAbsolute
        {
            get { return Position; }
        }

        public RootElement(Game game) : base(game)
        {
            Name = Type = "root";
            Position = Game.ViewportDimensions / 2;
            Dimensions = Game.ViewportDimensions;
        }
    }

    public class MainWindow
    {
        public Game Game { get; set; }

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
            get { return ActiveWindows.Peek(); }
        }

        public string InitialWindowName { get; set; }

        /// <summary>
        /// More or less a dummy object to act as root of the ui.
        /// </summary>
        public Element Root { get; set; }

        public MainWindow(Game game)
        {
            Game = game;
            Root = new RootElement(Game);
            Windows = new List<Window>();
            ActiveWindows = new Stack<Window>();
        }

        public void InitializeFromConfigFile(ConfigFile cfg)
        {
            foreach (var opt in cfg["Windows"].Options)
            {
                var window = Element.Create(Root, opt.Key, opt.Value.AsConfigFile().GlobalSection) as Window;
                Debug.Assert(window != null,
                    "non-window in [Windows] section of user interface definition!");
                Windows.Add(window);
            }

            Game.EventSystem.getEvent("ui.show").addListener(OnShowScreen);
            Game.EventSystem.getEvent("ui.hide").addListener(OnHideScreen);
            Game.EventSystem.getEvent("ui.close").addListener(OnClose);
        }

        public void LoadContent(ContentManager content)
        {
            foreach (var window in Windows)
            {
                window.LoadContent(content);
            }

            if (InitialWindowName != null)
            {
                Game.EventSystem.getEvent("ui.show").trigger(InitialWindowName);
                InitialWindowName = null;
            }
        }

        public void Update(GameTime gameTime)
        {
            ActiveWindow.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ActiveWindow.Draw(spriteBatch);
        }

        private void OnShowScreen(string data)
        {
            // Don't stack the same window on itself.
            if (ActiveWindows.Count > 0 && data == ActiveWindow.Name) { return; }

            var window = Windows.Single(w => w.Name == data);

            // Make a copy
            ActiveWindows.Push(new Window(window));
        }

        private void OnHideScreen(string data)
        {
            if (ActiveWindows.Count > 0)
            {
                ActiveWindows.Pop();

                if (ActiveWindows.Count == 0)
                {
                    Game.EventSystem.getEvent("ui.close").trigger(null);
                }
            }
        }

        private void OnClose(string data)
        {
            ActiveWindows.Clear();
        }
    }
}

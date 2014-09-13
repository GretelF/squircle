using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    public class UserInterfaceElement
    {
        public static UserInterfaceElement Create(UserInterfaceElement parent, string name, ConfigSection section)
        {
            var typeName = (string)section["type"];
            UserInterfaceElement result;

            switch (typeName)
            {
                case "button":
                    result = new UserInterfaceButton(parent.Game);
                    break;
                case "screen":
                    result = new UserInterfaceScreen(parent.Game);
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("Unsupported UserInterfaceElement type: {0}",
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

        public UserInterfaceElement Parent { get; set; }

        public IList<UserInterfaceElement> Children { get; set; }

        public UserInterfaceElement(Game game)
        {
            Game = game;
            Children = new List<UserInterfaceElement>();
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

                var child = UserInterfaceElement.Create(this, section.Key, section.Value);
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

        public T GetUserInterfaceElement<T>(string name) where T : UserInterfaceElement
        {
            return Children.FirstOrDefault(ui => ui.Name == name) as T;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }

    public class UserInterfaceButton : UserInterfaceElement
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

        public UserInterfaceButton(Game game) : base(game)
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
            spriteBatch.Draw(Texture, Position - Dimensions / 2, Color.White);

            base.Draw(spriteBatch);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, OnOffState);
        }
    }

    public class UserInterfaceScreen : UserInterfaceElement
    {
        public Texture2D Texture { get; set; }
        public string TextureName { get; set; }

        public UserInterfaceScreen(Game game) : base(game) { }

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
            spriteBatch.Draw(Texture, Position - Dimensions / 2, Color.White);

            base.Draw(spriteBatch);
        }
    }

    public class UserInterface : UserInterfaceElement
    {
        public Texture2D Background { get; set; }
        public string BackgroundName { get; set; }

        public IList<UserInterfaceButton> PressButtons { get; set; }

        public UserInterfaceScreen ActiveScreen { get; set; }
        public Event ShowScreenEvent { get; set; }
        public Event HideScreenEvent { get; set; }

        public int SelectedButtonIndex { get; set; }
        public UserInterfaceButton SelectedButton
        {
            get { return PressButtons[SelectedButtonIndex]; }
        }

        public UserInterface(Game game) : base(game)
        {
            PressButtons = new List<UserInterfaceButton>();
        }

        public override void Initialize(ConfigSection section)
        {
            Name = "root";
            Dimensions = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            Position = -Dimensions / 2;

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
                var button = child as UserInterfaceButton;
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
            if (SelectedButtonIndex >= PressButtons.Count) SelectedButtonIndex = 0;
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
            if (SelectedButtonIndex < 0) SelectedButtonIndex = PressButtons.Count - 1;
            SelectedButton.OnOffState.toggle();
        }

        private void OnShowScreen(string data)
        {
            ActiveScreen = GetUserInterfaceElement<UserInterfaceScreen>(data);
        }

        private void OnHideScreen(string data)
        {
            ActiveScreen = null;
        }
    }
}

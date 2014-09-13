using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Box2D.XNA;
using Configuration;

namespace Squircle
{

    public class TriggerObject : GameObject
    {
        private Vector2 _dim;
        private Texture2D _texture;
        private string _textureName;
        private string _enterEvent;
        private string _leaveEvent;
        private string _enterEventData;
        private string _leaveEventData;

        public Body Body { get; set; }

        public override Vector2 Pos
        {
            get { return Game.level.ConvertFromBox2D(Body.Position); }
            set { Body.Position = Game.level.ConvertToBox2D(value); }
        }

        public override Vector2 Dimensions
        {
            get { return _dim; }
        }

        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public TriggerObject(Game game)
            : base(game)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            if (_textureName == null) { return; }
            _texture = content.Load<Texture2D>(_textureName);
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            if (section.Options.ContainsKey("texture"))
            {
                _textureName = section["texture"];
            }

            var pos = section["position"].AsVector2();
            _dim = section["dimensions"].AsVector2();

            if (section.Options.ContainsKey("enterEvent"))
            {
                _enterEvent = section["enterEvent"];
            }

            if (section.Options.ContainsKey("leaveEvent"))
            {
                _leaveEvent = section["leaveEvent"];
            }

            if (section.Options.ContainsKey("enterEventData"))
            {
                _enterEventData = section["enterEventData"];
            }

            if (section.Options.ContainsKey("leaveEventData"))
            {
                _leaveEventData = section["leaveEventData"];
            }

            var bodyDef = new BodyDef();
            bodyDef.userData = this;
            var fixtureDef = new FixtureDef();
            var shape = new PolygonShape();
            shape.SetAsBox(Game.level.ConvertToBox2D(_dim.X / 2), Game.level.ConvertToBox2D(_dim.Y / 2));
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground };
            bodyDef.position = Game.level.ConvertToBox2D(pos);
            Body = Game.level.World.CreateBody(bodyDef);
            Body.CreateFixture(fixtureDef);
        }

        public override void BeginContact(ContactInfo contactInfo)
        {
            if (_enterEvent != null)
            {
                Game.EventSystem.getEvent(_enterEvent).trigger(_enterEventData);
            }
        }

        public override void EndContact(ContactInfo contactInfo)
        {
            if (_leaveEvent != null)
            {
                Game.EventSystem.getEvent(_leaveEvent).trigger(_leaveEventData);
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_texture == null) { return; }

            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Microsoft.Xna.Framework.Color.White);
        }
    }

    public class ButtonObjectBase : GameObject
    {
        public override Vector2 Pos { get; set; }
        public override Vector2 Dimensions { get { return new Vector2(ProximityRadius * 2); } }

        public override Texture2D Texture
        {
            get { return OnOffState.IsActive ? TextureOn : TextureOff; }
        }

        [IgnoreDebugData]
        public Texture2D TextureOn { get; set; }
        public string TextureOnName { get; set; }

        [IgnoreDebugData]
        public Texture2D TextureOff { get; set; }
        public string TextureOffName { get; set; }

        public State InitialOnOffState { get; set; }
        public State OnOffState { get; set; }
        public State PressedState { get; set; }

        public bool IsOnOffInInitialState
        {
            get { return OnOffState.Value == InitialOnOffState.Value; }
        }

        public GameObject Master { get; set; }

        public float ProximityRadius { get; set; }

        [IgnoreDebugData]
        public float ProximityRadiusSquared { get { return ProximityRadius * ProximityRadius; } }

        public Event OnEvent { get; set; }
        public string OnEventData { get; set; }

        public Event OffEvent { get; set; }
        public string OffEventData { get; set; }

        public bool IsMasterInProximity 
        {
            get
            {
                return Vector2.DistanceSquared(Pos, Master.Pos) <= ProximityRadiusSquared;
            }
        }

        public ButtonObjectBase(Game game) : base(game)
        {
            InitialOnOffState = new State();
            OnOffState = new State();
            PressedState = new State() { Value = StateType.Inactive };
        }

        public override void LoadContent(ContentManager content)
        {
            TextureOn = content.Load<Texture2D>(TextureOnName);
            TextureOff = content.Load<Texture2D>(TextureOffName);
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            TextureOnName = section["textureOn"];
            TextureOffName = section["textureOff"];
            Pos = section["position"].AsVector2();
            ProximityRadius = section["proximityRadius"];

            string masterName = section["master"];

            Master = Game.level.GetGameObject(masterName);

            var state = section["state"];

            if (state == "active")
            {
                InitialOnOffState.setActive();
            }
            else if (state == "inactive")
            {
                InitialOnOffState.setInactive();
            }
            else
            {
                throw new ArgumentException("Unsupported GameObject state: " + state);
            }

            OnOffState.Value = InitialOnOffState.Value;

            if (section.Options.ContainsKey("onButtonOn"))
            {
                OnEvent = Game.EventSystem.getEvent(section["onButtonOn"]);
            }

            if (section.Options.ContainsKey("onButtonOnData"))
            {
                OnEventData = section["onButtonOnData"];
            }

            if (section.Options.ContainsKey("onButtonOff"))
            {
                OffEvent = Game.EventSystem.getEvent(section["onButtonOff"]);
            }

            if (section.Options.ContainsKey("onButtonOffData"))
            {
                OffEventData = section["onButtonOffData"];
            }

            Game.EventSystem.getEvent("playerButtonPress").addListener(OnPlayerButtonPress);
            Game.EventSystem.getEvent("playerButtonRelease").addListener(OnPlayerButtonRelease);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var tex = Texture;

            var pos = Pos - new Vector2(tex.Width / 2, tex.Height / 2);
            spriteBatch.Draw(tex, pos, Color.White);

            if (Game.drawVisualHelpers)
            {
                spriteBatch.DrawLine(Pos, Master.Pos, Color.Red);
                spriteBatch.DrawCircle(Pos, ProximityRadius, 16, Color.Yellow);
            }
        }

        public virtual void OnPlayerButtonPress(string data)
        {
            PressedState.setActive();
        }

        public virtual void OnPlayerButtonRelease(string data)
        {
            PressedState.setInactive();
        }
    }

    public class ToggleButtonObject : ButtonObjectBase
    {
        public ToggleButtonObject(Game game) : base(game) { }

        public override void OnPlayerButtonPress(string data)
        {
            base.OnPlayerButtonPress(data);

            // Check if the master triggered this event and if the master is close enough.
            if (data != Master.Name || !IsMasterInProximity)
            {
                return;
            }

            OnOffState.toggle();

            if (OnOffState.IsActive)
            {
                if (OnEvent != null) OnEvent.trigger(OnEventData);
            }
            else
            {
                if (OffEvent != null) OffEvent.trigger(OffEventData);
            }
        }
    }

    public class HoldButtonObject : ButtonObjectBase
    {
        public bool WasMasterInProximity { get; private set; }

        public HoldButtonObject(Game game) : base(game) { }

        public override void Update(GameTime gameTime)
        {
            if (IsOnOffInInitialState || IsMasterInProximity)
            {
                return;
            }

            ToggleAndTriggerOnOff();
        }

        public override void OnPlayerButtonPress(string data)
        {
            base.OnPlayerButtonPress(data);

            // Check if the master triggered this event and if the master is close enough.
            if (data != Master.Name || !IsMasterInProximity)
            {
                return;
            }

            ToggleAndTriggerOnOff();
        }

        public override void OnPlayerButtonRelease(string data)
        {
            base.OnPlayerButtonRelease(data);

            // Check if the master triggered this event and if the master is close enough.
            if (IsOnOffInInitialState || data != Master.Name || !IsMasterInProximity)
            {
                return;
            }

            ToggleAndTriggerOnOff();
        }

        private void ToggleAndTriggerOnOff()
        {
            OnOffState.toggle();

            if (OnOffState.IsActive)
            {
                if (OnEvent != null) OnEvent.trigger(OnEventData);
            }
            else
            {
                if (OffEvent != null) OffEvent.trigger(OffEventData);
            }
        }
    }
}

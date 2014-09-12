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
        private Vector2 _pos;
        private Vector2 _dim;
        private Texture2D _texture;
        private string _textureName;
        private string _enterEvent;
        private string _leaveEvent;
        private string _enterEventData;
        private string _leaveEventData;

        public override Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
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

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void LoadContent(ContentManager content)
        {
            if (_textureName == null) { return; }
            _texture = content.Load<Texture2D>(_textureName);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
            if (section.Options.ContainsKey("texture"))
            {
                _textureName = section["texture"];
            }

            _pos = section["position"].AsVector2();
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
            shape.SetAsBox(_dim.X / 2, _dim.Y / 2);
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground };
            bodyDef.position = Pos;
            Game.level.World.CreateBody(bodyDef).CreateFixture(fixtureDef);
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

    public class ButtonObject : GameObject
    {
        private Vector2 _pos;
        private Vector2 _dim;
        private Texture2D _texture;
        private string _textureName;
        private string _onPressEvent;
        private string _onPressEventData;
        private string _onReleaseEvent;
        private string _onReleaseEventData;
        private GameObject _master;
        private float _proximityRadiusSquared;

        public override Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public override Vector2 Dimensions
        {
            get { return _dim; }
        }

        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public bool MasterIsCloseEnough
        {
            get { return (Pos - _master.Pos).LengthSquared() <= _proximityRadiusSquared; }
        }

        public GameObject Master { get { return _master; } }

        public ButtonObject(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureName);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
            _textureName = section["texture"];
            Pos = section["position"].AsVector2();
            var proximityRadius = section["proximityRadius"];
            _dim = new Vector2(proximityRadius * 2);
            _proximityRadiusSquared = proximityRadius * proximityRadius;

            string masterName = section["master"];

            if (masterName == "Circle")
            {
                _master = Game.level.circle;
            }
            else if (masterName == "Square")
            {
                _master = Game.level.square;
            }
            else
            {
                throw new ArgumentException("Unsupported master.");
            }

            if (section.Options.ContainsKey("onPressEvent"))
            {
                _onPressEvent = section["onPressEvent"];
            }

            if (section.Options.ContainsKey("onPressEventData"))
            {
                _onPressEventData = section["onPressEventData"];
            }

            if (section.Options.ContainsKey("onReleaseEvent"))
            {
                _onReleaseEvent = section["onReleaseEvent"];
            }

            if (section.Options.ContainsKey("onReleaseEventData"))
            {
                _onReleaseEventData = section["onReleaseEventData"];
            }

            Game.EventSystem.getEvent("playerButtonPress").addListener(onPlayerButtonPress);
            Game.EventSystem.getEvent("playerButtonRelease").addListener(onPlayerButtonRelease);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Color.White);

            if (Game.drawVisualHelpers)
            {
                spriteBatch.DrawLine(Pos, _master.Pos, Color.Red);
                spriteBatch.DrawCircle(Pos, (int)(Dimensions.X / 2), 16, Color.Yellow);
            }
        }

        public void onPlayerButtonPress(String data)
        {
            if (!MasterIsCloseEnough)
            {
                return;
            }

            if (data == "Square" && _master is Square
             || data == "Circle" && _master is Circle)
            {
                Game.EventSystem.getEvent(_onPressEvent).trigger(_onPressEventData);
            }
        }

        public void onPlayerButtonRelease(String data)
        {
            if (!MasterIsCloseEnough)
            {
                return;
            }

            if (data == "Square" && _master is Square
             || data == "Circle" && _master is Circle)
            {
                Game.EventSystem.getEvent(_onPressEvent).trigger(_onPressEventData);
            }
        }
    }
}

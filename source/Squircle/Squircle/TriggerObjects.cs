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
            _texture = content.Load<Texture2D>(_textureName);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
            _textureName = section["texture"];
            Pos = section["position"].AsVector2();
            var dim = section["dimensions"].AsVector2();

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
            shape.SetAsBox(dim.X / 2, dim.Y / 2);
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
            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Microsoft.Xna.Framework.Color.White);
        }
    }
    
    public class ButtonObject : GameObject
    {
        private Vector2 _pos;
        private Texture2D _texture;
        private string _textureName;
        private string _onPressEvent;
        private string _onPressEventData;
        private bool triggerEnabled = false;
        private PlayerType playerType;

        public override Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value;}
        }
        public override Texture2D Texture
        {
            get { return _texture; }
        }

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
            var dim = section["dimensions"].AsVector2();

            string playerTypeName = section["player"];

            if (playerTypeName == "Circle")
            {
                playerType = PlayerType.Circle;
            }
            else if (playerTypeName == "Square")
            {
                playerType = PlayerType.Square;
            }
            else
            {
                throw new ArgumentException("Unsupported player type.");
            }

            if (section.Options.ContainsKey("onPressEvent"))
            {
                _onPressEvent = section["onPressEvent"];
            }

            if (section.Options.ContainsKey("onPressEventData"))
            {
                _onPressEventData = section["onPressEventData"];
            }

            var bodyDef = new BodyDef();
            bodyDef.userData = this;
            var fixtureDef = new FixtureDef();
            var shape = new PolygonShape();
            shape.SetAsBox(dim.X / 2, dim.Y / 2);
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground};
            bodyDef.position = Pos;
            Game.level.World.CreateBody(bodyDef).CreateFixture(fixtureDef);

            Game.EventSystem.getEvent("onPressEvent").addListener(onPressEvent);
        }

        public override void BeginContact(ContactInfo contactInfo)
        {
            if (contactInfo.other is Player)
            {
                triggerEnabled = true;
            }
        }

        public override void EndContact(ContactInfo contactInfo)
        {
            if (contactInfo.other is Player)
            {
                triggerEnabled = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Microsoft.Xna.Framework.Color.White);
        }

        public void onPressEvent(String data)
        {
            if (!triggerEnabled)
            {
                return;
            }

            if (data == "Square" && playerType == PlayerType.Square || data == "Circle" && playerType == PlayerType.Circle)
            {
                Game.EventSystem.getEvent(_onPressEvent).trigger(_onPressEventData);
            }
        }
    }
}

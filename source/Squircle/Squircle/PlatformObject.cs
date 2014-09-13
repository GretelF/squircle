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
    public class PlatformObject : GameObject
    {
        private Vector2 _dimensions;
        private Texture2D _texture;

        [IgnoreDebugData]
        public Body Body { get; set; }

        public override Vector2 Pos
        {
            get { return Game.level.ConvertFromBox2D(Body.Position); }
            set { Body.Position = Game.level.ConvertToBox2D(value); }
        }

        public Vector2 PreviousPos { get; set; }

        public override Vector2 Dimensions
        {
            get { return _dimensions; }
        }

        public State State { get; set; }

        [IgnoreDebugData]
        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public string TextureName { get; set; }

        public float MovementSpeed { get; set; }

        public Vector2 WaypointStart { get; set; }
        public Vector2 WaypointEnd { get; set; }

        public GameObject Target { get; set; }

        public bool IsAtTarget { get { return Pos.EpsilonCompare(Target.Pos); } }

        public PlatformObject(Game game)
            : base(game)
        {
            State = new State();
            Target = new PhantomObject(Game);
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(TextureName);
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            TextureName = section["texture"];
            _dimensions = section["dimensions"].AsVector2();
            section.IfOptionExists("toggleEvent",
                opt => Game.EventSystem.getEvent(opt).addListener(onToggleEvent));

            var stateName = section["state"];

            if (stateName == "active")
            {
                State.setActive();
            }
            else if (stateName == "inactive")
            {
                State.setInactive();
            }
            else
            {
                throw new ArgumentException("Unsupported GameObject state: " + stateName);
            }

            section.IfOptionExists("movementSpeed", opt => MovementSpeed = opt);

            section.IfOptionExists("toggleWaypointEvent", opt => Game.EventSystem.getEvent(opt).addListener(onToggleWaypointEvent));

            var bodyDef = new BodyDef();
            var fixtureDef = new FixtureDef();
            var shape = new PolygonShape();
            shape.SetAsBox(Game.level.ConvertToBox2D(Dimensions.X / 2), Game.level.ConvertToBox2D(Dimensions.Y / 2));
            fixtureDef.shape = shape;
            fixtureDef.userData = new LevelElementInfo() { type = LevelElementType.Ground };
            bodyDef.type = BodyType.Kinematic;
            bodyDef.position = Game.level.ConvertToBox2D(section["position"].AsVector2());
            bodyDef.active = State.IsActive;
            Body = Game.level.World.CreateBody(bodyDef);
            Body.CreateFixture(fixtureDef);

            section.IfOptionExists("waypointStart",
                opt => WaypointStart = opt.AsVector2(),
                () => WaypointStart = Pos);

            section.IfOptionExists("waypointEnd",
                opt => WaypointEnd = opt.AsVector2(),
                () => WaypointEnd = Pos);

            section.IfOptionExists("target",
                targetName =>
                {
                    if (targetName == "start")
                    {
                        Target.Pos = WaypointStart;
                    }
                    else if (targetName == "end")
                    {
                        Target.Pos = WaypointEnd;
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported target name: " + targetName);
                    }
                },
                () => Target.Pos = WaypointEnd);
        }

        public override void Update(GameTime gameTime)
        {
            if (State.IsInactive || IsAtTarget) 
            {
                PreviousPos = Pos; 
                return; 
            }

            var diff = Target.Pos - Pos;
            var diffBefore = Target.Pos - PreviousPos;

            if (!diff.IsInSameQuadrant(diffBefore))
            {
                Body.SetLinearVelocity(Vector2.Zero);
                Pos = Target.Pos;
                return; 
            }

            diff.Normalize();
            var velocity = diff * (float)(MovementSpeed);

            Body.SetLinearVelocity(Game.level.ConvertToBox2D(velocity));

            PreviousPos = Pos;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (State.IsInactive) { return; }

            var pos = Pos - new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw(_texture, pos, Microsoft.Xna.Framework.Color.White);
        }

        public void onToggleEvent(String data)
        {
            State.toggle();
            Body.SetActive(State.IsActive);
        }

        public void onToggleWaypointEvent(String data)
        {
            if (State.IsInactive)
            {
                return;
            }

            if (Target.Pos == WaypointStart)
            {
                Target.Pos = WaypointEnd;
            }
            else
            {
                Target.Pos = WaypointStart;
            }
        }
    }
}

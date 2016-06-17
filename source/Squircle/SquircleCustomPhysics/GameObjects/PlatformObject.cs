using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Box2D.XNA;
using Configuration;
using Squircle.Physics;

namespace Squircle
{
    public delegate void PlatformFader(GameTime gameTime);

    public enum FadeState
    {
        Off,
        FadeIn,
        FadeOut
    }

    public class PlatformObject : GameObject
    {
        private Vector2 _dimensions;
        private Texture2D _texture;

        public Vector2 PreviousPos { get; set; }

        public State State { get; set; }

        [DebugData(Ignore = true)]
        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public string TextureName { get; set; }

        public float MovementSpeed { get; set; }

        private Vector2[] Waypoints { get; set; }

        public Vector2 WaypointStart { get { return Waypoints[0]; } set { Waypoints[0] = value; } }
        public Vector2 WaypointEnd { get { return Waypoints[1]; } set { Waypoints[1] = value; } }

        private RingIndex _targetIndex;

        [DebugData(Ignore = true)]
        public Vector2 Target { get { return Waypoints[_targetIndex]; } }

        public bool WasAtTarget { get; set; }
        public bool IsAtTarget { get { return Pos.EpsilonCompare(Target, 0.25f); } }

        [DebugData]
        public float TargetDistance { get { return Vector2.Distance(Body.transform.position, Target); } }

        /// <summary>
        /// In seconds.
        /// </summary>
        public float FadeTime { get; set; }
        public float CurrentFadeTime { get; set; }

        [DebugData]
        public Color DrawColor { get { return new Color(1.0f, 1.0f, 1.0f, CurrentFadeTime / FadeTime); } }

        private PlatformFader[] Faders { get; set; }
        public PlatformFader CurrentFader { get { return Faders[(int)FadeState]; } }

        [DebugData]
        public FadeState FadeState { get; private set; }

        public PlatformObject(Game game)
            : base(game)
        {
            State = new State();
            Waypoints = new Vector2[2];
            _targetIndex = new RingIndex()
            {
                Value = 0,
                LowerBound = 0,
                UpperBound = Waypoints.Length - 1
            };
            Faders = new PlatformFader[4];
            FadeTime = 0.2f;
            Faders[0] = dt => { /* Do nothing.*/ };
            Faders[1] = dt => // Fading in
            {
                CurrentFadeTime += (float)dt.ElapsedGameTime.TotalSeconds;
                if (CurrentFadeTime > FadeTime)
                {
                    CurrentFadeTime = FadeTime;
//                    Body.SetActive(true);
                    DisableFading();
                }
            };
            Faders[2] = dt => // Fading out
            {
                CurrentFadeTime -= (float)dt.ElapsedGameTime.TotalSeconds;
                if (CurrentFadeTime < 0.0f)
                {
                    CurrentFadeTime = 0.0f;
                    DisableFading();
                }
            };
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
                opt => Game.Events[opt].addListener(onToggleEvent));

            section.IfOptionExists("fadeTime", opt => FadeTime = opt);

            var stateName = section["state"];

            if (stateName == "active")
            {
                State.setActive();
                CurrentFadeTime = FadeTime;
            }
            else if (stateName == "inactive")
            {
                State.setInactive();
                CurrentFadeTime = 0.0f;
            }
            else
            {
                throw new ArgumentException("Unsupported GameObject state: " + stateName);
            }

            section.IfOptionExists("movementSpeed", opt => MovementSpeed = opt);

            section.IfOptionExists("toggleWaypointEvent", opt => Game.Events[opt].addListener(onToggleWaypointEvent));

            var bodyDescription = new scBodyDescription();
            bodyDescription.userData = this;
            var bodyPartDescription = new scBodyPartDescription();
            var shape = scRectangleShape.fromLocalPositionAndHalfExtents(Vector2.Zero, _dimensions / 2);
            bodyPartDescription.shape = shape;
            bodyDescription.transform.position = section["position"].AsVector2();
            Body = Game.level.World.createBody(bodyDescription, bodyPartDescription);
            Body.owner = this;

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
                        _targetIndex.Value = 0;
                    }
                    else if (targetName == "end")
                    {
                        _targetIndex.Value = 1;
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported target name: " + targetName);
                    }
                },
                () => _targetIndex.Value = 1);
        }

        public override void Update(GameTime gameTime)
        {
            CurrentFader(gameTime);

            if (State.IsInactive) { return; }

            if (IsAtTarget)
            {
                if (!WasAtTarget)
                {
                    Body.linearVelocity = Vector2.Zero;
                    Body.transform.position = Target;
                    WasAtTarget = true;
                }
                return;
            }

            var diff = Target - Pos;

            diff.Normalize();
            var velocity = diff * MovementSpeed;

            Body.linearVelocity = velocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = Pos;
            spriteBatch.Draw(_texture,
                             pos,
                             null,
                             DrawColor,
                             Body.transform.rotation.radians,
                             new Vector2(_texture.Width / 2, _texture.Height / 2),
                             1.0f,
                             SpriteEffects.None,
                             0.0f);

            if (State.IsInactive) { return; }

            if (!Game.drawVisualHelpers.IsNone)
            {
                spriteBatch.DrawLine(pos, WaypointStart, Color.Blue);
                spriteBatch.DrawLine(pos, WaypointEnd, Color.Blue);
                if(!IsAtTarget) spriteBatch.DrawLine(pos, Target, Color.Red);
            }
        }

        public void onToggleEvent(String data)
        {
            State.toggle();

            if (State.IsActive) { FadeIn(); }
            else                { FadeOut(); }
        }

        public void onToggleWaypointEvent(String data)
        {
            if (State.IsInactive) { return; }

            _targetIndex.Increment();
            WasAtTarget = false;
        }

        private void DisableFading()
        {
            FadeState = Squircle.FadeState.Off;
        }

        private void FadeIn()
        {
            CurrentFadeTime = 0.0f;
            FadeState = Squircle.FadeState.FadeIn;
        }

        private void FadeOut()
        {
            CurrentFadeTime = FadeTime;
            FadeState = Squircle.FadeState.FadeOut;
//            Body.SetActive(false);
        }
    }
}

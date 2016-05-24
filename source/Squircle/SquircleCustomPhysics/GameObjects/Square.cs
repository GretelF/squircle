using Box2D.XNA;
using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Squircle.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Squircle
{
    public class Square : GameObject
    {
        private string textureName;
        private Texture2D squareTexture;
        public float SideLength { get; set; }

        public bool CanJump
        {
            get
            {
                return JumpingEnabled
                    && Math.Abs(Body.linearVelocity.Y) <= JumpThreshold;
            }
        }
        public bool JumpingEnabled { get; set; }
        public float JumpThreshold { get; set; }

        public float Speed { get; set; }
        public float MaxSpeed { get; set; }
        public float JumpImpulse { get; set; }

        public override Texture2D Texture
        {
            get
            {
                return squareTexture;
            }
        }

        public override Vector2 Dimensions
        {
            get { return new Vector2(SideLength, SideLength); }
        }

        public Square(Game game) :  base(game) {}

        public override void LoadContent(ContentManager content)
        {
            squareTexture = content.Load<Texture2D>(textureName);
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            textureName = section["texture"];
            var position = section["position"].AsVector2();
            SideLength = section["sideLength"];
            Speed = section["speed"];
            MaxSpeed = section["maxSpeed"];
            JumpImpulse = section["jumpImpulse"];
            JumpThreshold = section["jumpThreshold"];

            var bodyDescription = new scBodyDescription();
            bodyDescription.bodyType = scBodyType.Kinematic;
            bodyDescription.transform.position = position;
            bodyDescription.linearDamping = section["linearDamping"];
            bodyDescription.angularDamping = section["angularDamping"];
            bodyDescription.userData = this;

            IList<scBodyPartDescription> bodyPartDescriptions = new List<scBodyPartDescription>();
            var bodyPartDescription = new scBodyPartDescription();

            var shape = new scRectangleShape();
            var offset = SideLength / 2;
            shape.vertices[0] = new Vector2(-offset, -offset);
            shape.vertices[1] = new Vector2( offset, -offset);
            shape.vertices[2] = new Vector2( offset,  offset);
            shape.vertices[3] = new Vector2(-offset,  offset);

            bodyPartDescription.shape = shape;

            bodyPartDescriptions.Add(bodyPartDescription);

            Body = Game.level.World.createBody(bodyDescription, bodyPartDescriptions);

#if false
            var bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;

            bodyDef.angle = 0;
            bodyDef.position = Game.level.ConvertToBox2D(pos);
            bodyDef.inertiaScale = section["inertiaScale"];
            bodyDef.linearDamping = section["linearDamping"];
            bodyDef.angularDamping = section["angularDamping"];

            bodyDef.userData = this;

            Body = Game.level.World.createBody(bodyDef);

            var shape = new PolygonShape();
            var offset = SideLength / 2;
            shape.Set(new Vector2[]{
                Game.level.ConvertToBox2D(new Vector2(-offset, -offset)),
                Game.level.ConvertToBox2D(new Vector2(offset, -offset)),
                Game.level.ConvertToBox2D(new Vector2(offset, offset)),
                Game.level.ConvertToBox2D(new Vector2(-offset, offset))
                }
            , 4);

            var fixture = new FixtureDef();
            fixture.restitution = section["restitution"];
            fixture.density = section["density"];
            fixture.shape = shape;
            fixture.friction = section["friction"];
            Body.CreateFixture(fixture);
#endif
        }

        public override void PrePhysicsUpdate(GameTime gameTime)
        {
            var input = Game.InputHandler;

            var tempPos = Vector2.Zero;
            tempPos.X = input.GamePadState.ThumbSticks.Right.X * Speed;

            if (input.IsDown(Keys.Right))
                tempPos.X = Speed;
            if (input.IsDown(Keys.Left))
                tempPos.X = -Speed;
            if ((input.IsDown(Keys.Up) || input.IsDown(Buttons.RightShoulder)) && CanJump)
            {
#if false
                Body.ApplyLinearImpulse(new Vector2(0.0f, -JumpImpulse), Pos);
#endif
            }
            JumpingEnabled = false;

            if (input.WasTriggered(Keys.Down) || input.WasTriggered(Buttons.RightTrigger))
            {
                Game.Events["playerButtonPress"].trigger(Name);
            }
            else if (input.WasReleased(Keys.Down) || input.WasReleased(Buttons.RightTrigger))
            {
                Game.Events["playerButtonRelease"].trigger(Name);
            }

            var velocity = Body.linearVelocity + tempPos;

            if (Math.Abs(velocity.X) >= MaxSpeed)
            {
                velocity.X = Math.Sign(velocity.X) * MaxSpeed;
            }

            Body.linearVelocity = velocity;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateAbilityToJump();
            base.Update(gameTime);
        }

        private void UpdateAbilityToJump()
        {
#if false
            for (var contact = Body.GetContactList(); contact != null; contact = contact.Next)
            {
                if (!contact.Contact.IsTouching()) { continue; }

                var circle = contact.Other.GetUserData() as Circle;
                if (circle != null)
                {
                    JumpingEnabled = true;
                    return;
                }

                var fixtureA = contact.Contact.GetFixtureA();
                var fixtureB = contact.Contact.GetFixtureB();

                Fixture otherFixture;
                if (fixtureA.GetBody() == Body)
                {
                    otherFixture = fixtureB;
                }
                else
                {
                    Debug.Assert(fixtureB.GetBody() == Body);
                    otherFixture = fixtureA;
                }

                var elementInfo = otherFixture.GetUserData() as LevelElementInfo;
                if (elementInfo != null && elementInfo.type == LevelElementType.Ground)
                {
                    JumpingEnabled = true;
                }
            }
#endif
        }
       
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(squareTexture, Pos, null, Color.White, Body.transform.rotation.radians, new Vector2(SideLength / 2, SideLength / 2), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}

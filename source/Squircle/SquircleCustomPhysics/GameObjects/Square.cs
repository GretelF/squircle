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
            bodyDescription.inertiaScale = section["inertiaScale"];
            bodyDescription.linearDamping = section["linearDamping"];
            bodyDescription.angularDamping = section["angularDamping"];
            bodyDescription.userData = this;

            var bodyPartDescription = new scBodyPartDescription();

            bodyPartDescription.shape = scRectangleShape.fromHalfExtents(new Vector2(SideLength / 2));
            bodyPartDescription.friction = section["friction"];
            bodyPartDescription.restitution = section["restitution"];
            bodyPartDescription.density = section["density"];

            Body = Game.level.World.createBody(bodyDescription, bodyPartDescription);
            Body.owner = this;
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

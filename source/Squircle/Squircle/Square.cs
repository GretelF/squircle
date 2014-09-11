using Box2D.XNA;
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
    class Square : Player
    {
        private Texture2D squareTexture;
        private Vector2 squarePos;
        private float squareSideLength = 50.0f;
        public float SideLength { get { return squareSideLength; } set { squareSideLength = value; } }
        private Level level;
        private Body body;
        public Body Body { get { return body; } }
        private Boolean canJump = false;

        public override Texture2D Texture
        {
            get
            {
                return squareTexture;
            }
        }

        public override Vector2 Pos
        {
            get { return squarePos; }
            set { squarePos = value; }
        }

        public Square(Game game, Level level) :  base(game)
        {
            this.level = level;
        }


        public override void LoadContent(ContentManager content)
        {
            squareTexture = content.Load<Texture2D>("player/square_50px");
        }

        public override void Initialize()
        {

            var bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;

            bodyDef.angle = 0;
            bodyDef.position = squarePos;
            bodyDef.inertiaScale = 1.0f;
            bodyDef.linearDamping = 0.0f;
            bodyDef.angularDamping = 10.0f;

            bodyDef.userData = this;

            body = level.World.CreateBody(bodyDef);

            var shape = new PolygonShape();
            var offset = squareSideLength / 2;
            shape.Set(new Vector2[]{
                new Vector2(-offset, -offset),
                new Vector2(offset, -offset),
                new Vector2(offset, offset),
                new Vector2(-offset, offset)
                }
            , 4);

          

            var fixture = new FixtureDef();
            fixture.restitution = 0.1f;
            fixture.density = 1.0f;
            fixture.shape = shape;
            fixture.friction = .2f;
            body.CreateFixture(fixture);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
        }

        public override void PrePhysicsUpdate(GameTime gameTime)
        {
            Vector2 tempPos = squarePos;
            float speed = 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Game.InputHandler.IsDown(Keys.Right))
                tempPos.X += speed;
            if (Game.InputHandler.IsDown(Keys.Left))
                tempPos.X -= speed;
            if (Game.InputHandler.IsDown(Keys.Up) && canJump)
            {
                body.ApplyLinearImpulse(new Vector2(0.0f, -10000000.0f), body.GetPosition());
            }
            canJump = false;

            if (Game.InputHandler.WasTriggered(Keys.Down))
            {
                Game.EventSystem.getEvent("onPressEvent").trigger("Square");
            }

            var velocity = body.GetLinearVelocity() + tempPos - squarePos;

            body.SetLinearVelocity(velocity);
        }

        public override void Update(GameTime gameTime)
        {
            squarePos = body.GetPosition();
            base.Update(gameTime);
        }
       
        public override void Draw (SpriteBatch spriteBatch)
        {
            var pos = squarePos + new Vector2(-squareSideLength/2, -squareSideLength/2);
            spriteBatch.Draw(squareTexture, squarePos, null, Microsoft.Xna.Framework.Color.White, body.Rotation, new Vector2(squareSideLength/2, squareSideLength/2), 1.0f, SpriteEffects.None, 0.0f);
        }

        public override void BeginContact(ContactInfo contactInfo)
        {
            Manifold manifold;
            contactInfo.contact.GetManifold(out manifold);
            Fixture fixture;
            if (contactInfo.fixtureType == FixtureType.A)
            {
                fixture = contactInfo.contact.GetFixtureB();
            }
            else
            {
                fixture = contactInfo.contact.GetFixtureA();
            }

            var gameObjectUserData = (GameObject)fixture.GetBody().GetUserData();
            var circle = gameObjectUserData as Circle;

            if (circle != null)
            {
                // TODO: check if above circle.
                canJump = true;
                return;
            }

            var elementInfo = fixture.GetUserData() as LevelElementInfo;
            if (elementInfo != null && elementInfo.type == LevelElementType.Ground)
            {
                canJump = true;
                return;
            }


        }
    }
}

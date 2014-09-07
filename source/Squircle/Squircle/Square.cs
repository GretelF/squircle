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
        private Level level;
        private Body body;

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

            var c = new ContactConstraint();
            var fixture = new FixtureDef();
            fixture.restitution = 0.1f;
            fixture.density = 1.0f;
            fixture.shape = shape;
            fixture.friction = .2f;
            body.CreateFixture(fixture);


        }

        public void PrePhysicsUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();


            Vector2 tempPos = squarePos;
            float speed = 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state.IsKeyDown(Keys.Right))
                tempPos.X += speed;
            if (state.IsKeyDown(Keys.Left))
                tempPos.X -= speed;
            if (state.IsKeyDown(Keys.Up))
                body.ApplyLinearImpulse(new Vector2(0.0f, -100000.0f), body.GetPosition());

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
    }
}

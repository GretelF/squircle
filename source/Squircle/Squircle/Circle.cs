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
    class Circle : Player
    {
        private Texture2D circleTexture;
        private Vector2 circlePos;
        private float circleRadius = 20.0f;
        private Level level;
        public Body body;

        public override Texture2D Texture
        {
            get
            {
                return circleTexture;
            }
        }

        public override Vector2 Pos
        {
            get { return circlePos; }
            set { circlePos = value; }
        }

        public Circle(Game game, Level level)
            : base(game)
        {
            this.level = level;
        }


        public override void LoadContent(ContentManager content)
        {
            circleTexture = content.Load<Texture2D>("player/circle_40px");
        }

        public override void Initialize()
        {
            var bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;

            bodyDef.angle = 0;
            bodyDef.position = circlePos;
            bodyDef.inertiaScale = 1.0f;
            bodyDef.linearDamping = 0.0f;
            bodyDef.angularDamping = 10.0f;

            body = level.World.CreateBody(bodyDef);

            var shape = new CircleShape();
            shape._radius = circleRadius;

            var fixture = new FixtureDef();
            fixture.restitution = 0.1f;
            fixture.density = 1.0f;
            fixture.shape = shape;
            fixture.friction = 10.0f;
            body.CreateFixture(fixture);
        }

        public override void InitializeFromConfig(ConfigSection section)
        {
        }

        public override void PrePhysicsUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();


            float tempDir = 0.0f;
            float speed = 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state.IsKeyDown(Keys.D))
                tempDir = 1.0f;
            if (state.IsKeyDown(Keys.A))
                tempDir = -1.0f;

            body.ApplyTorque(tempDir * 50000000);
        }

        public override void Update(GameTime gameTime)
        {
            circlePos = body.GetPosition();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = circlePos + new Vector2(-circleRadius, -circleRadius);

            spriteBatch.Draw(circleTexture, circlePos, null, Microsoft.Xna.Framework.Color.White, body.Rotation, new Vector2(circleRadius, circleRadius), 1.0f, SpriteEffects.None, 0.0f);
        }
    }

}

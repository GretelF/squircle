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
    public class Circle : Player
    {
        private Texture2D circleTexture;
        private Vector2 circlePos;
        public float Radius { get; set; }

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

        public override Vector2 Dimensions
        {
            get { return new Vector2(Radius * 2, Radius * 2); }
        }

        public Circle(Game game) : base(game) {}

        public override void LoadContent(ContentManager content)
        {
            circleTexture = content.Load<Texture2D>("player/circle_40px");
        }

        public override void Initialize(ConfigSection section)
        {
            circlePos = section["position"].AsVector2();
            Radius = section["radius"];

            var bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;

            bodyDef.position = circlePos;
            bodyDef.inertiaScale = section["inertiaScale"];
            bodyDef.linearDamping = section["linearDamping"];
            bodyDef.angularDamping = section["angularDamping"];

            bodyDef.userData = this;

            Body = Game.level.World.CreateBody(bodyDef);

            var shape = new CircleShape();
            shape._radius = Radius;

            var fixture = new FixtureDef();
            fixture.restitution = section["restitution"];
            fixture.density = section["density"];
            fixture.shape = shape;
            fixture.friction = section["friction"];
            Body.CreateFixture(fixture);
        }

        public override void PrePhysicsUpdate(GameTime gameTime)
        {
            float tempDir = 0.0f;
            float speed = 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Game.InputHandler.IsDown(Keys.D))
            {
                tempDir = 1.0f;
            }
            if (Game.InputHandler.IsDown(Keys.A))
            {
                tempDir = -1.0f;
            }
            if (Game.InputHandler.WasTriggered(Keys.S))
            {
                Game.EventSystem.getEvent("playerButtonPress").trigger(Name);
            }
            else if (Game.InputHandler.WasReleased(Keys.S))
            {
                Game.EventSystem.getEvent("playerButtonRelease").trigger(Name);
            }

            Body.ApplyTorque(tempDir * 50000000);
        }

        public override void Update(GameTime gameTime)
        {
            circlePos = Body.GetPosition();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = circlePos + new Vector2(-Radius, -Radius);

            spriteBatch.Draw(circleTexture, circlePos, null, Color.White, Body.Rotation, new Vector2(Radius, Radius), 1.0f, SpriteEffects.None, 0.0f);
        }
    }

}

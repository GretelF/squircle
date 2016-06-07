using Box2D.XNA;
using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Squircle.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    public class Circle : GameObject
    {
        private string textureName;
        private Texture2D circleTexture;
        private Vector2 circlePos;
        public float Radius { get; set; }
        public float MaxTorque { get; set; }


        public override Texture2D Texture
        {
            get
            {
                return circleTexture;
            }
        }

        public override Vector2 Dimensions
        {
            get { return new Vector2(Radius * 2, Radius * 2); }
        }

        public Circle(Game game) : base(game) {}

        public override void LoadContent(ContentManager content)
        {
            circleTexture = content.Load<Texture2D>(textureName);
        }

        public override void Initialize(ConfigSection section)
        {
            base.Initialize(section);

            textureName = section["texture"];
            circlePos = section["position"].AsVector2();
            Radius = section["radius"];

            var bodyDescription = new scBodyDescription();
            bodyDescription.bodyType = scBodyType.Kinematic;

            bodyDescription.transform.position = circlePos;
            bodyDescription.inertiaScale = section["inertiaScale"];
            bodyDescription.linearDamping = section["linearDamping"];
            bodyDescription.angularDamping = section["angularDamping"];

            bodyDescription.userData = this;

            var shape = new scCircleShape();
            shape.radius = Radius;

            var bodyPartDescription = new scBodyPartDescription();
            bodyPartDescription.restitution = section["restitution"];
            bodyPartDescription.density = section["density"];
            bodyPartDescription.shape = shape;
            bodyPartDescription.friction = section["friction"];
            
            Body = Game.level.World.createBody(bodyDescription, bodyPartDescription);


        }

        public override void PrePhysicsUpdate(GameTime gameTime)
        {
            var input = Game.InputHandler;

            float tempDir = input.GamePadState.ThumbSticks.Left.X;

            if (input.IsDown(Keys.D))
            {
                tempDir = 1.0f;
            }
            if (input.IsDown(Keys.A))
            {
                tempDir = -1.0f;
            }
            if (input.WasTriggered(Keys.S) || input.WasTriggered(Buttons.LeftTrigger))
            {
                Game.Events["playerButtonPress"].trigger(Name);
            }
            else if (input.WasReleased(Keys.S) || input.WasReleased(Buttons.LeftTrigger))
            {
                Game.Events["playerButtonRelease"].trigger(Name);
            }
#if false
            Body.ApplyTorque(tempDir * MaxTorque);
#endif
        }

        public override void Update(GameTime gameTime)
        {
#if false
            circlePos = Game.level.ConvertFromBox2D(Body.GetPosition());
#endif
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(circleTexture, Pos, null, Color.White, Body.transform.rotation.radians, new Vector2(Radius, Radius), 1.0f, SpriteEffects.None, 0.0f);
        }
    }

}

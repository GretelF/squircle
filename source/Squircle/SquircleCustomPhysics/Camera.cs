using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/** modified version of Khalid Abuhakmeh's post on stackoverflow.com:
 *  http://stackoverflow.com/a/723918
 *  (date: 2014-09-09)
**/


namespace Squircle
{
    public interface ICamera2D
    {
        /// <summary>
        /// Gets or sets the position of the camera
        /// </summary>
        /// <value>The position.</value>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the move speed of the camera.
        /// The camera will tween to its destination.
        /// </summary>
        /// <value>The move speed.</value>
        float MoveSpeed { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        /// <value>The rotation.</value>
        float Rotation { get; set; }

        /// <summary>
        /// Gets the origin of the viewport (accounts for Scale)
        /// </summary>        
        /// <value>The origin.</value>
        Vector2 Origin { get; }

        /// <summary>
        /// Gets or sets the scale of the Camera
        /// </summary>
        /// <value>The scale.</value>
        float Scale { get; set; }

        /// <summary>
        /// Gets the screen center (does not account for Scale)
        /// </summary>
        /// <value>The screen center.</value>
        Vector2 ScreenCenter { get; }

        /// <summary>
        /// Gets the transform that can be applied to 
        /// the SpriteBatch Class.
        /// </summary>
        /// <see cref="SpriteBatch"/>
        /// <value>The transform.</value>
        Matrix Transform { get; }

        /// <summary>
        /// Gets or sets the focus of the Camera.
        /// </summary>
        /// <seealso cref="IFocusable"/>
        /// <value>The focus.</value>
        Vector2 Focus { get; set; }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if the target is in view at the specified position; otherwise, <c>false</c>.
        /// </returns>
        bool IsInView(Vector2 position, Texture2D texture);
    }

    public class Camera2D : GameComponent, ICamera2D
    {
        private Vector2 _position;
        protected float _viewportHeight;
        protected float _viewportWidth;
        private Rectangle _focusBounds;
        private Rectangle _viewBounds;

        public Camera2D(Game game)
            : base(game)
        { }

        #region Properties

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public Vector2 ScreenCenter { get { return new Vector2(_viewportWidth / 2.0f, _viewportHeight / 2.0f); } }
        public Matrix Transform { get; set; }
        public Vector2 Focus { get; set; }
        public float MoveSpeed { get; set; }
        public float MaxMoveSpeed { get; set; }
        public Rectangle ViewBounds
        {
            get
            {
                return _viewBounds;
            }
            set
            {
                _viewBounds = value;

                // calculate _focusBounds
                var origin = new Vector2(_viewportWidth / 2, _viewportHeight / 2);
                var width = ViewBounds.Width - (int)_viewportWidth;
                var height = ViewBounds.Height - (int)_viewportHeight;
                _focusBounds = new Rectangle((int)origin.X, (int)origin.Y, width, height);
            }
        }



        #endregion

        /// <summary>
        /// Called when the GameComponent needs to be initialized. 
        /// </summary>
        public override void Initialize()
        {
            _viewportWidth = Game.GraphicsDevice.Viewport.Width;
            _viewportHeight = Game.GraphicsDevice.Viewport.Height;

            Scale = 1;
            MaxMoveSpeed = 1.25f;
            MoveSpeed = 1.25f;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Move the Camera to the position that it needs to go
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;


            Origin = ScreenCenter / Scale;

            var targetPos = CalculateTarget();

            //TODO: Test if dt should be multiplied after clamping
            _position.X += MathHelper.Clamp((targetPos.X - _position.X) * MoveSpeed * dt, -MaxMoveSpeed, MaxMoveSpeed);
            _position.Y += MathHelper.Clamp((targetPos.Y - _position.Y) * MoveSpeed * dt, -MaxMoveSpeed, MaxMoveSpeed);

            base.Update(gameTime);

            // Create the Transform used by any
            // spritebatch process
            Transform = Matrix.Identity *
                        Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                        Matrix.CreateScale(new Vector3(Scale, Scale, Scale));
        }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInView(Vector2 position, Texture2D texture)
        {
            // If the object is not within the horizontal bounds of the screen

            if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
                return false;

            // If the object is not within the vertical bounds of the screen
            if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
                return false;

            // In View
            return true;
        }

        private Vector2 CalculateTarget()
        {
            var target = new Vector2();

            target.X = MathHelper.Clamp(Focus.X, _focusBounds.Left, _focusBounds.Right);
            target.Y = MathHelper.Clamp(Focus.Y, _focusBounds.Top, _focusBounds.Bottom);

            return target;
        }
    }
}

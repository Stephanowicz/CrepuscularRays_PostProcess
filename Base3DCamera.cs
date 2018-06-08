using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrepuscularRays_PostProcess
{
    public class Base3DCamera : GameComponent, ICameraService
    {
        /// <summary>
        /// Position
        /// </summary>
        protected Vector3 position;
        /// <summary>
        /// Scale
        /// </summary>
        protected Vector3 scale;
        /// <summary>
        /// Rotation
        /// </summary>
        protected Quaternion rotation;
        /// <summary>
        /// World
        /// </summary>
        public Matrix world;
        #region I3DCamera Members

        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position
        {
            get
            { return position; }
            set
            { position = value; }
        }
        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 Scale
        {
            get
            { return scale; }
            set
            { scale = value; }
        }
        /// <summary>
        /// Rotation
        /// </summary>
        public Quaternion Rotation
        {
            get
            { return rotation; }
            set
            { rotation = value; }
        }
        /// <summary>
        /// World
        /// </summary>
        public Matrix World
        {
            get { return world; }
        }

        #endregion

        /// <summary>
        /// View
        /// </summary>
        protected Matrix view;
        /// <summary>
        /// View
        /// </summary>
        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        /// <summary>
        /// Projection
        /// </summary>
        protected Matrix projection;
        /// <summary>
        /// Projection
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
        }
        /// <summary>
        /// View port
        /// </summary>
        protected Viewport viewport;
        public Viewport Viewport
        {
            get { return viewport; }
            set { viewport = value; }
        }

        /// <summary>
        /// Frustum
        /// </summary>
        public BoundingFrustum Frustum
        {
            get
            {
                return new BoundingFrustum(Matrix.Multiply(View, Projection));
            }
        }

        /// <summary>
        /// View ports min depth
        /// </summary>
        protected float minDepth;
        /// <summary>
        /// Viewports max depth.
        /// </summary>
        protected float maxDepth;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        public Base3DCamera(Game game, float minDepth, float maxDepth)
            : base(game)
        {
            position = new Vector3(0, 0, 100);
            scale = Vector3.One;
            rotation = Quaternion.Identity;
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;

            game.Components.Add(this);
            game.Services.AddService(typeof(ICameraService), this);
        }
        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialize()
        {
            viewport = Game.GraphicsDevice.Viewport;
            viewport.MinDepth = minDepth;
            viewport.MaxDepth = maxDepth;
        }
        
        /// <summary>
        /// Method to update.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            world = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(Position);
            view = Matrix.Invert(world);

            //projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 3.0f, (float)Viewport.Width / (float)Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Viewport.Width / (float)Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth);
            base.Update(gameTime);
        }
        /// <summary>
        /// Method to translate object
        /// </summary>
        /// <param name="distance"></param>
        public void Translate(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Matrix.CreateFromQuaternion(rotation));
        }
        /// <summary>
        /// Method to rotate object
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(rotation));
            rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * rotation);
        }
    }
}

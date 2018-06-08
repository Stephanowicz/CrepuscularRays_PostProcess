using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrepuscularRays_PostProcess
{
    public class Base3DModel : DrawableGameComponent 
    {
        string modelAsset = string.Empty;
        string effectAsset = string.Empty;
        string textureAsset = string.Empty;

        Matrix world;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Orientation = Quaternion.Identity;

        public Vector3 LightPosition;

        Model thisMesh;
        Matrix[] transforms;
        Matrix meshWorld;
        Matrix meshWVP;

        Effect effect;

        public BoundingSphere BoundingSphere
        {
            get 
            {
                return thisMesh.Meshes[0].BoundingSphere.Transform(world);
            }
        }

        public ICameraService Camera
        {
            get { return (ICameraService)Game.Services.GetService(typeof(ICameraService)); }
        }

        public Base3DModel(Game game, string modelAsset, string effectAsset)
            : base(game)
        {
            this.modelAsset = modelAsset;
            this.effectAsset = effectAsset;
        }

        public Base3DModel(Game game, string modelAsset, string effectAsset, string textureAsset) 
            : base(game)
        {
            this.modelAsset = modelAsset;
            this.effectAsset = effectAsset;
            this.textureAsset = textureAsset;
        }

        protected override void LoadContent()
        {
            thisMesh = Game.Content.Load<Model>(modelAsset);
            transforms = new Matrix[thisMesh.Bones.Count];
            thisMesh.CopyAbsoluteBoneTransformsTo(transforms);

            effect = Game.Content.Load<Effect>(effectAsset);
        }

        public override void Update(GameTime gameTime)
        {
            world = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);

            //Rotate(Vector3.Normalize(Position), .01f);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Camera.Frustum.Contains(BoundingSphere) == ContainmentType.Disjoint)
                return;

            foreach (ModelMesh mesh in thisMesh.Meshes)
            {
                meshWorld = Matrix.Multiply(transforms[mesh.ParentBone.Index] , world);
                meshWVP = meshWorld * Camera.View * Camera.Projection;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    effect.Parameters["world"].SetValue(meshWorld);
                    effect.Parameters["wvp"].SetValue(meshWVP);
                    effect.Parameters["lightDirection"].SetValue(LightPosition - Position);

                    if (textureAsset == string.Empty)
                        effect.Parameters["textureMat"].SetValue(((BasicEffect)meshPart.Effect).Texture);
                    else
                        effect.Parameters["textureMat"].SetValue(Game.Content.Load<Texture2D>(textureAsset));

                    effect.CurrentTechnique.Passes[0].Apply();

                    Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    Game.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }
        }
        
        public void Translate(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Matrix.CreateFromQuaternion(Orientation));
        }
        
        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Orientation));
            Orientation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Orientation);
        }
    }
}

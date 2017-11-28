using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    class Projectile
    {
        public Model ball;
        public Vector3 position, direction;
        Camera camera;
        public Matrix[] boneTransforms;

        public Projectile(ContentManager content, Camera camera, Vector3 initialDirection, Vector3 initialPosition)
        {
            ball = content.Load<Model>("CannonBall");
            boneTransforms = new Matrix[ball.Bones.Count];
            this.camera = camera;
            position = initialPosition;
            direction = initialDirection;
        }

        public void Movement()
        {
            position += direction * Constants.CannonBallSpeed;
            direction.Y -= Constants.CannonBallDecay;
        }

        public void Draw()
        {
            ball.Root.Transform = Matrix.CreateScale(Constants.CannonBallScale) * Matrix.CreateTranslation(position);
            ball.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in ball.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.GetViewMatrix();
                    effect.Projection = camera.GetProjection();

                    Lighting.SetLight(effect);
                }
                mesh.Draw();
            }
        }
    }
}

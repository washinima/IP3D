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
    public class Tanque
    {
        Camera camera;
        Model tank;
        ModelBone turretBone, cannonBone, rightFrontWheelBone, leftFrontWheelBone, rightBackWheelBone, leftBackWheelBone, hatchBone;
        Matrix turretTransform, cannonTransform, rightFrontWheelTransform, leftFrontWheelTransform, rightBackWheelTransform, leftBackWheelTransform, hatchTransform;
        float turretAngle, cannonAngle;
        Matrix rotacao, translacao;
        float scale;

        private float[,] height;

        Matrix[] boneTransforms;

        public Tanque(ContentManager content, GraphicsDevice graphicsDevice, Camera camera)
        {
            this.camera = camera;
            tank = content.Load<Model>("tank");
            scale = 0.002f;
            translacao = Matrix.CreateTranslation(new Vector3(100f, 4f, 100f));
            rotacao = Matrix.Identity;


            rightFrontWheelBone = tank.Bones["r_front_wheel_geo"];
            leftFrontWheelBone = tank.Bones["l_front_wheel_geo"];
            rightBackWheelBone = tank.Bones["r_back_wheel_geo"];
            leftBackWheelBone = tank.Bones["l_back_wheel_geo"];
            hatchBone = tank.Bones["hatch_geo"];
            turretBone = tank.Bones["turret_geo"];
            cannonBone = tank.Bones["canon_geo"];

            turretAngle = 0.0f;
            cannonAngle = 0.0f;

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftBackWheelTransform = leftBackWheelBone.Transform;
            hatchTransform = hatchBone.Transform;

            boneTransforms = new Matrix[tank.Bones.Count];
        }

        public void LoadHeights(float[,] height)
        {
            this.height = height;
        }

        public void UpdateCameraHeight()
        {
            Vector3 topLeft, topRight, bottomLeft, bottomRight;
            float topLeftX, topLeftZ;
            float heightBottom, heightTop, heightFinal;

            float offset = Constants.CameraSurfaceOffset;

            Vector3 position = new Vector3(tank.Root.Transform.Translation.X, tank.Root.Transform.Translation.Y,tank.Root.Transform.Translation.Z);

            topLeftX = (float)Math.Floor(position.X);
            topLeftZ = (float)Math.Floor(position.Z);

            topLeft = new Vector3(topLeftX, height[(int)topLeftX, (int)topLeftZ], topLeftZ);
            topRight = new Vector3(topLeft.X + 1, height[(int)topLeftX + 1, (int)topLeftZ], topLeft.Z);
            bottomLeft = new Vector3(topLeft.X, height[(int)topLeftX, (int)topLeftZ + 1], topLeft.Z + 1);
            bottomRight = new Vector3(topLeft.X + 1, height[(int)topLeftX + 1, (int)topLeftZ + 1], topLeft.Z + 1);

            heightTop = (position.X - topLeft.X) * topRight.Y + (topRight.X - position.X) * topLeft.Y;
            heightBottom = (position.X - bottomLeft.X) * bottomRight.Y + (bottomRight.X - position.X) * bottomLeft.Y;
            heightFinal = (position.Z - topLeft.Z) * heightBottom + (bottomLeft.Z - position.Z) * heightTop;


            position.Y = heightFinal + offset;
        }

        public void Draw()
        {
            tank.Root.Transform = Matrix.CreateScale(scale) * rotacao * translacao;

            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;

            tank.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in tank.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.GetViewMatrix();
                    effect.Projection = camera.GetProjection();
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = Vector3.Normalize(new Vector3(255f, 255f, 255f));
                    effect.DirectionalLight0.Direction = new Vector3(-1f, -0.5f, 0f);
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                }
                mesh.Draw();
            }
        }
    }
}

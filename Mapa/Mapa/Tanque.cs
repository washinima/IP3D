using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        float scale, speed;
        int playerNum;
        Keys kForward, kRight, kLeft, kBackward;

        Matrix[] boneTransforms;

        public Tanque(ContentManager content, GraphicsDevice graphicsDevice, Camera camera, int playerNum)
        {
            this.playerNum = playerNum;
            this.camera = camera;
            tank = content.Load<Model>("tank");
            scale = Constants.TankScale;
            speed = Constants.TankMovSpeed;
            translacao = Matrix.CreateTranslation(new Vector3(100f, 4f, 100f));
            rotacao = Matrix.Identity;

            switch (playerNum)
            {
                case 1:
                    kForward = Keys.W;
                    kRight = Keys.D;
                    kLeft = Keys.A;
                    kBackward = Keys.S;
                    break;
                case 2:
                    kForward = Keys.Up;
                    kRight = Keys.Right;
                    kLeft = Keys.Left;
                    kBackward = Keys.Down;
                    break;
            }

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

        public void Update()
        {
            translacao = Matrix.CreateTranslation(Movement());


            tank.Root.Transform = Matrix.CreateScale(scale) * rotacao * translacao;
            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;
        }

        private Vector3 Movement()
        {
            Vector3 oldPosition = tank.Root.Transform.Translation;
            Vector3 position = tank.Root.Transform.Translation;

            if (Keyboard.GetState().IsKeyDown(kForward))
                position += speed * tankForward;
            if (Keyboard.GetState().IsKeyDown(kBackward))
                position -= speed * tankForward;

            if (Keyboard.GetState().IsKeyDown(kRight))
                position += speed * tankRight;
            if (Keyboard.GetState().IsKeyDown(kLeft))
                position -= speed * tankRight;

            if (position.X < 0 || position.X > height.GetLength(0) - 1)
                position.X = oldPosition.X;
            if (position.Z < 0 || position.Z > height.GetLength(1) - 1)
                position.Z = oldPosition.Z;

            return position;
        }

        public void Draw()
        {
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

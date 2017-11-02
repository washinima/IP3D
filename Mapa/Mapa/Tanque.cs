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
        float scale, speed, rotSpeed, yaw;
        int playerNum;
        Keys kForward, kRight, kLeft, kBackward;
        private Vector3 tankForward, tankRight, tankNormal, origin, direction;

        private NormalPosition[,] normalPositions;

        Matrix[] boneTransforms;

        public Tanque(ContentManager content, GraphicsDevice graphicsDevice, Camera camera, int playerNum)
        {
            this.playerNum = playerNum;
            this.camera = camera;
            tank = content.Load<Model>("tank");
            scale = Constants.TankScale;
            speed = Constants.TankMovSpeed;
            translacao = Matrix.CreateTranslation(new Vector3(100f, 4f, 100f));
            tank.Root.Transform = translacao;
            rotacao = Matrix.Identity;
            origin = Vector3.Forward;
            direction = origin;
            rotSpeed = 1f;
            //rotacao = Matrix.CreateRotationY(MathHelper.ToRadians(180));

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
            UpdateTankNormal();

            tank.Root.Transform = Matrix.CreateScale(scale) * rotacao * translacao;
            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;
        }

        private float UpdateTankHeight()
        {
            Vector3 topLeft, topRight, bottomLeft, bottomRight;
            float topLeftX, topLeftZ;
            float heightBottom, heightTop, heightFinal;

            float offset = Constants.CameraSurfaceOffset;

            Vector3 position = tank.Root.Transform.Translation;

            topLeftX = (float)Math.Floor(position.X);
            topLeftZ = (float)Math.Floor(position.Z);

            topLeft = new Vector3(topLeftX, normalPositions[(int)topLeftX, (int)topLeftZ].pos.Y, topLeftZ);
            topRight = new Vector3(topLeft.X + 1, normalPositions[(int)topLeftX + 1, (int)topLeftZ].pos.Y, topLeft.Z);
            bottomLeft = new Vector3(topLeft.X, normalPositions[(int)topLeftX, (int)topLeftZ + 1].pos.Y, topLeft.Z + 1);
            bottomRight = new Vector3(topLeft.X + 1, normalPositions[(int)topLeftX + 1, (int)topLeftZ + 1].pos.Y, topLeft.Z + 1);

            heightTop = (position.X - topLeft.X) * topRight.Y + (topRight.X - position.X) * topLeft.Y;
            heightBottom = (position.X - bottomLeft.X) * bottomRight.Y + (bottomRight.X - position.X) * bottomLeft.Y;
            heightFinal = (position.Z - topLeft.Z) * heightBottom + (bottomLeft.Z - position.Z) * heightTop;


             return heightFinal;
        }

        public void LoadMapNormalsPos(NormalPosition[,] a)
        {
            normalPositions = a;
        }

        private void UpdateTankNormal()
        {
            NormalPosition topLeft, topRight, bottomLeft, bottomRight;
            float topLeftX, topLeftZ;
            Vector3 normalBottom, normalTop, normalFinal;
            Vector3 position = tank.Root.Transform.Translation;

            topLeftX = (float)Math.Floor(position.X);
            topLeftZ = (float)Math.Floor(position.Z);
                 
            topLeft = normalPositions[(int)topLeftX, (int)topLeftZ];
            topRight = normalPositions[(int)topLeftX + 1, (int)topLeftZ];
            bottomLeft = normalPositions[(int)topLeftX, (int)topLeftZ + 1];
            bottomRight = normalPositions[(int)topLeftX + 1, (int)topLeftZ + 1];
            
            normalTop = Vector3.Normalize((position.X - topLeft.pos.X) * topRight.normal + (topRight.pos.X - position.X) * topLeft.normal);
            normalBottom = Vector3.Normalize((position.X - bottomLeft.pos.X) * bottomRight.normal + (bottomRight.pos.X - position.X) * bottomLeft.normal);
            normalFinal = (position.Z - topLeft.pos.Z) * normalBottom + (bottomLeft.pos.Z- position.Z) * normalTop;

            tankNormal = Vector3.Normalize(normalFinal);
            tankRight = Vector3.Cross(direction, tankNormal);
            tankForward = Vector3.Cross(tankNormal, tankRight);

            rotacao.Up = tankNormal;
            rotacao.Right = tankRight;
            rotacao.Forward = tankForward;
            rotacao *= Matrix.CreateFromAxisAngle(tankNormal, MathHelper.ToRadians(180));
            Console.WriteLine(tankNormal);
            Console.WriteLine(tankRight);
            Console.WriteLine(tankForward);
            Console.WriteLine("_______________________________________");
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
                yaw -= MathHelper.ToRadians(rotSpeed);
            if (Keyboard.GetState().IsKeyDown(kLeft))
                yaw += MathHelper.ToRadians(rotSpeed);

            Matrix rotationPlane = Matrix.CreateFromAxisAngle(tankNormal, yaw);
            direction = Vector3.Transform(origin, rotationPlane);

            if (position.X < 0 || position.X > normalPositions.GetLength(0) - 1)
                position.X = oldPosition.X;
            if (position.Z < 0 || position.Z > normalPositions.GetLength(1) - 1)
                position.Z = oldPosition.Z;

            position.Y = UpdateTankHeight();

            camera.PosicaoRotationTank(position, tankForward);

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
                    Lighting.SetLight(effect);
                }
                mesh.Draw();
            }
        }
    }
}

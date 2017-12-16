using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    public class Tanque
    {
        #region VariableSet1

        ContentManager content;
        Camera camera;
        Model tank;
        ModelBone turretBone, cannonBone, rightFrontWheelBone, leftFrontWheelBone, rightBackWheelBone, leftBackWheelBone, hatchBone, leftSteerBone, rightSteerBone;
        Matrix turretInitTransform, cannonInitTransform, rightFrontWheelInitTransform, leftFrontWheelInitTransform, rightBackWheelInitTransform, leftBackWheelInitTransform, hatchInitTransform, leftSteerBoneInitTransform, rightSteerBoneInitTransform;
        Matrix turretTransform, cannonTransform, rightFrontWheelTransform, leftFrontWheelTransform, rightBackWheelTransform, leftBackWheelTransform, hatchTransform, leftSteerBoneTransform, rightSteerBoneTransform;
        float turretAngle, cannonAngle;
        public Matrix rotacao, translacao;
        float yaw, wheelRotationPitch;
        int playerNum;
        Keys kForward, kRight, kLeft, kBackward, kShoot, kCannonUp, kCannonDown, kCannonRight, kCannonLeft;
        private Vector3 tankForward, tankRight, tankNormal, origin, direction, cannonDirection, turretForward;

        private Matrix tPos;

        private NormalPosition[,] normalPositions;

        Matrix[] boneTransforms;

        List<Projectile> projectiles;

        #endregion

        #region VariableSet2
        double shootTime;
        private Vector3 vel;

        private float _r;
        private Vector3 oldPosition;

        public float Raio
        {
            get{ return _r; }
        }

        public Vector3 Position
        {
            get { return tPos.Translation; }
            set { tPos.Translation = value; }
        }

        public Vector3 OldPosition
        {
            get { return oldPosition; }
        }

        public List<Projectile> Projectiles
        {
            get { return projectiles; }
        }

        public List<Tanque> tanques;
        SistemaDeParticulas sistemaDeParticulas;
        Random random;
        #endregion

        public Tanque(ContentManager content, Camera camera, int playerNum, Vector3 posicaoInicial, List<Tanque> tanques)
        {
            this.tanques = tanques;
            sistemaDeParticulas = new SistemaDeParticulas();
            _r = 0.6f;
            shootTime = 1f;
            vel = Vector3.Zero;
            random = new Random();

            this.content = content;
            this.playerNum = playerNum;
            this.camera = camera;
            tank = content.Load<Model>("tank");
            translacao = Matrix.CreateTranslation(posicaoInicial);
            tPos = translacao;
            rotacao = Matrix.Identity;
            origin = Vector3.Forward;
            direction = origin;
            cannonDirection = origin;
            turretForward = origin;

            wheelRotationPitch = 0f;

            switch (playerNum)
            {
                case 1:
                    kForward = Keys.W;
                    kRight = Keys.D;
                    kLeft = Keys.A;
                    kBackward = Keys.S;
                    kShoot = Keys.E;
                    kCannonUp = Keys.T;
                    kCannonDown = Keys.G;
                    kCannonLeft = Keys.F;
                    kCannonRight = Keys.H;
                    break;
                case 2:
                    kForward = Keys.I;
                    kRight = Keys.L;
                    kLeft = Keys.J;
                    kBackward = Keys.K;
                    kShoot = Keys.O;
                    kCannonUp = Keys.Up;
                    kCannonDown = Keys.Down;
                    kCannonLeft = Keys.Left;
                    kCannonRight = Keys.Right;
                    break;
            }

            #region ModelBones
            rightFrontWheelBone = tank.Bones["r_front_wheel_geo"];
            leftFrontWheelBone = tank.Bones["l_front_wheel_geo"];
            rightBackWheelBone = tank.Bones["r_back_wheel_geo"];
            leftBackWheelBone = tank.Bones["l_back_wheel_geo"];
            leftSteerBone = tank.Bones["l_steer_geo"];
            rightSteerBone = tank.Bones["r_steer_geo"];
            hatchBone = tank.Bones["hatch_geo"];
            turretBone = tank.Bones["turret_geo"];
            cannonBone = tank.Bones["canon_geo"];

            turretAngle = 0.0f;
            cannonAngle = 0.0f;

            turretInitTransform = turretBone.Transform;
            turretTransform = turretBone.Transform;
            cannonInitTransform = cannonBone.Transform;
            cannonTransform = cannonBone.Transform;
            leftSteerBoneInitTransform = leftSteerBone.Transform;
            leftSteerBoneTransform = leftSteerBone.Transform;
            rightSteerBoneInitTransform = rightSteerBone.Transform;
            rightSteerBoneTransform = rightSteerBone.Transform;
            rightFrontWheelInitTransform = rightFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftFrontWheelInitTransform = leftFrontWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightBackWheelInitTransform = rightBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftBackWheelInitTransform = leftBackWheelBone.Transform;
            leftBackWheelTransform = leftBackWheelBone.Transform;
            hatchInitTransform = hatchBone.Transform;
            hatchTransform = hatchBone.Transform;

            boneTransforms = new Matrix[tank.Bones.Count];
            #endregion

            projectiles = new List<Projectile>();
        }

        public void Update()
        {
            if (playerNum != 0) //Controllable Tank
            {
                shootTime += 1f / 60f;
                translacao = Matrix.CreateTranslation(Movement());

                UpdateTankNormal();
                MoveCannonAndTurret();

                tPos = Matrix.CreateScale(Constants.TankScale) * rotacao * translacao;

                if (Keyboard.GetState().IsKeyDown(kShoot) && shootTime >= Constants.ShootCooldown)
                    Shoot();
                for (int i = projectiles.Count - 1; i >= 0; i--)
                {
                    if (projectiles[i].IsDead(normalPositions))
                        projectiles.Remove(projectiles[i]);
                    else
                        projectiles[i].Movement();
                }
                sistemaDeParticulas.Update(this);
            }
            
            else //AI Tank
            {
                if (true)//Constants.LengthOfVector3(tanques[1].Position - Position) > 5f)
                {
                    Vector3 target = WanderBehaviour();
                    SeekBehaviour(target);
                    translacao.Translation += vel;
                    sistemaDeParticulas.AddDust(this, true);
                    direction = Vector3.Normalize(vel);
                }

                translacao = Matrix.CreateTranslation(new Vector3(translacao.Translation.X, UpdateTankHeight(), translacao.Translation.Z));
                UpdateTankNormal();
                
                tPos = Matrix.CreateScale(Constants.TankScale) * rotacao * translacao;
                sistemaDeParticulas.Update(this);
            }
            
        }

        private float UpdateTankHeight()
        {
            Vector3 topLeft, topRight, bottomLeft, bottomRight;
            float topLeftX, topLeftZ;
            float heightBottom, heightTop, heightFinal;

            Vector3 position = tPos.Translation;

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
            Vector3 position = tPos.Translation;

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
        }

        private Vector3 Movement()
        {
            oldPosition = tPos.Translation;
            Vector3 position = tPos.Translation;

            if (Keyboard.GetState().IsKeyDown(kForward))
            {
                sistemaDeParticulas.AddDust(this, true);
                position += Constants.TankMovSpeed * tankForward;
                MoveWheelsForward(true);
            }
            if (Keyboard.GetState().IsKeyDown(kBackward))
            {
                position -= Constants.TankMovSpeed * tankForward;
                MoveWheelsForward(false);
                sistemaDeParticulas.AddDust(this, false);
            }

            if (Keyboard.GetState().IsKeyDown(kRight))
            {
                TurnFrontWheels(true);
                yaw -= MathHelper.ToRadians(Constants.TankRotSpeed);
            }
            else if (Keyboard.GetState().IsKeyDown(kLeft))
            {
                TurnFrontWheels(false);
                yaw += MathHelper.ToRadians(Constants.TankRotSpeed);
            }
            else
            {
                leftSteerBoneTransform = leftSteerBoneInitTransform;
                rightSteerBoneTransform = rightSteerBoneInitTransform;
            }

            Matrix rotationPlane = Matrix.CreateFromAxisAngle(tankNormal, yaw);
            direction = Vector3.Transform(origin, rotationPlane);

            if (position.X - Raio < 0 || position.X + Raio > normalPositions.GetLength(0) - 1)
                position.X = oldPosition.X;
            if (position.Z - Raio < 0 || position.Z + Raio > normalPositions.GetLength(1) - 1)
                position.Z = oldPosition.Z;

            position.Y = UpdateTankHeight();

            camera.PosicaoRotationTank(position, tankForward);

            return position;
        }

        private void MoveCannonAndTurret()
        {
            if (Keyboard.GetState().IsKeyDown(kCannonUp))
            {
                cannonAngle -= Constants.CannonRotSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(kCannonDown))
            {
                cannonAngle += Constants.CannonRotSpeed;
            }

            if (cannonAngle < -1.2f)
                cannonAngle = -1.2f;
            if (cannonAngle > 0.5f)
                cannonAngle = 0.5f;

            if (Keyboard.GetState().IsKeyDown(kCannonRight))
            {
                turretAngle -= Constants.CannonRotSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(kCannonLeft))
            {
                turretAngle += Constants.CannonRotSpeed;
            }

            turretTransform = Matrix.CreateRotationY(turretAngle) * turretInitTransform;
            cannonTransform = Matrix.CreateRotationX(cannonAngle) * cannonInitTransform;

            cannonDirection = -Vector3.Transform(origin, Matrix.CreateFromYawPitchRoll(turretAngle, cannonAngle, 0.0f) * rotacao);
            turretForward = Vector3.Transform(origin, Matrix.CreateFromAxisAngle(tankNormal, turretAngle) * rotacao) ;
        }

        private void Shoot()
        {
            shootTime = 0.0f;
            projectiles.Add(new Projectile(content, camera, cannonDirection, (tPos.Translation - tankForward * 0.1f) + tankNormal * 0.65f + turretForward * -0.25f));
        }

        private void MoveWheelsForward(bool isGoingForward)
        {
            if (isGoingForward)
                wheelRotationPitch += Constants.TankWheelSpinSpeed;
            else
                wheelRotationPitch -= Constants.TankWheelSpinSpeed;

            rightBackWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * rightBackWheelInitTransform;
            rightFrontWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * rightFrontWheelInitTransform;
            leftBackWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * leftBackWheelInitTransform;
            leftFrontWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * leftFrontWheelInitTransform;
        }

        private void TurnFrontWheels(bool isTurningRight)
        {
            float wheelTurn;
            if (isTurningRight)
                wheelTurn = MathHelper.ToRadians(-30f);
            else
                wheelTurn = MathHelper.ToRadians(30f);

            wheelRotationPitch += Constants.TankWheelSpinSpeed;

            leftSteerBoneTransform = Matrix.CreateRotationY(wheelTurn) * leftSteerBoneInitTransform;
            rightSteerBoneTransform = Matrix.CreateRotationY(wheelTurn) * rightSteerBoneInitTransform;

            rightBackWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * rightBackWheelInitTransform;
            rightFrontWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * rightFrontWheelInitTransform;
            leftBackWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * leftBackWheelInitTransform;
            leftFrontWheelTransform = Matrix.CreateRotationX(wheelRotationPitch) * leftFrontWheelInitTransform;
        }

        private Vector3 WanderBehaviour()
        {
            Vector3 target, centro;
            centro = Position + direction * 26f;
            float raio = 25f;
            float angle = MathHelper.ToRadians(random.Next(0, 359));
            Console.WriteLine(angle + ", " + centro);

            target = new Vector3(centro.X + raio * (float)Math.Cos(angle), 0.0f, centro.Z + raio * -(float)Math.Sin(angle));

            return target;
        }

        private void SeekBehaviour(Vector3 target)
        {
            Vector3 vSeek = Vector3.Normalize(target - Position);
            Vector3 acceleration = Vector3.Normalize(vSeek - vel) * 0.001f;
            vel = Vector3.Normalize(vel + acceleration) * Constants.TankMovSpeed;
        }

        public void Draw(GraphicsDevice device)
        {
            rightBackWheelBone.Transform = rightBackWheelTransform;
            rightFrontWheelBone.Transform = rightFrontWheelTransform;
            leftBackWheelBone.Transform = leftBackWheelTransform;
            leftFrontWheelBone.Transform = leftFrontWheelTransform;
            leftSteerBone.Transform = leftSteerBoneTransform;
            rightSteerBone.Transform = rightSteerBoneTransform;
            turretBone.Transform = turretTransform;
            cannonBone.Transform = cannonTransform;
            tank.Root.Transform = tPos;
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

            foreach (Projectile p in projectiles)
                p.Draw();

            sistemaDeParticulas.Draw(device, camera);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    public class Projectile
    {
        public Model ball;
        public Vector3 position, direction;
        Camera camera;
        public Matrix[] boneTransforms;

        public Vector3 _oldPos;

        private float _raio;
        private bool _isDead;

        public float Raio
        {
            get { return _raio; }
        }

        public bool Dead
        {
            get { return _isDead; }
            set { _isDead = value; }
        }

        public Projectile(ContentManager content, Camera camera, Vector3 initialDirection, Vector3 initialPosition)
        {
            ball = content.Load<Model>("CannonBall");
            boneTransforms = new Matrix[ball.Bones.Count];
            this.camera = camera;
            position = initialPosition;
            direction = initialDirection;
            _raio = 0.01f;
        }

        public void Movement()
        {
            _oldPos = position;
                
            position += direction * Constants.CannonBallSpeed;
            direction.Y -= Constants.CannonBallDecay;

        }

        public bool IsDead(NormalPosition[,] normalPositions)
        {
            if (position.Y < MapHeight(normalPositions))
            {
                _isDead = true;
                MapDestruction(normalPositions);
                Constants.isDestroyed = true;
            }
            else
                _isDead = false;

            return _isDead;
        }

        private void MapDestruction(NormalPosition[,] normalPositions)
        {
            float topLeftX = (float)Math.Floor(position.X);
            float topLeftZ = (float)Math.Floor(position.Z);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((int) topLeftX + i <= Constants.MapWidth && (int) topLeftX + i >= 0)
                    {
                        if( (int)topLeftZ + j <= Constants.MapHeight && (int)topLeftZ + j >= 0)
                        {
                            if (i == 0 && j == 0)
                            {
                                normalPositions[(int) topLeftX + i, (int) topLeftZ + j].pos.Y -= Constants.Destruction;
                            }
                            else
                            {
                                normalPositions[(int)topLeftX + i, (int)topLeftZ + j].pos.Y -= Constants.Destruction * 0.5f;
                            }
                        }
                    }   

                }
            }

        }


        private float MapHeight(NormalPosition[,] normalPositions )
        {
            Vector3 topLeft, topRight, bottomLeft, bottomRight;
            float topLeftX, topLeftZ;
            float heightBottom, heightTop, heightFinal;

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

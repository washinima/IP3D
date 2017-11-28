using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mapa
{
    public class Camera
    {
        MouseState mouseState, oldMouseState;
        Vector3 origin, originSide, direction, directionSide, normal, normalOrigin;
        Vector3 position;
        Matrix rotation;
        float yaw;
        float pitch;
        NormalPosition[,] heightNormalPositions;
        int cameraOption;
        private GameWindow window;

        //TANK
        Vector3 posicaoTank;
        Vector3 tankFoward;

        public Camera(GameWindow window)
        {
            position = Constants.CameraInitialPosition;
            origin = Vector3.Right;
            originSide = Vector3.Backward;
            direction = origin;
            directionSide = originSide;
            normalOrigin = Vector3.Up;
            mouseState = Mouse.GetState();
            yaw = 0;
            pitch = 0;
            cameraOption = 1;
            this.window = window;
        }

        public void LoadHeights(NormalPosition[,] heightNormal)
        {
            heightNormalPositions = heightNormal;
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                cameraOption = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                cameraOption = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
                cameraOption = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
                cameraOption = 4;

            switch (cameraOption)
            {
                case 1:
                    SurfaceFollowUpdate();
                    break;
                case 2:
                    FreeLookUpdate();
                    break;
                case 3:
                    rotation = Matrix.Identity;
                    yaw = 0f;
                    pitch = 0f;
                    direction = origin;
                    directionSide = originSide;
                    normal = normalOrigin;
                    TankFollowUpdate(true);
                    break;
                case 4:
                    rotation = Matrix.Identity;
                    yaw = 0f;
                    pitch = 0f;
                    direction = origin;
                    directionSide = originSide;
                    normal = normalOrigin;
                    TankFollowUpdate(false);
                    break;
            }
        }

        public void UpdateCameraHeight()
        {
            Vector3 topLeft, topRight, bottomLeft, bottomRight;
            float topLeftX, topLeftZ;
            float heightBottom, heightTop, heightFinal;
            topLeftX = (float)Math.Floor(position.X);
            topLeftZ = (float)Math.Floor(position.Z);

            topLeft = new Vector3(topLeftX, heightNormalPositions[(int)topLeftX, (int)topLeftZ].pos.Y, topLeftZ);
            topRight = new Vector3(topLeft.X + 1, heightNormalPositions[(int)topLeftX + 1, (int)topLeftZ].pos.Y, topLeft.Z);
            bottomLeft = new Vector3(topLeft.X, heightNormalPositions[(int)topLeftX, (int)topLeftZ + 1].pos.Y, topLeft.Z + 1);
            bottomRight = new Vector3(topLeft.X + 1, heightNormalPositions[(int)topLeftX + 1, (int)topLeftZ + 1].pos.Y, topLeft.Z + 1);

            heightTop = (position.X - topLeft.X) * topRight.Y + (topRight.X - position.X) * topLeft.Y;
            heightBottom = (position.X - bottomLeft.X) * bottomRight.Y + (bottomRight.X - position.X) * bottomLeft.Y;
            heightFinal = (position.Z - topLeft.Z) * heightBottom + (bottomLeft.Z - position.Z) * heightTop;


            position.Y = heightFinal + Constants.CameraSurfaceOffset;
        }

        private void TankFollowUpdate(bool isFromBack)
        {
            position = posicaoTank;
            if (isFromBack)
                position -= Vector3.Normalize(tankFoward) * 3;
            else
                position += Vector3.Normalize(tankFoward) * 3;
            position.Y += 1f;
            direction = posicaoTank - position;
        }

        public void PosicaoRotationTank(Vector3 posicaoTank, Vector3 tankFoward)
        {
            this.posicaoTank = posicaoTank;
            this.tankFoward = tankFoward;
        }

        private void FreeLookUpdate()
        {
            BaseMovement();

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad7))
            {
                position += Constants.CameraMovementSpeed * normal;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                position -= Constants.CameraMovementSpeed * normal;
            }
        }

        private void SurfaceFollowUpdate()
        {
            if (IsOutOfBoundaries())
                MoveToMap();

            Vector3 oldPosition = position;
            BaseMovement();

            if (position.X < 0 || position.X > heightNormalPositions.GetLength(0) - 1)
                position.X = oldPosition.X;
            if (position.Z < 0 || position.Z > heightNormalPositions.GetLength(1) - 1)
                position.Z = oldPosition.Z;

            UpdateCameraHeight();
        }

        private void BaseMovement()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                position -= Constants.CameraMovementSpeed * directionSide;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                position += Constants.CameraMovementSpeed * directionSide;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
            {
                position += Constants.CameraMovementSpeed * direction;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5))
            {
                position -= Constants.CameraMovementSpeed * direction;
            }

            ResetMouse();

            /*mouseState = Mouse.GetState();
            yaw = MathHelper.ToRadians(-mouseState.X * speed);
            pitch = MathHelper.ToRadians(-mouseState.Y * speed);*/
            rotation = Matrix.CreateFromYawPitchRoll(yaw, 0.0f, pitch);
            direction = Vector3.Transform(origin, rotation);
            directionSide = Vector3.Transform(originSide, rotation);
            normal = Vector3.Transform(normalOrigin, rotation);
        }

        private void ResetMouse()
        {
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (mouseState.X > window.ClientBounds.Width || mouseState.X < 0)
            {
                Mouse.SetPosition(window.ClientBounds.Width / 2, mouseState.Y);
                mouseState = Mouse.GetState();
                oldMouseState = mouseState;
            }

            if (mouseState.Y > window.ClientBounds.Height || mouseState.Y < 0)
            {
                Mouse.SetPosition(mouseState.X, window.ClientBounds.Height / 2);
                mouseState = Mouse.GetState();
                oldMouseState = mouseState;
            }

            if (mouseState != oldMouseState)
            {
                yaw += MathHelper.ToRadians(-(mouseState.X - oldMouseState.X) * Constants.CameraSensitivity);
                pitch += MathHelper.ToRadians(-(mouseState.Y - oldMouseState.Y) * Constants.CameraSensitivity);
            }

            /*oldMouseState = mouseState;
            mouseState = Mouse.GetState();
            if (mouseState != oldMouseState)
            {
                yaw += MathHelper.ToRadians(-(mouseState.X - oldMouseState.X) * Constants.CameraSensitivity);
                pitch += MathHelper.ToRadians(-(mouseState.Y - oldMouseState.Y) * Constants.CameraSensitivity);

                Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);

                mouseState = Mouse.GetState();
                oldMouseState = mouseState;
            }*/
        }

        private bool IsOutOfBoundaries()
        {
            if (position.X < heightNormalPositions.GetLength(0) - 1 && position.X > 0 && position.Z < heightNormalPositions.GetLength(1) - 1 && position.Z > 0)
                return false;
            return true;
        }

        private void MoveToMap()
        {
            position.Normalize();
            position = new Vector3(Math.Abs(position.X), position.Y, Math.Abs(position.Z));
            position = position * (heightNormalPositions.GetLength(0) - 2);
        }

        public Vector3 GetDirection() => direction;

        public Vector3 GetNormal() => normal;

        public Vector3 GetLookAt() => position + direction;

        public Vector3 GetPosition() => position;

        public Matrix GetViewMatrix() => Matrix.CreateLookAt(position, position + direction, normal);

        public Matrix GetProjection() => Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60.0f), (float)window.ClientBounds.Width / (float)window.ClientBounds.Height, 0.0001f, 1000.0f);
    }
}


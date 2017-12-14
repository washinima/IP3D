using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Mapa
{
    public static class Constants
    {
        //----------------ECRÃ----------------//
        public static Color ScreenColor = Color.CornflowerBlue; //new Color(160, 40, 0);

        //----------------CÂMARA----------------//
        public static float CameraSurfaceOffset = 1.5f;
        public static float CameraMovementSpeed = 0.2f;
        public static float CameraSensitivity = 0.2f;
        public static Vector3 CameraInitialPosition = new Vector3(100f, 50f, 100f);


        //----------------MAPA----------------//
        public static float MapHeightScale = 0.025f;

        //----------------TANQUE----------------//
        public static float TankScale = 0.002f;
        public static float TankMovSpeed = 0.03f;
        public static float TankRotSpeed = 2f;
        public static float TankWheelSpinSpeed = 0.06f;
        public static float CannonRotSpeed = 0.03f;
        public static float CannonBallSpeed = 0.6f;
        public static float CannonBallDecay = 0.02f;
        public static float CannonBallScale = 0.3f;
        public static float ShootCooldown = 1f;


        public static double LengthOfVector3(Vector3 v)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2) + Math.Pow(v.Z, 2)));
        }
    }
}
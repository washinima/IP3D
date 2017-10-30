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

        //----------------CAMERA----------------//
        public static float CameraSurfaceOffset = 1.5f;
        public static float CameraMovementSpeed = 0.2f;
        public static float CameraSensitivity = 0.2f;
        public static Vector3 CameraInitialPosition = new Vector3(100f, 50f, 100f);


        //----------------MAPA----------------//
        public static float MapHeightScale = 0.03f;
    }
}
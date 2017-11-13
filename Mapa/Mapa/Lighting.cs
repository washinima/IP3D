using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    public static class Lighting
    {
        public static Vector3 Color = new Vector3(1f, 1f, 1f);
        public static Vector3 Direction = new Vector3(1f, -0.5f, 0f);
        public static Vector3 Ambient = Color / 10f;

        public static void SetLight(BasicEffect effect)
        {
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = Color;
            effect.DirectionalLight0.Direction = Direction;
            effect.AmbientLightColor = Ambient;
            effect.DirectionalLight0.SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
}

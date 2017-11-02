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
        private static Vector3 Color = Vector3.Normalize(new Vector3(256, 200, 90));
        private static Vector3 Direction = new Vector3(-1f, -0.5f, 0f);
        private static Vector3 Ambient = Color / 5f;

        public static void SetLight(BasicEffect effect)
        {
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = Color;
            effect.DirectionalLight0.Direction = Direction;
            effect.AmbientLightColor = Ambient;
            effect.DirectionalLight0.SpecularColor = new Vector3(1f, 1f, 1f);
        }
    }
}

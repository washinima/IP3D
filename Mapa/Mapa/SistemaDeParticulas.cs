using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mapa
{
    public class SistemaDeParticulas
    {
        List<ParticleDust> dust;
        BasicEffect effect;
        Random random;

        public SistemaDeParticulas()
        {
            dust = new List<ParticleDust>();

            random = new Random();
        }

        public void Update()
        {
            for (int i = dust.Count - 1; i >= 0; i--)
            {
                if (dust[i].duration > Constants.DustMaxDuration)
                    dust.Remove(dust[i]);
                else
                    dust[i].Life();
            }
        }

        public void AddDust(Tanque tanque, bool goingForward)
        {
            Color brown = new Color(68, 50, 33)
            {
                A = 10
            };

            if (goingForward)
            {
                dust.Add(new ParticleDust((tanque.Position + tanque.rotacao.Forward * 0.6f + tanque.rotacao.Right * 0.4f) + (new Vector3(random.Next(-10, 10) * tanque.rotacao.Left.X, 0f, random.Next(-10, 10) * tanque.rotacao.Left.Z)) / 50f, tanque.rotacao.Forward + tanque.rotacao.Up * 0.3f, Constants.DustTrailSize, 1f, brown));
                dust.Add(new ParticleDust((tanque.Position + tanque.rotacao.Forward * 0.6f + tanque.rotacao.Left * 0.4f) + (new Vector3(random.Next(-10, 10) * tanque.rotacao.Left.X, 0f, random.Next(-10, 10) * tanque.rotacao.Left.Z)) / 50f, tanque.rotacao.Forward + tanque.rotacao.Up * 0.3f, Constants.DustTrailSize, 1f, brown));
            }
            else
            {
                dust.Add(new ParticleDust((tanque.Position + tanque.rotacao.Forward * 0.6f + tanque.rotacao.Right * 0.4f) + (new Vector3(random.Next(-10, 10) * tanque.rotacao.Left.X, 0f, random.Next(-10, 10) * tanque.rotacao.Left.Z)) / 50f, tanque.rotacao.Backward + tanque.rotacao.Up * 0.3f, Constants.DustTrailSize, 1f, brown));
                dust.Add(new ParticleDust((tanque.Position + tanque.rotacao.Forward * 0.6f + tanque.rotacao.Left * 0.4f) + (new Vector3(random.Next(-10, 10) * tanque.rotacao.Left.X, 0f, random.Next(-10, 10) * tanque.rotacao.Left.Z)) / 50f, tanque.rotacao.Backward + tanque.rotacao.Up * 0.3f, Constants.DustTrailSize, 1f, brown));
            }
        }

        public void DirtExplosion(Vector3 position)
        {
            Color brown = new Color(68, 50, 33)
            {
                A = 10
            };

            for (int i = 0; i < 60; i++)
                dust.Add(new ParticleDust(position, new Vector3(0.0f, random.Next(10, 30) / 10f, 0.0f) + new Vector3(random.Next(-10, 10), 0.0f, random.Next(-10, 10)) / 10f, Constants.DustExplosionSize, 5f, brown));
        }

        public void TankExplosion(Vector3 position)
        {
            Color brown = new Color(68, 50, 33)
            {
                A = 10
            };

            Color red = new Color(255, 0, 0)
            {
                A = 10
            };

            for (int i = 0; i < 200; i++)
                if( i < 101)
                    dust.Add(new ParticleDust(position, new Vector3(0.0f, random.Next(10, 30) / 10f, 0.0f) + new Vector3(random.Next(-10, 10), 0.0f, random.Next(-10, 10)) / 10f, Constants.DustExplosionSize, 5f, red));
                else
                {
                    dust.Add(new ParticleDust(position, new Vector3(0.0f, random.Next(10, 30) / 10f, 0.0f) + new Vector3(random.Next(-10, 10), 0.0f, random.Next(-10, 10)) / 10f, Constants.DustExplosionSize, 5f, brown));
                }
        }

        public void Draw(GraphicsDevice device, Camera camera)
        {
            effect = new BasicEffect(device)
            {
                VertexColorEnabled = true,
                View = camera.GetViewMatrix(),
                Projection = camera.GetProjection()
            };
            Lighting.SetLight(effect);

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ParticleDust p in dust)
            {
                device.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 24, p.cubeIndexes, 0, 3);
                device.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 24, p.cubeIndexes, 5, 2);
                device.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 24, p.cubeIndexes, 9, 2);
                device.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 24, p.cubeIndexes, 13, 2);
                device.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 24, p.cubeIndexes, 17, 2);
                device.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 24, p.cubeIndexes, 21, 3);
            }
        }
    }
}

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

        public void Update(Tanque tanque)
        {
            dust.Add(new ParticleDust(tanque.Position, tanque.rotacao.Forward + tanque.rotacao.Up, random));
            //dust.Add(new ParticleDust(tanque.Position, tanque.rotacao.Backward, random));
            for (int i = dust.Count - 1; i >= 0; i--)
            {
                if (dust[i].position.Y <= 0f)
                    dust.Remove(dust[i]);
                else
                    dust[i].Life();
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

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ParticleDust p in dust)
            {
                device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 8, p.cubeIndexes, 0, 3);
                device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 8, p.cubeIndexes, 5, 8);
                device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, p.cubeVertexes, 0, 8, p.cubeIndexes, 15, 3);
            }
        }
    }
}

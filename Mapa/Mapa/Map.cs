using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    public struct NormalPosition
    {
        public Vector3 normal;
        public Vector3 pos;
    }

    public class Map
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        BasicEffect effect;
        Matrix worldMatrix;
        int w, h;
        Texture2D alturas, textura;
        float scale;
        VertexPositionNormalTexture[] vertexes;
        ushort[] indexes;
        Camera camera;

        public NormalPosition[,] normalPosition;

        public Map(ContentManager content, GraphicsDevice graphicsDevice, Camera camera)
        {
            worldMatrix = Matrix.Identity;
            alturas = content.Load<Texture2D>("lh3d1");
            textura = content.Load<Texture2D>("grass");
            this.camera = camera;
            effect = new BasicEffect(graphicsDevice)
            {
                View = camera.GetViewMatrix(),
                Projection = camera.GetProjection(),
                TextureEnabled = true,
                Texture = textura
            };
            Lighting.SetLight(effect);

            scale = Constants.MapHeightScale;

            effect.Texture = textura;
            w = alturas.Width;
            h = alturas.Height;

            normalPosition = new NormalPosition[w, h];

            CreateMap(graphicsDevice);
        }

        public void CreateMap(GraphicsDevice graphicsDevice)
        {
            Color[] texels = new Color[w * h];
            vertexes = new VertexPositionNormalTexture[w * h];
            alturas.GetData<Color>(texels);

            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    float y = (texels[z * w + x].R * scale);
                    vertexes[z * w + x] = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.Up, new Vector2(x % 2, z % 2));
                }
            }
            camera.LoadHeights(normalPosition);
            CalculateNormals();

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertexes.Length, BufferUsage.None);
            vertexBuffer.SetData(vertexes);

            indexes = new ushort[h * 2 * (w - 1)];

            for (int ix = 0; ix < w - 1; ix++)
            {
                for (int iz = 0; iz < h; iz++)
                {
                    indexes[2 * iz + 0 + ix * 2 * h] = (ushort)(iz * w + ix);
                    indexes[2 * iz + 1 + ix * 2 * h] = (ushort)(iz * w + 1 + ix);
                }
            }

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indexes.Length, BufferUsage.None);
            indexBuffer.SetData(indexes);
        }

        private void CalculateNormals()
        {
            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    List<Vector3> normais = new List<Vector3>();

                    //Linha em cima
                    if (z == 0)
                    {
                        if (x == 0)
                        {
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        }

                        else if (x == w - 1)
                        {
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position)));
                        }

                        else
                        {
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        }
                    }

                    //Linha em baixo
                    else if (z == h - 1)
                    {
                        if (x == 0)
                        {
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position)));
                        }

                        else if (x == w - 1)
                        {
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + x].Position - vertexes[(z - 1) * w + x].Position), Vector3.Normalize(vertexes[z * w + x].Position - vertexes[(z - 1) * w + (x - 1)].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + x].Position - vertexes[(z - 1) * w + (x - 1)].Position), Vector3.Normalize(vertexes[z * w + x].Position - vertexes[z * w + (x - 1)].Position)));
                        }

                        else
                        {
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                            normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        }
                    }

                    //Coluna da esquerda
                    //z nunca vai ser 0 ou h - 1 nesta parte
                    else if (x == 0)
                    {
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position)));
                    }

                    else if (x == w - 1)
                    {
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position)));
                    }

                    //Todos os vértices no meio
                    else
                    {
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position)));
                        normais.Add(Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position)));
                    }

                    Vector3 normal = Vector3.Zero;
                    foreach (Vector3 v in normais)
                        normal += v;
                    normal.Normalize();

                    vertexes[z * w + x].Normal = normal;
                    normalPosition[x, z].normal = normal;
                    normalPosition[x, z].pos = vertexes[z * w + x].Position;
                }
            }
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            effect.World = worldMatrix;
            effect.View = camera.GetViewMatrix();
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            for (int i = 0; i < w - 1; i++)
            {
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, i * h * 2, 2 * h - 2);
            }
        }
    }
}
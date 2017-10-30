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

        private NormalPosition[,] normalPosition;

        public Map(ContentManager content, GraphicsDevice graphicsDevice, Camera camera)
        {
            worldMatrix = Matrix.Identity;

            this.camera = camera;
            effect = new BasicEffect(graphicsDevice)
            {
                View = camera.GetViewMatrix(),
                Projection = camera.GetProjection(),
                TextureEnabled = true,
                LightingEnabled = true,
                VertexColorEnabled = false
            };
            //effect.EnableDefaultLighting();
            effect.DirectionalLight0.DiffuseColor = Vector3.Normalize(new Vector3(255f, 255f, 255f));
            effect.DirectionalLight0.Direction = new Vector3(-1f, -0.5f, 0f);
            effect.DirectionalLight1.DiffuseColor = Vector3.Normalize(new Vector3(255f, 0f, 0f));
            effect.DirectionalLight1.Direction = new Vector3(-1f, -0.5f, 0f);
            effect.DirectionalLight1.Enabled = false;
            //effect.DirectionalLight0.SpecularColor = new Vector3(0, 0.1f, 0);
            effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
            //effect.EmissiveColor = new Vector3(1f, 1f, 1f);

            scale = Constants.MapHeightScale;

            alturas = content.Load<Texture2D>("lh3d1");
            textura = content.Load<Texture2D>("grass");
            effect.Texture = alturas;
            w = alturas.Width;
            h = alturas.Height;

            normalPosition = new NormalPosition[w,h];

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
                    Vector3 normal;

                    //Linha em cima
                    if (z == 0)
                    {
                        if (x == 0)
                        {
                            Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position));
                            normal = Vector3.Normalize(normal1 + normal2);
                        }

                        else if (x == w - 1)
                        {
                            Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position));
                            normal = Vector3.Normalize(normal1 + normal2);
                        }

                        else
                        {
                            Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position));
                            Vector3 normal3 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal4 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position));
                            normal = Vector3.Normalize(normal1 + normal2 + normal3 + normal4);
                        }
                    }

                    //Linha em baixo
                    else if (z == h - 1)
                    {
                        if (x == 0)
                        {
                            Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position));
                            normal = Vector3.Normalize(normal1 + normal2);
                        }

                        else if (x == w - 1)
                        {
                            Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + x].Position - vertexes[(z - 1) * w + x].Position), Vector3.Normalize(vertexes[z * w + x].Position - vertexes[(z - 1) * w + (x - 1)].Position));
                            Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + x].Position - vertexes[(z - 1) * w + (x - 1)].Position), Vector3.Normalize(vertexes[z * w + x].Position - vertexes[z * w + (x - 1)].Position));
                            normal = Vector3.Normalize(normal1 + normal2);
                        }

                        else
                        {
                            Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position));
                            Vector3 normal3 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                            Vector3 normal4 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position));
                            normal = Vector3.Normalize(normal1 + normal2 + normal3 + normal4);
                        }
                    }

                    //Coluna da esquerda
                    //z nunca vai ser 0 ou h - 1 nesta parte
                    else if (x == 0)
                    {
                        Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal3 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal4 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position));
                        normal = Vector3.Normalize(normal1 + normal2 + normal3 + normal4);
                    }

                    else if (x == w - 1)
                    {
                        Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal3 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal4 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position));
                        normal = Vector3.Normalize(normal1 + normal2 + normal3 + normal4);
                    }

                    //Todos os vértices no meio
                    else
                    {
                        Vector3 normal1 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal2 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal3 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal4 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x - 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position));
                        Vector3 normal5 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + x].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal6 = Vector3.Cross(Vector3.Normalize(vertexes[(z + 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal7 = Vector3.Cross(Vector3.Normalize(vertexes[z * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position));
                        Vector3 normal8 = Vector3.Cross(Vector3.Normalize(vertexes[(z - 1) * w + (x + 1)].Position - vertexes[z * w + x].Position), Vector3.Normalize(vertexes[(z - 1) * w + x].Position - vertexes[z * w + x].Position));
                        normal = Vector3.Normalize(normal1 + normal2 + normal3 + normal4 + normal5 + normal6 + normal7 + normal8);
                    }
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
            effect.Texture = textura;
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
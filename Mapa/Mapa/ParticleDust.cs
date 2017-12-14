using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    //É necessário ter normais para iluminação e como não quero usar textura, criei o VertexPositionColorNormal
    public struct VertexPositionColorNormal : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration
            = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
                );

        public VertexPositionColorNormal(Vector3 pos, Color c, Vector3 n)
        {
            Position = pos;
            Color = c;
            Normal = n;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

    public class ParticleDust
    {
        public Vector3 position;
        public Vector3 direction;
        float speed;
        public VertexPositionColorNormal[] cubeVertexes;
        public short[] cubeIndexes;
        private float cubeSize;
        private Color brown;

        public ParticleDust(Vector3 initialPosition, Vector3 initialDirection, Random random, Matrix rotacao)
        {
            position = initialPosition + (new Vector3(random.Next(-10, 10) * rotacao.Left.X, 0f, random.Next(-10, 10) * rotacao.Left.Z)) / 50f;

            direction = initialDirection + (Vector3.Normalize(new Vector3(random.Next(-10, 10), 0, random.Next(-10, 10)))) * 0.07f;

            speed = 0.02f;
            cubeSize = 0.02f;
            cubeVertexes = new VertexPositionColorNormal[24];
            brown = new Color(68, 50, 33)
            {
                A = 10
            };
        }

        public void Life()
        {
            position += direction * speed;
            direction.Y -= 0.02f;


            //Bastava ter importado um modelo mas quis experimentar asssim
            //Base
            cubeVertexes[0] = new VertexPositionColorNormal(position, brown, Vector3.Down);
            cubeVertexes[1] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y, position.Z), brown, Vector3.Down);
            cubeVertexes[2] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y, position.Z + cubeSize), brown, Vector3.Down);
            cubeVertexes[3] = new VertexPositionColorNormal(new Vector3(position.X, position.Y, position.Z + cubeSize), brown, Vector3.Down);
            //Face superior
            cubeVertexes[4] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + cubeSize, position.Z), brown, Vector3.Up);
            cubeVertexes[5] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z), brown, Vector3.Up);
            cubeVertexes[6] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z + cubeSize), brown, Vector3.Up);
            cubeVertexes[7] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + cubeSize, position.Z + cubeSize), brown, Vector3.Up);
            //Face frontal
            cubeVertexes[8] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + cubeSize, position.Z), brown, Vector3.Forward);
            cubeVertexes[9] = new VertexPositionColorNormal(position, brown, Vector3.Forward);
            cubeVertexes[10] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z), brown, Vector3.Forward);
            cubeVertexes[11] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y, position.Z), brown, Vector3.Forward);
            //Face direita
            cubeVertexes[12] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z), brown, Vector3.Right);
            cubeVertexes[13] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y, position.Z), brown, Vector3.Right);
            cubeVertexes[14] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z + cubeSize), brown, Vector3.Right);
            cubeVertexes[15] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y, position.Z + cubeSize), brown, Vector3.Right);
            //Face traseira
            cubeVertexes[16] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z + cubeSize), brown, Vector3.Backward);
            cubeVertexes[17] = new VertexPositionColorNormal(new Vector3(position.X + cubeSize, position.Y, position.Z + cubeSize), brown, Vector3.Backward);
            cubeVertexes[18] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + cubeSize, position.Z + cubeSize), brown, Vector3.Backward);
            cubeVertexes[19] = new VertexPositionColorNormal(new Vector3(position.X, position.Y, position.Z + cubeSize), brown, Vector3.Backward);
            //Face esquerda
            cubeVertexes[20] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + cubeSize, position.Z + cubeSize), brown, Vector3.Left);
            cubeVertexes[21] = new VertexPositionColorNormal(new Vector3(position.X, position.Y, position.Z + cubeSize), brown, Vector3.Left);
            cubeVertexes[22] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + cubeSize, position.Z), brown, Vector3.Left);
            cubeVertexes[23] = new VertexPositionColorNormal(position, brown, Vector3.Left);

            cubeIndexes = new short[] { 3, 2, 1, 0, 3,
                8, 9, 10, 11,
                12, 13, 14, 15,
                16, 17, 18, 19,
                20, 21, 22, 23,
                4, 5, 6, 7, 4};
        }
    }
}


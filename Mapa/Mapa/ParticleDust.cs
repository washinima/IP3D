using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa
{
    public class ParticleDust
    {
        public Vector3 position;
        public Vector3 direction;
        float speed;
        public VertexPositionColor[] cubeVertexes;
        public short[] cubeIndexes;
        private float cubeSize;
        private Color brown;

        public ParticleDust(Vector3 initialPosition, Vector3 initialDirection, Random random)
        {
            position = initialPosition;
            direction = initialDirection;

            direction = initialDirection + (Vector3.Normalize(new Vector3(random.Next(-10, 10), 0, random.Next(-10, 10)))) * 0.07f;

            speed = 0.02f;
            cubeSize = 0.1f;
            cubeVertexes = new VertexPositionColor[8];
            brown = new Color(56, 24, 7);
            brown.A = 10;
        }

        public void Life()
        {
            position += direction * speed;
            direction.Y -= 0.01f;


            cubeVertexes[0] = new VertexPositionColor(position, brown);
            cubeVertexes[1] = new VertexPositionColor(new Vector3(position.X + cubeSize, position.Y, position.Z), brown);
            cubeVertexes[2] = new VertexPositionColor(new Vector3(position.X + cubeSize, position.Y, position.Z + cubeSize), brown);
            cubeVertexes[3] = new VertexPositionColor(new Vector3(position.X, position.Y, position.Z + cubeSize), brown);

            cubeVertexes[4] = new VertexPositionColor(new Vector3(position.X, position.Y + cubeSize, position.Z), brown);
            cubeVertexes[5] = new VertexPositionColor(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z), brown);
            cubeVertexes[6] = new VertexPositionColor(new Vector3(position.X + cubeSize, position.Y + cubeSize, position.Z + cubeSize), brown);
            cubeVertexes[7] = new VertexPositionColor(new Vector3(position.X, position.Y + cubeSize, position.Z + cubeSize), brown);

            cubeIndexes = new short[] { 3, 2, 1, 0, 3,
            4, 0, 5, 1, 6, 2, 7, 3, 4, 0,
            4, 5, 6, 7, 4};
        }
    }
}


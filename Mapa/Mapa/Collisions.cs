using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Mapa
{
    public class Collisions
    {
        private List<Tanque> _tanqueList;
        private int hit = 0;

        public Collisions(List<Tanque> tanques)
        {
            _tanqueList = tanques;
        }

        private void TankCollisionUpdateHelper(Tanque a, Tanque b)
        {
            float somaRadio = a.Raio + b.Raio;
            Vector3 distEntreTanques = a.Position - b.Position;
            distEntreTanques = new Vector3(
                Math.Abs(distEntreTanques.X),
                Math.Abs(distEntreTanques.Y),
                Math.Abs(distEntreTanques.Z)
            );

            if (somaRadio > distEntreTanques.X && somaRadio > distEntreTanques.Z)
            {
                if (somaRadio > distEntreTanques.X)
                {
                    a.Position = new Vector3(
                        a.OldPosition.X,
                        a.Position.Y,
                        a.Position.Z
                    );
                    b.Position = new Vector3(
                        b.OldPosition.X,
                        b.Position.Y,
                        b.Position.Z
                    );
                }
                if (somaRadio > distEntreTanques.Z)
                {
                    a.Position = new Vector3(
                        a.Position.X,
                        a.Position.Y,
                        a.OldPosition.Z
                    );
                    b.Position = new Vector3(
                        b.Position.X,
                        b.Position.Y,
                        b.OldPosition.Z
                    );
                }
            }
        }

        private void BulletCollisionUpdateHelper(Tanque a, Tanque b)
        {
            List<Projectile> _bullets = a.Projectiles;

            for (int i = 0; i <= _bullets.Count - 1; i++)
            {
                float somaRadio = b.Raio + _bullets[i].Raio;
                Vector3 prediction = _bullets[i].position + _bullets[i].direction;

                Vector3 lineCast = b.Position - _bullets[i]._disTravelled;

                lineCast = new Vector3(
                    Math.Abs(lineCast.X),
                    Math.Abs(lineCast.Y),
                    Math.Abs(lineCast.Z)
                );

                
                Vector3 distEntreObj = b.Position - prediction;
                distEntreObj = new Vector3(
                    Math.Abs(distEntreObj.X),
                    Math.Abs(distEntreObj.Y),
                    Math.Abs(distEntreObj.Z)
                );
                
                if (somaRadio > distEntreObj.X && somaRadio > distEntreObj.Z && somaRadio > distEntreObj.Y)
                {
                    if (somaRadio > distEntreObj.X || somaRadio > distEntreObj.Z || somaRadio > distEntreObj.Y)
                    {
                        hit++;
                        Debug.WriteLine("HIT " + hit);
                        _bullets[i].Dead = true;
                    }
                }
            }
        }

        public void BulletCollisionUpdate(Tanque tanque1, Tanque tanque2)
        {
            BulletCollisionUpdateHelper(tanque1, tanque2);
            BulletCollisionUpdateHelper(tanque2, tanque1);
        }

        public void CollisionUpdate()
        {
            //Ao usar helpers nao preciso de percorrer a lista varias vezes. Apenas uma.
            for (int i = 0; i <= _tanqueList.Count - 2; i++)
            {
                for (int j = i + 1; j <= _tanqueList.Count - 1; j++)
                {
                    TankCollisionUpdateHelper(_tanqueList[i], _tanqueList[j]);

                    BulletCollisionUpdate(_tanqueList[i], _tanqueList[j]);
                }
            }
        }

        public void Update () 
        {
            CollisionUpdate ();
        }
    }
}
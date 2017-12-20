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

        private Vector3 AbsDist(Vector3 a, Vector3 b)
        {
            Vector3 tmp = b - a;

            tmp = new Vector3(
                Math.Abs(tmp.X),
                Math.Abs(tmp.Y),
                Math.Abs(tmp.Z)
            );

            return tmp;
        }


        private void BulletCollisionUpdateHelper(Tanque a, Tanque b)
        {
            List<Projectile> _bullets = a.Projectiles;

            for (int i = 0; i <= _bullets.Count - 1; i++)
            {
                float somaRadio = b.Raio + _bullets[i].Raio;

                Vector3 AB = AbsDist(_bullets[i]._oldPos, _bullets[i].position);
                double ab = Constants.LengthOfVector3(AB);


                Vector3 AC = AbsDist(_bullets[i]._oldPos, b.Position);
                double ac = Constants.LengthOfVector3(AC);

                Vector3 CB = AbsDist(_bullets[i].position, b.Position);
                double cb = Constants.LengthOfVector3(CB);

                double areaRectangle = ab + ac + cb;
                double sp = areaRectangle / 2;

                double areaTri = Math.Sqrt(
                    sp * (sp - ab) * (sp - ac) * (sp - cb)
                );

                double distance = 2 * areaTri / ab;


              
                /* Se dot for positivo o angulo e agudo
                 * Se for 0 e perpendicular
                 * Se for negativo e obtuso
                 */
                float dot = Vector3.Dot(  
                    _bullets[i].position - _bullets[i]._oldPos,
                    b.Position - _bullets[i]._oldPos 
                );

                if (distance < somaRadio && dot > 0)
                {
                    _bullets[i].Dead = true;
                    b.Health -= 1;
                }


                /*double angle = Math.Acos(
                                  AB.X * AC.X + AB.Y * AC.Y +
                                  AB.Z * AC.Z / 
                                  (Constants.LengthOfVector3(AB) * Constants.LengthOfVector3(AC))
                              );




               double angle = Math.Acos(Vector3.Dot(AB, AC));
               */

                /*Vector3 distEntreObj = b.Position - prediction;
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
                }*/
            }
        }

        public void BulletCollisionUpdate(Tanque tanque1, Tanque tanque2)
        {
            BulletCollisionUpdateHelper(tanque1, tanque2);
            BulletCollisionUpdateHelper(tanque2, tanque1);
        }

        private void BulletRemoval(Tanque tanque)
        {
            for (int i = tanque.Projectiles.Count - 1; i >= 0 ; i--)
            {
                if (tanque.Projectiles[i].Dead)
                {
                    tanque.Projectiles.RemoveAt(i);
                }
            }
        }

        private void TankRemoval()
        {
            for (int i = _tanqueList.Count - 1; i >= 0; i--)
            {
                if (_tanqueList[i].Health <= 0)
                {
                    _tanqueList[i].SistemaDeParticulas.TankExplosion(_tanqueList[i].Position);
                    _tanqueList.RemoveAt(i);
                }
            }
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

            for (int i = 0; i <= _tanqueList.Count - 1; i++)
            {
                if (_tanqueList[i].Position.X - _tanqueList[i].Raio < 0 || _tanqueList[i].Position.X + _tanqueList[i].Raio > _tanqueList[i].NormalPosition.GetLength(0) - 1)
                    _tanqueList[i].Position = new Vector3(
                        _tanqueList[i].OldPosition.X,
                        _tanqueList[i].Position.Y,
                        _tanqueList[i].Position.Z
                    );
                if (_tanqueList[i].Position.Z - _tanqueList[i].Raio < 0 || _tanqueList[i].Position.Z + _tanqueList[i].Raio > _tanqueList[i].NormalPosition.GetLength(1) - 1)
                    _tanqueList[i].Position = new Vector3(
                        _tanqueList[i].Position.X,
                        _tanqueList[i].Position.Y,
                        _tanqueList[i].OldPosition.Z
                    );


                BulletRemoval(_tanqueList[i]);

            }

            TankRemoval();

        }

        public void Update () 
        {
            CollisionUpdate ();
        }
    }
}
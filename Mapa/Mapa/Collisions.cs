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

        public void TankCollisionUpdate()
        {
            for (int i = 0; i <= _tanqueList.Count - 2; i++)
            {
                for (int j = i+1; j <= _tanqueList.Count - 1; j++)
                {
                    float somaRadio = _tanqueList[i].Raio + _tanqueList[j].Raio;
                    Vector3 distEntreTanques = _tanqueList[i].Position - _tanqueList[j].Position;
                    distEntreTanques = new Vector3(
                        Math.Abs(distEntreTanques.X),
                        Math.Abs(distEntreTanques.Y), 
                        Math.Abs(distEntreTanques.Z)
                        );

                    if (somaRadio > distEntreTanques.X && somaRadio > distEntreTanques.Z)
                    {
                        if (somaRadio > distEntreTanques.X)
                        {
                            _tanqueList[i].Position = new Vector3(
                                _tanqueList[i].OldPosition.X,
                                _tanqueList[i].Position.Y,
                                _tanqueList[i].Position.Z
                            );
                            _tanqueList[j].Position = new Vector3(
                                _tanqueList[j].OldPosition.X,
                                _tanqueList[j].Position.Y,
                                _tanqueList[j].Position.Z
                            );
                        }
                        if (somaRadio > distEntreTanques.Z)
                        {
                            _tanqueList[i].Position = new Vector3(
                                _tanqueList[i].Position.X,
                                _tanqueList[i].Position.Y,
                                _tanqueList[i].OldPosition.Z
                            );
                            _tanqueList[j].Position = new Vector3(
                                _tanqueList[j].Position.X,
                                _tanqueList[j].Position.Y,
                                _tanqueList[j].OldPosition.Z
                            );
                        }
                    }
                }
            }
        }

        public void BulletCollisionUpdate()
        {
            for (int i = 0; i <= _tanqueList.Count - 2; i++)
            {
                for (int j = i + 1; j <= _tanqueList.Count - 1; j++)
                {
                    List<Projectile> _bullets= _tanqueList[i].Projectiles;
                
                    for (int pi = 0; pi <= _bullets.Count - 1; pi++)
                    {
                        float somaRadio = _tanqueList[j].Raio + _bullets[pi].Raio;
                        Vector3 distEntreObj = _tanqueList[j].Position - _bullets[pi].position;
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
                                _bullets[pi].Dead = true;
                            }
                        }
                    }


                    _bullets = _tanqueList[j].Projectiles;

                    for (int pj = 0; pj <= _bullets.Count - 1; pj++)
                    {
                        float somaRadio = _tanqueList[i].Raio + _bullets[pj].Raio;
                        Vector3 distEntreObj = _tanqueList[i].Position - _bullets[pj].position;
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
                                _bullets[pj].Dead = true;
                            }
                        }
                    }

                }
            }
        }

        public void Update()
        {
            TankCollisionUpdate();
            BulletCollisionUpdate();
        }
    }
}

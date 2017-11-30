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

        public void TankCollisionUpdate()
        {
            for (int i = 0; i <= _tanqueList.Count - 2; i++)
            {
                for (int j = i+1; j <= _tanqueList.Count - 1; j++)
                {
                    float somaRadio = _tanqueList[i].Sphere.Radius + _tanqueList[j].Sphere.Radius;
                    Vector3 distEntreTanques = _tanqueList[i].Sphere.Center - _tanqueList[j].Sphere.Center;
                    distEntreTanques = new Vector3(
                        Math.Abs(distEntreTanques.X),
                        Math.Abs(distEntreTanques.Y), 
                        Math.Abs(distEntreTanques.Z)
                        );

                    if (somaRadio > distEntreTanques.X && somaRadio > distEntreTanques.Z)
                    {
                        Console.WriteLine("A");
                    }
                    else
                    {
                        Console.WriteLine("Bullshit");
                    }
                }
            }
        }
    }
}

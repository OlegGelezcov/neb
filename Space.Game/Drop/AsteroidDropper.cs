using Space.Game.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using GameMath;


namespace Space.Game.Drop
{
    public class AsteroidDropper
    {
        /// <summary>
        /// Create or not asteroid in zone, with prob of data
        /// </summary>
        public bool DropOccured(int zoneLevel, AsteroidData data, bool forceCreation)
        {
            if (forceCreation)
                return true;

            return (Rand.Float01() < this.GetDataProb(data, zoneLevel));
        }

        public Dictionary<string, int> DropMaterials(AsteroidData asteroidData )
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            if (asteroidData != null) {
                foreach (var dropData in asteroidData.ContentData) {
                    for (int i = 0; i < dropData.MaxCount; i++) {
                        if (Rand.Float01() <= dropData.Prob) {
                            if (result.ContainsKey(dropData.Id))
                                result[dropData.Id]++;
                            else
                                result.Add(dropData.Id, 1);
                        }
                    }
                }
            }
            return result;
        }

        private float GetDataProb(AsteroidData data, int zoneLevel)
        {
            
            float res =  0.5f / (float)(Math.Pow(Math.Abs(data.Quality - zoneLevel), 2) + 1.0f);
            //Console.WriteLine("prob of asteroid: " + res);
            return res;
        }
    }
}

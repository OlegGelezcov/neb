using Common;
using Nebula.Engine;
using Nebula.Game;
using Nebula.Game.Components;
using Nebula.Server;
using Space.Game;
using Space.Game.Resources.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DSL_TEST.InfoStats {
    public class Collector {
        private const int NUM_TESTS_FOR_WORKSHOP = 1000;

        private Res mRes;
        private MmoWorld mWorld;
        private readonly List<StatsData> mStatList;

        public Collector() {
            mStatList = new List<StatsData>();
            mRes = new Res("");
            mRes.Load();
            mWorld = new MmoWorld("", new GameMath.Vector { X = -100, Y = -100, Z = -100 }, new GameMath.Vector { X = 100, Y = 100, Z = 100 }, new GameMath.Vector { X = 100, Y = 100, Z = 100 }, mRes);
        }

        public void CollectTests(int level) {
            //mStatList.Clear();
            //foreach(Workshop workshop in Enum.GetValues(typeof(Workshop))) {
            //    StatsData stats = new StatsData(workshop, level);
            //    for(int counter = 0; counter < NUM_TESTS_FOR_WORKSHOP; counter++) {
            //        stats.Aggregate(CreateNewNebulaObject(workshop, level));
            //    }
            //    mStatList.Add(stats);
            //}

            //WriteResults();
        }

        private float maximumDamage {
            get {
                return mStatList.Max(sd => sd.averageDamage);
            }
        }

        private float maximumSpeed {
            get {
                return mStatList.Max(sd => sd.averageSpeed);
            }
        }

        private float maximumHealth {
            get {
                return mStatList.Max(sd => sd.averageHealth);
            }
        }

        private float maximumOptimalDistance {
            get {
                return mStatList.Max(sd => sd.averageOptimalDistance);
            }
        }

        public float maximumCriticalChance {
            get {
                return mStatList.Max(sd => sd.averageCriticalChance);
            }
        }

        public float maximumCriticalDamage {
            get {
                return mStatList.Max(sd => sd.averageCriticalDamage);
            }
        }

        private void WriteResults() {
            float maxDamage = maximumDamage;
            float maxSpeed = maximumSpeed;
            float maxHealth = maximumHealth;
            float maxOptimalDistance = maximumOptimalDistance;
            float maxCritChance = maximumCriticalChance;
            float maxCritDamage = maximumCriticalDamage;


            XElement root = new XElement("stats");
            foreach(var stat in mStatList) {
                XElement workshopElement = new XElement("workshop");
                workshopElement.SetAttributeValue("name", stat.workshop.ToString());
                workshopElement.SetAttributeValue("damage", stat.damagePercent(maxDamage).ToString());
                workshopElement.SetAttributeValue("speed", stat.speedPercent(maxSpeed).ToString());
                workshopElement.SetAttributeValue("hp", stat.healthPercent(maxHealth));
                workshopElement.SetAttributeValue("optimal_distance", stat.optimalDistance(maxOptimalDistance));
                workshopElement.SetAttributeValue("crit_chance", stat.criticalChance(maxCritChance));
                workshopElement.SetAttributeValue("crit_damage", stat.criticalDamage(maxCritDamage));
                root.Add(workshopElement);
            }
            Console.WriteLine(root.ToString());
            root.Save("stats.xml");
        }

        //private NebulaObject CreateNewNebulaObject(Workshop workshop, int level) {
        //    ZoneNpcInfo info = new ZoneNpcInfo {
        //        aiType = new FreeFlyNearPointAIType { battleMovingType = AttackMovingType.AttackPurchase, radius = 100 },
        //        Difficulty = Common.Difficulty.none,
        //        fraction = Common.FractionType.Pirate,
        //        Id = Guid.NewGuid().ToString(),
        //        level = level,
        //        name = "BOT",
        //        Position = new float[] { 0, 0, 0 },
        //        Race = CommonUtils.RaceForWorkshop(workshop),
        //        RespawnInterval = 100,
        //        Rotation = new float[] { 0, 0, 0 },
        //        Workshop = workshop
        //    };
        //    var obj = ObjectCreate.CombatNpc(mWorld, info);
        //    obj.Update(0);
        //    return obj;
        //}
    }
}

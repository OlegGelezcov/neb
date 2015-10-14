using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Server;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using System;
using System.Linq;
using System.Threading;

namespace Space.Game {
    public class NpcGroup : INpcOwner
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private int npcCounter;
        private readonly NpcGroupData npcGroupData;
        private float lastSpawnTime;
        private readonly MmoWorld world;

        public NpcGroup(MmoWorld world, NpcGroupData npcGroupData)
        {
            this.npcGroupData = npcGroupData;
            this.npcCounter = 0;
            this.lastSpawnTime = 0f;
            this.world = world;
        }
        public void HandleNpcDeath(string npcID)
        {
            this.DecrementNpcCounter();
        }

        private NpcGroupData NpcGroupData()
        {
            return this.npcGroupData;
        }

        public void Update(float time)
        {
            if(time - lastSpawnTime < this.NpcGroupData().SpawnInterval)
            {
                return;
            }

            if (this.NpcCountLessThanMaxValue())
            {
                this.CreateSingleNpc(time);
            }
        }


        private int NpcCounter()
        {
            return this.npcCounter;
        }

        private float LastSpawnTime()
        {
            return this.lastSpawnTime;
        }

        private void IncrementNpcCounter()
        {
            Interlocked.Increment(ref npcCounter);
            //log.InfoFormat("Npc counter on npc group incremented, {0}:{1}", NpcGroupData().Id, NpcCounter());
        }

        private void DecrementNpcCounter()
        {
            Interlocked.Decrement(ref npcCounter);
            //log.InfoFormat("Npc counter on npc group decremented, {0}:{1}", NpcGroupData().Id, NpcCounter());
        }

        private void SetLastSpawnTime(float time)
        {
            this.lastSpawnTime = time;
        }

        private bool NpcCountLessThanMaxValue()
        {
            return this.NpcCounter() < this.NpcGroupData().MaxCount;
        }

        private MmoWorld World()
        {
            return this.world;
        }

        private void CreateSingleNpc(float time)
        {
            //var npc = StandardCombatNpcObject.Create(this.World(), ZoneNpcInfo(), NpcGroupData().Level, GameObjectEventInfo.Default, NpcGroupData().NpcName, NpcObjectOwner.CreateGroup(this));
            //if(!npc.AddToWorld())
            //{
            //    log.ErrorFormat("Error of adding grouped npc to world. Source group: {0}", NpcGroupData().Id);
            //    return;
            //}
            //this.IncrementNpcCounter();
            //this.SetLastSpawnTime(time);
        }

        private ZoneNpcInfo ZoneNpcInfo()
        {
            return new ZoneNpcInfo {
                Difficulty = NpcGroupData().Difficulty,
                Id = Guid.NewGuid().ToString(),
                Position = GenPosition(),
                Rotation = GenRotation(),
                Race = NpcGroupData().Race,
                RespawnInterval = float.MaxValue,
                //TypeName = NpcGroupData().NpcTypeName,
                Workshop = CommonUtils.RandomWorkshop(NpcGroupData().Race),
                name = "Bot",
                level = world.Zone.Level,
                fraction = FractionType.Pirate,
                aiType = new FreeFlyAtBoxAIType { corners = new MinMax { min = new Vector3(-100, -100, -100), max = new Vector3(100, 100, 100) }, battleMovingType = AttackMovingType.AttackPurchase  }
            };
        }

        private float[] GenPosition()
        {
            return NpcGroupData().Center.ToArray().RandomizeInMinMax(NpcGroupData().Bounds);
        }

        private float[] GenRotation()
        {
            return Rand.VectorMinMax(90).ToArray();
        }




        public string GetID()
        {
            return this.npcGroupData.Id;
        }
    }
}

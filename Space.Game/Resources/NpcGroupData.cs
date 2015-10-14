using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Resources
{
    public class NpcGroupData
    {
        private readonly string id;
        private readonly Vector3 center;
        private readonly MinMax bounds;
        private readonly int maxCount;
        private readonly string npcTypeName;
        private readonly int level;
        private readonly float spawnInterval;
        private readonly Difficulty difficulty;
        private readonly Race race;
        private readonly string npcName;


        public NpcGroupData(string id, Vector3 center, MinMax bounds, 
            int maxCount, string npcTypeName, int level, float spawnInterval,
            Difficulty difficulty, Race race, string npcName)
        {
            this.id = id;
            this.center = center;
            this.bounds = bounds;
            this.maxCount = maxCount;
            this.npcTypeName = npcTypeName;
            this.level = level;
            this.spawnInterval = spawnInterval;
            this.difficulty = difficulty;
            this.race = race;
            this.npcName = npcName;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public Vector3 Center
        {
            get
            {
                return this.center;
            }
        }

        public MinMax Bounds
        {
            get
            {
                return this.bounds;
            }
        }

        public int MaxCount
        {
            get
            {
                return this.maxCount;
            }
        }

        public string NpcTypeName
        {
            get
            {
                return this.npcTypeName;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }

        public float SpawnInterval
        {
            get
            {
                return this.spawnInterval;
            }
        }

        public Difficulty Difficulty
        {
            get
            {
                return this.difficulty;
            }
        }

        public Race Race
        {
            get
            {
                return this.race;
            }
        }

        public string NpcName
        {
            get
            {
                return this.npcName;
            }
        }
    }
}

//using Space.Game.Events;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Common;
//using GameMath;
//using System.Collections;

//namespace Space.Game.Resources
//{
//    public class EventInfo
//    {
//        private readonly string id;
//        private readonly string nameId;
//        private readonly string descriptionId;
//        private readonly float cooldown;
//        private readonly int rewardExp;
//        private readonly int rewardCoins;
//        private readonly Vector3 position;
//        private readonly Dictionary<int, EventStage> stages;
//        private readonly Hashtable inputs;

//        public EventInfo(string id, string nameId, string descriptionId, float cooldown, int rewardExp, int rewardCoins, Vector3 position,
//            Dictionary<int, EventStage> stages, Hashtable inputs)
//        {
//            this.id = id;
//            this.nameId = nameId;
//            this.descriptionId = descriptionId;
//            this.cooldown = cooldown;
//            this.rewardExp = rewardExp;
//            this.rewardCoins = rewardCoins;
//            this.position = position;
//            this.stages = stages;
//            this.inputs = inputs;
//        }

//        public string Id
//        {
//            get
//            {
//                return this.id;
//            }
//        }

//        public string Name
//        {
//            get
//            {
//                return this.nameId;
//            }
//        }

//        public string Description
//        {
//            get
//            {
//                return this.descriptionId;
//            }
//        }

//        public float Cooldown
//        {
//            get
//            {
//                return this.cooldown;
//            }
//        }

//        public int RewardExp
//        {
//            get
//            {
//                return this.rewardExp;
//            }
//        }

//        public int RewardCoins
//        {
//            get
//            {
//                return this.rewardCoins;
//            }
//        }

//        public Vector3 Position
//        {
//            get
//            {
//                return this.position;
//            }
//        }

//        public Dictionary<int, EventStage> Stages
//        {
//            get
//            {
//                return this.stages;
//            }
//        }

//        public EventStage Stage(int stageId )
//        {
//            if(this.stages.ContainsKey(stageId))
//                return this.stages[stageId];
//            throw new Exception("not found stage: {0}".f(stageId));
//        }

//        public T GetInput<T>(string key)
//        {
//            return this.inputs.GetValue<T>(key, default(T));
//        }
//    }
//}

//using Common;
//using ExitGames.Logging;
//using GameMath;
//using Nebula.Game.Components;
//using ServerClientCommon;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Space.Game.Groups {
//    public class CooperativeGroupMember : IInfoSource {

//        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

//        private readonly MmoActor actor;
//        private bool isLeader;

//        public CooperativeGroupMember(MmoActor actor, bool isLeader) {
//            this.actor = actor;
//            this.isLeader = isLeader;
            
//        }

//        public void SetLeader(bool is_leader) {
//            this.isLeader = is_leader;
//        }

//        public string GameRefId() {
//            return actor.nebulaObject.Id;
//        }
//        public string CharacterId() {
//            return actor.GetComponent<PlayerCharacterObject>().characterId;
//        }
//        public Workshop Workshop() {
//            return (Workshop)actor.GetComponent<PlayerCharacterObject>().workshop;
//        }
//        public string DisplayName() {
//            return this.actor.name;
//        }

//        public bool IsLeader() {
//            return this.isLeader;
//        }

//        public Vector3 Position() {
//            return this.actor.Position;
//        }

//        public string WorldName() {
//            return ((MmoWorld)this.actor.nebulaObject.world).Name;
//        }

//        public int Level() {
//            return this.actor.GetComponent<PlayerCharacterObject>().level;
//        }

//        public Hashtable Buffs() {
//            return this.actor.Buffs();
//        }

//        public void Detach() {
//            actor.GroupController().Detach();
//        }

//        public string ItemId() {
//            return actor.nebulaObject.Id;
//        }

//        public MmoActor Player() {
//            return actor;
//        }

//        public Hashtable GetInfo() {
//            return new Hashtable { 
//                {(int)SPC.Workshop,        (byte)Workshop()                },
//                {(int)SPC.DisplayName,    this.DisplayName()              },
//                {(int)SPC.IsLeader,       this.IsLeader()                 },
//                {(int)SPC.Position,        this.Position().ToArray()       },
//                {(int)SPC.WorldId,        this.WorldName()                },
//                {(int)SPC.Level,           this.Level()                    },
//                {(int)SPC.Buffs,           this.Buffs()                    },
//                {(int)SPC.SelectedCharacterId, this.CharacterId()        },
//                {(int)SPC.ItemId, this.ItemId() }
//            };
//        }

//        public Hashtable RequestSearchInfo() {
//            return new Hashtable { 
//                {(int)SPC.Workshop, (byte)Workshop() },
//                {(int)SPC.DisplayName, this.DisplayName() },
//                {(int)SPC.Level, Level() },
//                {(int)SPC.ItemId, ItemId() }
//            };
//        }

//        public void SendGroupUpdate(string groupId, Hashtable groupInfo) {
//            this.actor.EnqueueEventGroupUpdate(groupId, groupInfo);
//        }

//        public void SendChatMessage(Hashtable msg) {
//            if (this.Player() == null) {
//                log.Error("member actor null");
//                return;
//            }
//            this.Player().Chat.AddMessage(msg);
//        }
//    }
//}

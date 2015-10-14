//using Common;
//using ServerClientCommon;
//using System.Collections;

//namespace Nebula.Client {
//    public class ClientCooperativeGroupMember : IInfoParser {

//        private Workshop workshop;
//        private string displayName;
//        private bool isLeader;
//        private float[] position;
//        private string worldId;
//        private int level;
//        private Hashtable buffs;
//        private string characterId;
//        private string itemId;

//        public ClientCooperativeGroupMember(Hashtable info) {
//            this.ParseInfo(info);
//        }

//        public void ParseInfo(Hashtable info) {
//            this.workshop = (Workshop)info.GetValue<byte>((int)SPC.Workshop, (byte)Common.Workshop.Arlen);
//            this.displayName = info.GetValue<string>((int)SPC.DisplayName, string.Empty);
//            this.isLeader = info.GetValue<bool>((int)SPC.IsLeader, false);
//            this.position = info.GetValue<float[]>((int)SPC.Position, new float[] { 0f, 0f, 0f });
//            this.worldId = info.GetValue<string>((int)SPC.WorldId, string.Empty);
//            this.level = info.GetValue<int>((int)SPC.Level, 0);
//            this.buffs = info.GetValue<Hashtable>((int)SPC.Buffs, new Hashtable());
//            this.characterId = info.GetValue<string>((int)SPC.SelectedCharacterId, string.Empty);
//            this.itemId = info.GetValue<string>((int)SPC.ItemId, string.Empty);
//        }

//        public Workshop Workshop() {
//            return this.workshop;
//        }
//        public string DisplayName() {
//            return this.displayName;
//        }
//        public bool IsLeader() {
//            return this.isLeader;
//        }
//        public float[] Position() {
//            return this.position;
//        }
//        public string WorldId() {
//            return this.worldId;
//        }
//        public int Level() {
//            return this.level;
//        }
//        public Hashtable Buffs() {
//            return this.buffs;
//        }

//        public string CharacterId() {
//            return this.characterId;
//        }

//        public string ItemId() {
//            return this.itemId;
//        }
//    }
//}

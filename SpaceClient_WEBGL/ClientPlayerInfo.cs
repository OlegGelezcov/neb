using Common;
using System.Text;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientPlayerInfo : IInfoParser {
        public string CharacterId { get; private set; }
        public string Name { get; private set; }
        public Workshop Workshop { get; private set; }
        public Race Race { get; private set; }
        public int Exp { get; private set; }
        public Hashtable Model { get; private set; }

        private System.Action<int, int> onExpChanged;

        public ClientPlayerInfo() {
            CharacterId = "";
            Name = "";
            Workshop = Workshop.Arlen;
            Race = Race.None;
            Exp = 0;
            Model = new Hashtable();
        }

        public ClientPlayerInfo(Hashtable info) {
            this.Exp = 0;
            this.ParseInfo(info);
        }


        public void ParseInfo(Hashtable info) {
            int oldExp = this.Exp;
            CharacterId = info.GetValueString((int)SPC.Id);
            Name = info.GetValueString((int)SPC.Name);
            Workshop = (Workshop)info.GetValueInt((int)SPC.Workshop);
            Race = (Race)info.GetValueInt((int)SPC.Race);
            Exp = info.GetValueInt((int)SPC.Exp);
            Model = info.GetValueHash((int)SPC.Model);

            this.HandleExpLevelChanges(oldExp, this.Exp);
        }

        public void Replace(ClientPlayerInfo other) {
            int oldExp = this.Exp;
            this.Exp = other.Exp;
            this.Workshop = other.Workshop;
            this.Name = other.Name;
            this.Race = other.Race;
            Model = other.Model;

            this.HandleExpLevelChanges(oldExp, this.Exp);
        }

        private void HandleExpLevelChanges(int oldExp, int exp) {
            if (exp != oldExp)
                if (onExpChanged != null)
                    onExpChanged(oldExp, exp);
        }

        public void SetExpChanged(System.Action<int, int> onExpChanged) {
            this.onExpChanged = onExpChanged;
        }

        public override string ToString() {
            Hashtable ht = new Hashtable {
                {SPC.Id, CharacterId },
                { SPC.Name, Name },
                { SPC.Workshop, Workshop },
                { SPC.Race, Race },
                { SPC.Exp, Exp },
                { SPC.Model, Model}
            };
            StringBuilder builder = new StringBuilder();
            CommonUtils.ConstructHashString(ht, 1, ref builder);
            return builder.ToString();
        }
    }
}

using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;

namespace Nebula.Client.Contracts {
    public class DestroyConstructionContract : BaseContract {
        public BotItemSubType constructionType { get; private set; }
        public Race constructionRace { get; private set; }

        public DestroyConstructionContract(Hashtable hash) 
            : base(hash) {
            constructionType = (BotItemSubType)(byte)hash.GetValueInt((int)SPC.SubType);
            constructionRace = (Race)(byte)hash.GetValueInt((int)SPC.Race);
        }

        public override string ToString() {
            string bString = base.ToString();
            string nString = string.Format("construction type = {0}, construction race = {1}", constructionType, constructionRace);
            return bString + Environment.NewLine + nString;
        }

        public override Hashtable Dump() {
            var hash = base.Dump();
            hash.Add("construction type", constructionType.ToString());
            hash.Add("construction race", constructionRace.ToString());
            return hash;
        }

        public override bool TargetAtWorld(string worldId) {
            return false;
        }

        public override string GetTargetWorld() {
            return string.Empty;
        }
    }
}

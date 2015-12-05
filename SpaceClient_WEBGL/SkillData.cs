using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class SkillData : IInfoParser {
        public int id { get; private set; }
        public SkillType skillType { get; private set; }
        public float duration { get; private set; }
        public float requiredEnergy { get; private set; }
        public float cooldown { get; private set; }
        public Hashtable inputs { get; private set; }

        public void ParseInfo(Hashtable info) {
            id = info.GetValueInt((int)SPC.Id);
            skillType = (SkillType)info.GetValueByte((int)SPC.Type);
            duration = info.GetValueFloat((int)SPC.Duration);
            requiredEnergy = info.GetValueFloat((int)SPC.Energy);
            cooldown = info.GetValueFloat((int)SPC.Cooldown);
            inputs = info.GetValueHash((int)SPC.Inputs);
        }

        public SkillData(Hashtable info) {
            ParseInfo(info);
        }
    }
}

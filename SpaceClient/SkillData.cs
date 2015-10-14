using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client {
    public class SkillData  : IInfoParser{
        public int id { get; private set; }
        public SkillType skillType { get; private set; }
        public float duration { get; private set; }
        public float requiredEnergy { get; private set; }
        public float cooldown { get; private set; }
        public Hashtable inputs { get; private set; }

        public void ParseInfo(Hashtable info) {
            id = info.Value<int>((int)SPC.Id);
            skillType = (SkillType)info.Value<byte>((int)SPC.Type);
            duration = info.Value<float>((int)SPC.Duration);
            requiredEnergy = info.Value<float>((int)SPC.Energy);
            cooldown = info.Value<float>((int)SPC.Cooldown);
            inputs = info.Value<Hashtable>((int)SPC.Inputs);
        }

        public SkillData(Hashtable info) {
            ParseInfo(info);
        }
    }
}

using Common;
using ServerClientCommon;
using System.Collections;

namespace Space.Game.Resources {
    public class SkillData : IInfoSource
    {
        public int Id { get; set; }
        public SkillType Type { get; set; }
        public float Durability { get; set; }
        public float RequiredEnergy { get; set; }
        public float Cooldown { get; set; }
        public Hashtable Inputs { get; set; }

        public static SkillData Empty
        {
            get
            {
                return new SkillData {
                    Cooldown = float.MaxValue,
                    Durability = 0,
                    Id = -1,
                    Inputs = new Hashtable(),
                    RequiredEnergy = float.MaxValue,
                    Type = SkillType.OneUse
                };
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Id == -1;
            }
        }

        public Hashtable GetInfo()
        {
            return new Hashtable 
            {
                {(int)SPC.Id, this.Id },
                {(int)SPC.Type, this.Type.toByte() },
                {(int)SPC.Duration, this.Durability},
                {(int)SPC.Energy, this.RequiredEnergy},
                {(int)SPC.Cooldown, this.Cooldown},
                {(int)SPC.Inputs, (this.Inputs != null ) ? this.Inputs : new Hashtable() }
            };
        }
    }
}

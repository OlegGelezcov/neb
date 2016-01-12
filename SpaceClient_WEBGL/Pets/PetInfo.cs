using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Pets {
    public class PetInfo : IInfoParser {
        public bool active { get; private set; }
        public int[] activeSkills { get; private set; }
        public PetColor color { get; private set; }
        public int exp { get; private set; }
        public string id { get; private set; }
        public int passiveSkills { get; private set; }
        public string model { get; private set; }
        public float damage { get; private set; }
        public float maxHp { get; private set; }
        public float optimalDistance { get; private set; }
        public float cooldown { get; private set; }
        public WeaponDamageType damageType { get; private set; }
        public float killedTime { get; private set; }
        public int mastery { get; private set; }
        public float currentTime { get; private set; }

        public PetInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public void ParseInfo(Hashtable hash) {
            active = hash.GetValueBool((int)SPC.Active);
            activeSkills = hash.GetValueIntArray((int)SPC.Skills);
            color = (PetColor)hash.GetValueInt((int)SPC.Color, (int)PetColor.gray);
            exp = hash.GetValueInt((int)SPC.Exp);
            id = hash.GetValueString((int)SPC.Id);
            passiveSkills = hash.GetValueInt((int)SPC.PassiveSkill);
            model = hash.GetValueString((int)SPC.Type);
            damage = hash.GetValueFloat((int)SPC.Damage);
            maxHp = hash.GetValueFloat((int)SPC.MaxHealth);
            optimalDistance = hash.GetValueFloat((int)SPC.OptimalDistance);
            cooldown = hash.GetValueFloat((int)SPC.Cooldown);
            damageType = (WeaponDamageType)(byte)hash.GetValueInt((int)SPC.DamageType);
            killedTime = hash.GetValueFloat((int)SPC.KilledTime);
            mastery = hash.GetValueInt((int)SPC.Mastery);
            currentTime = hash.GetValueFloat((int)SPC.CurrentTime);
        }

        public bool hasActiveSkills {
            get {
                if(activeSkills == null ) {
                    activeSkills = new int[] { };
                }
                return (activeSkills.Length > 0);
            }
        }


    }
}

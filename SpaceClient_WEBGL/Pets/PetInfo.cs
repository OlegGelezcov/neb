using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System.Collections.Generic;

namespace Nebula.Client.Pets {
    public class PetInfo : IInfoParser {
        public bool active { get; private set; }
        public PetActiveSkill[] activeSkills { get; private set; }
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

        public override string ToString() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Format("active: {0}", active));
            sb.AppendLine(string.Format("active skill count: {0}", activeSkills.Length));
            sb.AppendLine(string.Format("color: {0}", color));
            sb.AppendLine(string.Format("exp: {0}", exp));
            sb.AppendLine(string.Format("id: {0}", id));
            sb.AppendLine(string.Format("passive skill: {0}", passiveSkills));
            sb.AppendLine(string.Format("model: {0}", model));
            sb.AppendLine(string.Format("damage: {0}", damage));
            sb.AppendLine(string.Format("max hp: {0}", maxHp));
            sb.AppendLine(string.Format("optimal distance: {0}", optimalDistance));
            sb.AppendLine(string.Format("cooldown: {0}", cooldown));
            sb.AppendLine(string.Format("damage type: {0}", damageType));
            sb.AppendLine(string.Format("killed time: {0}", killedTime));
            sb.AppendLine(string.Format("mastery: {0}", mastery));
            sb.AppendLine(string.Format("current time: {0}", currentTime));
            return sb.ToString();
        }

        public PetInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public void ParseInfo(Hashtable hash) {
            active = hash.GetValueBool((int)SPC.Active);
            activeSkills = ParseActiveSkills(hash);
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

        private PetActiveSkill[] ParseActiveSkills(Hashtable hash) {
            Hashtable[] skillArr = hash[(int)SPC.Skills] as Hashtable[];
            List<PetActiveSkill> result = new List<PetActiveSkill>();
            if(skillArr != null ) {
                foreach(var h in skillArr ) {
                    result.Add(new PetActiveSkill(h));
                }
            }
            return result.ToArray();
        }

        public bool hasActiveSkills {
            get {
                if(activeSkills == null ) {
                    activeSkills = new PetActiveSkill[] { };
                }
                return (activeSkills.Length > 0);
            }
        }

        public bool hasMaxColor {
            get {
                return (color == nextColor);
            }
        }

        public bool hasMaxMastery {
            get {
                return (mastery == nextMastery);
            }
        }

        public PetColor nextColor {
            get {
                switch (color) {
                    case PetColor.gray:
                        return PetColor.white;
                    case PetColor.white:
                        return PetColor.blue;
                    case PetColor.blue:
                        return PetColor.yellow;
                    case PetColor.yellow:
                        return PetColor.green;
                    case PetColor.green:
                        return PetColor.orange;
                    case PetColor.orange:
                        return PetColor.orange;
                    default:
                        return PetColor.gray;

                }
            }
        }

        public int maxAllowedMastery {
            get {
                switch(color) {
                    case PetColor.gray:
                        return 1;
                    case PetColor.white:
                        return 2;
                    case PetColor.blue:
                        return 3;
                    case PetColor.yellow:
                        return 4;
                    case PetColor.green:
                        return 5;
                    case PetColor.orange:
                        return 6;
                    default:
                        return 6;
                }
            }
        }

        public PetColor RequiredColorForMastery(int mastery) {
            switch (mastery) {
                case 0:
                case 1:
                    return PetColor.gray;
                case 2:
                    return PetColor.white;
                case 3:
                    return PetColor.blue;
                case 4:
                    return PetColor.yellow;
                case 5:
                    return PetColor.green;
                case 6:
                    return PetColor.orange;
                default:
                    return PetColor.orange;
            }
        }

        public int nextMastery {
            get {
                if (mastery < 6) {
                    return mastery + 1;
                }
                return 6;
            }  
        }
    }
}

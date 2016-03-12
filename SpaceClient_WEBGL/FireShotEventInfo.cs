using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class FireShotEventInfo : IInfoParser {
        public string sourceID { get; private set; }
        public ItemType sourceType { get; private set; }
        public string targetID { get; private set; }
        public ItemType targetType { get; private set; }
        public Workshop sourceWorkshop { get; private set; }
        public int skillID { get; private set; }
        public bool isHitAllowed { get; private set; }
        public bool isHitted { get; private set; }
        public float damage { get; private set; }
        public bool isCritical { get; private set; }
        public string errorMessage { get; private set; }
        public float rocketDamage { get; private set; }
        public float acidDamage { get; private set; }
        public float laserDamage { get; private set; }
        public WeaponBaseType weaponBaseType { get; private set; }

        public void ParseInfo(Hashtable info) {
            sourceID = info.GetValueString((int)SPC.Source);
            sourceType = (ItemType)info.GetValueByte((int)SPC.SourceType, (byte)ItemType.Avatar);
            targetID = info.GetValueString((int)SPC.Target);
            targetType = (ItemType)info.GetValueByte((int)SPC.TargetType, (byte)ItemType.Avatar);
            sourceWorkshop = (Workshop)info.GetValueByte((int)SPC.Workshop, (byte)Workshop.Arlen);
            skillID = info.GetValueInt((int)SPC.Skill, -1);
            isHitAllowed = info.GetValueBool((int)SPC.FireAllowed, false);
            isHitted = info.GetValueBool((int)SPC.IsHitted, false);
            damage = info.GetValueFloat((int)SPC.ActualDamage, 0f);
            isCritical = info.GetValueBool((int)SPC.IsCritical, false);
            errorMessage = info.GetValueString((int)SPC.ErrorMessageId, string.Empty);
            rocketDamage = info.GetValueFloat((int)SPC.RocketDamage);
            acidDamage = info.GetValueFloat((int)SPC.AcidDamage);
            laserDamage = info.GetValueFloat((int)SPC.LaserDamage);
            weaponBaseType = (WeaponBaseType)info.GetValueInt((int)SPC.WeaponBaseType, (int)WeaponBaseType.Rocket);
        }

        public FireShotEventInfo(Hashtable info) {
            ParseInfo(info);
        }
    }
}

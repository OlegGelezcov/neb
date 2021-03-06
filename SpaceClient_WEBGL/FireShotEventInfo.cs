﻿using Common;
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
        //public bool isHitAllowed { get; private set; }
        //public bool isHitted { get; private set; }
        public float damage { get; private set; }
        //public bool isCritical { get; private set; }
        //public string errorMessage { get; private set; }
        public float rocketDamage { get; private set; }
        public float acidDamage { get; private set; }
        public float laserDamage { get; private set; }
        public WeaponBaseType weaponBaseType { get; private set; }
        public ShotState state { get; private set; }

        public void ParseInfo(Hashtable info) {
            sourceID = info.GetValueString((int)SPC.Source);
            sourceType = (ItemType)info.GetValueByte((int)SPC.SourceType, (byte)ItemType.Avatar);
            targetID = info.GetValueString((int)SPC.Target);
            targetType = (ItemType)info.GetValueByte((int)SPC.TargetType, (byte)ItemType.Avatar);
            sourceWorkshop = (Workshop)info.GetValueByte((int)SPC.Workshop, (byte)Workshop.Arlen);
            skillID = info.GetValueInt((int)SPC.Skill, -1);
            //isHitAllowed = info.GetValueBool((int)SPC.FireAllowed, false);
            //isHitted = info.GetValueBool((int)SPC.IsHitted, false);
            damage = info.GetValueFloat((int)SPC.ActualDamage, 0f);
            //isCritical = info.GetValueBool((int)SPC.IsCritical, false);
            //errorMessage = info.GetValueString((int)SPC.ErrorMessageId, string.Empty);
            rocketDamage = info.GetValueFloat((int)SPC.RocketDamage);
            acidDamage = info.GetValueFloat((int)SPC.AcidDamage);
            laserDamage = info.GetValueFloat((int)SPC.LaserDamage);
            weaponBaseType = (WeaponBaseType)info.GetValueInt((int)SPC.WeaponBaseType, (int)WeaponBaseType.Rocket);

            if (info.ContainsKey((int)SPC.ShotState)) {
                state = (ShotState)info.GetValueByte((int)SPC.ShotState);
            } else {
                state = ShotState.normal;
            }
        }

        public FireShotEventInfo(Hashtable info) {
            ParseInfo(info);
        }

        public bool hitted {
            get {
                return (state == ShotState.normal) || (state == ShotState.normalCritical);
            }
        }

        public bool critical {
            get {
                return (state == ShotState.normalCritical);
            }
        }

        public bool missed {
            get {
                return (state == ShotState.missed);
            }
        }
        public bool normalOrMissed {
            get {
                return hitted || missed;
            }
        }

        public static FireShotEventInfo CreateTest(int skillId) {
            Hashtable hash = new Hashtable {
                { (int)SPC.Source, System.Guid.NewGuid().ToString() },
                { (int)SPC.SourceType, (byte)ItemType.Avatar },
                { (int)SPC.Target, System.Guid.NewGuid().ToString() },
                { (int)SPC.TargetType, (byte)ItemType.Avatar },
                { (int)SPC.Workshop, (byte)Workshop.DarthTribe },
                { (int)SPC.Skill, skillId },
                //{ (int)SPC.FireAllowed, true },
                //{ (int)SPC.IsHitted, true },
                { (int)SPC.ActualDamage, 100.0f },
                //{ (int)SPC.IsCritical, false },
                //{ (int)SPC.ErrorMessageId, string.Empty },
                { (int)SPC.RocketDamage, 100.0f },
                { (int)SPC.AcidDamage, 0.0f },
                { (int)SPC.LaserDamage, 0.0f },
                { (int)SPC.WeaponBaseType, (int)WeaponBaseType.Rocket }
            };
            return new FireShotEventInfo(hash);
        }
    }
}

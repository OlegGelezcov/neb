using Common;
using System.Xml.Linq;
using ExitGames.Client.Photon;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class SimpleWeaponComponentData : MultiComponentData, IDatabaseComponentData{

        public float optimalDistance { get; private set; }
        public float damage { get; private set; }
        public float cooldown { get; private set; }
        public bool useTargetHPForDamage { get; private set; }
        public float targetHPPercentDamage { get; private set; }

        public SimpleWeaponComponentData(XElement e) {
            optimalDistance = e.GetFloat("optimal_distance");
            damage = e.GetFloat("damage");
            cooldown = e.GetFloat("cooldown");

            if(e.HasAttribute("use_target_hp"))
                useTargetHPForDamage = e.GetBool("use_target_hp");
            if(e.HasAttribute("target_hp_percent"))
                targetHPPercentDamage = e.GetFloat("target_hp_percent");
        }

        public SimpleWeaponComponentData(float optimalDistance, float damage, float cooldown, bool useTargetHPForDamage, float targetHPPercentDamage) {
            this.optimalDistance = optimalDistance;
            this.damage = damage;
            this.cooldown = cooldown;
            this.useTargetHPForDamage = useTargetHPForDamage;
            this.targetHPPercentDamage = targetHPPercentDamage;
        }

        public SimpleWeaponComponentData(Hashtable hash) {
            optimalDistance = hash.GetValue<float>((int)SPC.OptimalDistance, 0f);
            damage = hash.GetValue<float>((int)SPC.Damage, 0f);
            cooldown = hash.GetValue<float>((int)SPC.Cooldown, 0f);
            useTargetHPForDamage = hash.GetValue<bool>((int)SPC.UseTargetHpForDamage, false);
            targetHPPercentDamage = hash.GetValue<float>((int)SPC.TargetHpPercentDamage, 0f);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Weapon;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.weapon_simple;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                {(int)SPC.OptimalDistance, optimalDistance },
                {(int)SPC.Damage, damage  },
                {(int)SPC.Cooldown, cooldown },
                {(int)SPC.UseTargetHpForDamage, useTargetHPForDamage },
                {(int)SPC.TargetHpPercentDamage, targetHPPercentDamage}
            };
        }
    }
}

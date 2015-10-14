/*
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public struct WeaponShotInfo : IInfo
    {
        private float damage;
        private float cooldown;
        private float energy;
        private float critDamage;

        public WeaponShotInfo(float damage, float cooldown, float energy, float critDamage)
        {
            this.damage = damage;
            this.cooldown = cooldown;
            this.energy = energy;
            this.critDamage = critDamage;
        }

        public void SetCooldown(float cooldown)
        {
            this.cooldown = cooldown;
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }

        public float Damage
        {
            get
            {
                return this.damage;
            }
        }

        public float Cooldown
        {
            get
            {
                return this.cooldown;
            }
        }

        public float Energy
        {
            get
            {
                return energy;
            }
        }

        public float CritDamage
        {
            get
            {
                return this.critDamage;
            }
        }

        public Hashtable GetInfo()
        {
            return new Hashtable
            {
                {(int)SPC.Damage, this.damage},
                {(int)SPC.Cooldown, this.cooldown },
                {(int)SPC.Energy, this.energy },
                {(int)SPC.CritDamage, this.critDamage}
            };
        }

        public void ParseInfo(Hashtable info)
        {
            this.damage = info.GetValue<float>((int)SPC.Damage, 0f);
            this.cooldown = info.GetValue<float>((int)SPC.Cooldown, 0f);
            this.energy = info.GetValue<float>((int)SPC.Energy, 0f);
            this.critDamage = info.GetValue<float>((int)SPC.CritDamage, 0f);
        }
    }

}*/

/*
using Common;
using Space.Game.Inventory.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameMath;

namespace Space.Game
{
    public class NpcShipWeapon : IShipWeapon, IInfo
    {

        private ICombatActor owner;
        private float lastLightTime;
        private bool ready;
        private float hitProb;
        private WeaponObject weaponObject;

        public NpcShipWeapon(ICombatActor actor)
        {
            this.owner = actor;
            this.lastFireTime = 0.0f;
        }

        public void Initialize(WeaponObject weaponObject)
        {
            this.weaponObject = weaponObject;
            this.Update();
        }

        public WeaponObject SetWeapon(WeaponObject weaponObject)
        {
            WeaponObject oldWeapon = this.weaponObject;
            this.weaponObject = weaponObject;
            this.Update();
            return oldWeapon;
        }

        public bool HasWeapon()
        {
            return this.weaponObject != null;
        }

        public void SetLastFireTime(float time)
        {
            this.lastFireTime = time;
            this.Update();
        }

        public float LastFireTime
        {
            get { return this.lastFireTime; }
        }

        public bool Ready
        {
            get
            {
                this.Update();
                return this.ready;
            }
        }

        public float Damage
        {
            get
            {
                if(this.HasWeapon())
                {
                    return this.weaponObject.Damage;
                }
                return 0.0f;
            }
        }

        public float Range
        {
            get
            {
                if(this.HasWeapon() && this.owner.Check<ICombatActor>() )
                {
                    return this.weaponObject.Range;
                }
                return 0.0f;
            }
        }

        public float Distance
        {
            get
            {
                if(this.HasWeapon() && this.owner.Check<ICombatActor>())
                {
                    float t = Time.time;;
                    return this.weaponObject.Distance * this.owner.Bonuses.OptimalDistanceBonusesValue(t);
                }
                return 0.0f;
            }
        }


        public float MaxSpeed
        {
            get
            {
                if(this.HasWeapon() && this.owner.Check<ICombatActor>())
                {
                    return this.weaponObject.MaxSpeed;
                }
                return 0.0f;
            }
        }

        public float Cooldown
        {
            get
            {
                if(this.HasWeapon() && this.owner.Check<ICombatActor>())
                {
                    float t = Time.time;
                    return this.weaponObject.Cooldown * this.owner.Bonuses.CooldownBonusesValue(t);
                }
                return 1e6f;
            }
        }

        public float HitProb
        {
            get
            {
                if(this.HasWeapon() && this.owner.Check<ICombatActor>())
                {
                    float t = Time.time;
                    return this.hitProb * this.owner.Bonuses.HitChanceBonusesValue(t);
                }
                return 0.0f;
            }
        }

        public WeaponObject Weapon
        {
            get
            {
                return this.weaponObject;
            }
        }


        public void Update()
        {
            if(this.HasWeapon() && this.owner.Check<ICombatActor>() )
            {
                float t = Time.time;
                this.ready = (t > this.lastFireTime + this.Cooldown);

                if (this.owner.Target.TargetIsValidCombatActor())
                {
                    this.hitProb = GameBalance.ComputeHitProb(this.Distance, this.Range, this.owner.Target.Distance(), this.MaxSpeed, this.owner.Target.Speed());
                }
                else
                {
                    this.hitProb = 0.0f;
                }

                this.SetOwnerProperty(GroupProps.SHIP_WEAPON_STATE, Props.SHIP_WEAPON_STATE_READY, this.ready);
                this.SetOwnerProperty(GroupProps.SHIP_WEAPON_STATE, Props.SHIP_WEAPON_STATE_CURRENT_RELOAD_TIMER, (t - this.lastFireTime));
                this.SetOwnerProperty(GroupProps.SHIP_WEAPON_STATE, Props.SHIP_WEAPON_STATE_HIT_PROB, this.HitProb);
                this.SetOwnerProperty(GroupProps.SHIP_WEAPON_STATE, Props.SHIP_WEAPON_STATE_RANGE, this.Range);
                this.owner.UpdateProperties();
            }
        }


        public WeaponHitInfo TryHit()
        {
            this.Update();
            float prob = this.HitProb;
            prob = Mathf.Clamp01(prob);

            if(Rand.Float01() < prob )
            {
                return new WeaponHitInfo(true, prob, true, false);
            }
            else
            {
                return new WeaponHitInfo(true, prob, false, false);
            }
        }


        private void SetOwnerProperty(string group, string name, object value )
        {
            if(this.owner != null )
            {
                this.owner.SetProperty(group, name, value);
            }
        }

        public Hashtable GetInfo()
        {
            this.Update();
            Hashtable result = new Hashtable();
            result.Add(GenericEventProps.has_weapon, this.HasWeapon());
            result.Add(GenericEventProps.ready, this.Ready);
            result.Add(GenericEventProps.damage, this.Damage);
            result.Add(GenericEventProps.range, this.Range);
            result.Add(GenericEventProps.cooldown, this.Cooldown);
            result.Add(GenericEventProps.hit_prob, this.HitProb);
            result.Add(GenericEventProps.timer, Time.time - this.lastFireTime);
            result.Add(GenericEventProps.source, this.HasWeapon() ? this.weaponObject.GetInfo() : new Hashtable());
            result.Add(GenericEventProps.optimal_distance, this.Distance);
            return result;
        }

        public void ParseInfo(Hashtable info)
        {
            throw new NotImplementedException("NpcShipWeapon.ParseInfo");
        }


        public float CriticalChance
        {
            get 
            {
                if (this.HasWeapon() == false)
                    return 0.0f;

                return this.weaponObject.CritChance * this.owner.Bonuses.CriticalChanceBonusesValue(Time.time);
            }
        }

        public float CriticalDamage
        {
            get 
            {
                if (this.HasWeapon() == false)
                    return 0.0f;
                return this.weaponObject.CritDamage * this.owner.Bonuses.CriticalDamageBonusValue(Time.time);
            }
        }
    }
}
*/
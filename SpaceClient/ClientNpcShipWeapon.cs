using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client
{
    
    public class ClientNpcShipWeapon : IInfo
    {
        private float cooldown;
        private float lastFireTime;
        private float damage;
        private bool ready;
        private float hitProb;
        private float optimalDistance;
        private float range;
        private float farProb;
        private float nearProb;
        private float maxHitSpeed;
        private float maxFireDistance;
        private float time;
        private float farDist;
        private float nearDist;

        public float Cooldown { get { return this.cooldown; } }
        public float LastFireTime { get { return this.lastFireTime; } }
        public float Damage { get { return this.damage; } }
        public bool Ready { get { return this.ready; } }
        public float HitProb { get { return this.hitProb; } }
        public float OptimalDistance { get { return this.optimalDistance; } }
        public float Range { get { return this.range; } }
        public float FarProb { get { return this.farProb; } }
        public float NearProb { get { return this.nearProb; } }
        public float MaxHitSpeed { get { return this.maxHitSpeed; } }
        public float MaxFireDistance { get { return this.maxFireDistance; } }
        public float Time { get { return this.time; } }

        public float FarDist { get { return this.farDist; } }
        public float NearDist { get { return this.nearDist; } }

        public ClientNpcShipWeapon()
        {

        }

        public ClientNpcShipWeapon(Hashtable info )
        {
            this.ParseInfo(info);
        }

        public Hashtable GetInfo()
        {
            return new Hashtable();
        }

        public void ParseInfo(Hashtable info)
        {
            this.cooldown = info.GetValue<float>((int)SPC.Cooldown, 0.0f);
            this.lastFireTime = info.GetValue<float>((int)SPC.LastFireTime, 0.0f);
            this.damage = info.GetValue<float>((int)SPC.Damage, 0.0f);
            this.ready = info.GetValue<bool>((int)SPC.Ready, false);
            this.hitProb = info.GetValue<float>((int)SPC.HitProb, 0.0f);
            this.optimalDistance = info.GetValue<float>((int)SPC.OptimalDistance, 0.0f);
            this.range = info.GetValue<float>((int)SPC.Range, 0.0f);
            this.farProb = info.GetValue<float>((int)SPC.FarProb, 0.0f);
            this.nearProb = info.GetValue<float>((int)SPC.NearProb, 0.0f);
            this.farDist = info.GetValue<float>((int)SPC.FarDist, 0.0f);
            this.nearDist = info.GetValue<float>((int)SPC.NearDist, 0.0f);
            this.maxHitSpeed = info.GetValue<float>((int)SPC.MaxHitSpeed, 0.0f);
            this.maxFireDistance = info.GetValue<float>((int)SPC.MaxFireDistance, 0.0f);
            this.time = info.GetValue<float>((int)SPC.Time, 0.0f);
        }
    }

}

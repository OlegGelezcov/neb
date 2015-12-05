using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client {

    public class ClientNpcShipWeapon : IInfo {
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

        public ClientNpcShipWeapon() {

        }

        public ClientNpcShipWeapon(Hashtable info) {
            this.ParseInfo(info);
        }

        public Hashtable GetInfo() {
            return new Hashtable();
        }

        public void ParseInfo(Hashtable info) {
            this.cooldown = info.GetValueFloat((int)SPC.Cooldown);
            this.lastFireTime = info.GetValueFloat((int)SPC.LastFireTime);
            this.damage = info.GetValueFloat((int)SPC.Damage);
            this.ready = info.GetValueBool((int)SPC.Ready);
            this.hitProb = info.GetValueFloat((int)SPC.HitProb);
            this.optimalDistance = info.GetValueFloat((int)SPC.OptimalDistance);
            this.range = info.GetValueFloat((int)SPC.Range);
            this.farProb = info.GetValueFloat((int)SPC.FarProb);
            this.nearProb = info.GetValueFloat((int)SPC.NearProb);
            this.farDist = info.GetValueFloat((int)SPC.FarDist);
            this.nearDist = info.GetValueFloat((int)SPC.NearDist);
            this.maxHitSpeed = info.GetValueFloat((int)SPC.MaxHitSpeed);
            this.maxFireDistance = info.GetValueFloat((int)SPC.MaxFireDistance);
            this.time = info.GetValueFloat((int)SPC.Time);
        }
    }

}

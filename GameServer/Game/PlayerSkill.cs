namespace Nebula.Game.Components {
    using Common;
    using GameMath;
    using global::Common;
    using Nebula.Engine;
    using Nebula.Game.Bonuses;
    using Nebula.Game.Skills;
    using ServerClientCommon;
    using Space.Game;
    using Space.Game.Resources;
    using Space.Game.Skills;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class PlayerSkill: IInfoSource
    {
        private bool mIsOn = false;

        public SkillData data { get; private set; }



        public bool isOn {
            get {
                if(data.IsEmpty || (data.Type != SkillType.Persistent)) {
                    mIsOn = false;
                }
                return mIsOn;
            }
            set {
                if(data.IsEmpty || (data.Type != SkillType.Persistent)) {
                    mIsOn = false;
                } 
                if(data.Type == SkillType.Persistent) {
                    mIsOn = value;
                }
            }
        }

        public string id {
            get {
                return idInt.ToString();
            }
        }

        public int idInt {
            get {
                return data.Id;
            }
        }

        private float mTimer;

        private float cooldown {
            get {
                float pcBonus = 0f;
                float cntBonus = 0f;
                if(skills && skills.bonuses) {
                    pcBonus = skills.bonuses.cooldownPcBonus;
                    cntBonus = skills.bonuses.cooldownCntBonus;
                }
                return Mathf.ClampLess(data.Cooldown * (1f + pcBonus) + cntBonus, 0);
            }
        }

        private PlayerSkills skills { get; set; }


        public PlayerSkill(PlayerSkills inSkills) {
            skills = inSkills;
            this.data = SkillData.Empty;
            this.isOn = false;
        }

        public void SetData(SkillData data) {
            this.data = data;
        }

        public void SetOn(bool value) {
            this.isOn = value;
        }

        public bool ready {
            get {
                return mTimer <= 0f;
            }
        }


        public bool Use(NebulaObject target) {
            if (data.IsEmpty) {
                return false;
            }
            if(skills.blocked ) {
                return false;
            }
            //if (target) {
            //    if (false == target.GetComponent<DamagableObject>()) {
            //        return false;
            //    }
            //    if (false == target.GetComponent<PlayerBonuses>()) {
            //        return false;
            //    }
            //}

            switch(data.Type) {
                case SkillType.Durable:
                    return UseDurable(target);
                case SkillType.OneUse:
                    return UseInstant(target);
                case SkillType.Persistent:
                    return UsePersistent(target);
                default:
                    return false;
            }
        }

        private void SetEnergyDebuff(NebulaObject target, int skillID, float energyMOD) {
            string buffID = target.Id + skillID;

            if (target != null) {
                if (target.Skills() && target.Bonuses()) {
                    Buff buff = new Buff(buffID, target, BonusType.decrease_max_energy_on_cnt, -1, energyMOD, () => true, skillID);

                    target.GetComponent<PlayerBonuses>().SetBuff(buff);
                }
            }
        }

        private void RemoveEnergyBuff(NebulaObject target, int skillID) {
            string buffID = target.Id + skillID;
            if (target != null && target.Bonuses() != null) {
                target.GetComponent<PlayerBonuses>().RemoveBuff(BonusType.decrease_max_energy_on_cnt, buffID);
            }
        }

        private bool UsePersistent(NebulaObject target) {
            if(isOn) {
                isOn = false;
                var energy = target.GetComponent<ShipEnergyBlock>();
                RemoveEnergyBuff(target, data.Id);

                if (skills.message) {
                    skills.message.PublishSkillUsed( EventReceiver.OwnerAndSubscriber, target, this, true, "off", new Hashtable());
                }
                return true;
            } else {
                if(ready) {
                    var energyComponent = skills.GetComponent<ShipEnergyBlock>();
                    if(energyComponent.currentEnergy >= data.RequiredEnergy ) {

                        Hashtable result = new Hashtable();
                        isOn = true;

                        if (GetExecutor().TryCast(skills.nebulaObject, this, out result)) {
                            SetEnergyDebuff(target, data.Id, data.RequiredEnergy);
                            mTimer = cooldown;
                            if (skills.message) {
                                skills.message.PublishSkillUsed(EventReceiver.OwnerAndSubscriber, target, this, true, "on", result);
                            }
                            skills.SetLastSkill(data.Id);
                            return true;
                        } else {
                            isOn = false;
                            if (skills.message) {
                                skills.message.PublishSkillUsed(EventReceiver.OwnerAndSubscriber, target, this, false, "fail", result);
                            }
                            return false;
                        }

                    } else {
                        if(skills.message) {
                            skills.message.PublishSkillUsed( EventReceiver.ItemOwner, target, this, false, "energy", new Hashtable());
                        }
                        return false;
                    }
                } else {
                    if (skills.message) {
                        skills.message.PublishSkillUsed( EventReceiver.ItemOwner, target, this, false, "not ready", new Hashtable());
                    }
                    return false;
                }
            }
        }

        private bool UseDurable(NebulaObject target ) {
            if(!ready) {
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.ItemOwner, target, this, false, "not ready", new Hashtable());
                }
                return false;
            }

            var energyComponent = skills.GetComponent<ShipEnergyBlock>();
            if(energyComponent.currentEnergy < data.RequiredEnergy) {
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.ItemOwner, target, this, false, "energy", new Hashtable());
                }
                return false;
            }

            Hashtable result = new Hashtable();
            if (GetExecutor().TryCast(skills.nebulaObject, this, out result)) {
                energyComponent.RemoveEnergy(data.RequiredEnergy);
                mTimer = cooldown;
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.OwnerAndSubscriber, target, this, true, "durable ok", result);
                }
                skills.SetLastSkill(data.Id);
                return true;
            } else {
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.ItemOwner, target, this, false, "fail", result);
                }
                return false;
            }
        }

        public bool energyOk {
            get {
                if(skills.energy) {
                    return skills.energy.currentEnergy > data.RequiredEnergy;
                }
                return false;
            }
        }
        private bool UseInstant(NebulaObject target) {
            if (!ready) {
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.ItemOwner, target, this, false, "not ready", new Hashtable());
                }
                return false;
            }

            var energyComponent = skills.GetComponent<ShipEnergyBlock>();
            if (energyComponent.currentEnergy < data.RequiredEnergy) {
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.ItemOwner, target, this, false, "energy", new Hashtable());
                }
                return false;
            }

            Hashtable result = new Hashtable();
            if (GetExecutor().TryCast(skills.nebulaObject, this, out result)) {
                energyComponent.RemoveEnergy(data.RequiredEnergy);
                mTimer = cooldown;
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.OwnerAndSubscriber, target, this, true, "instant ok", result);
                }
                skills.SetLastSkill(data.Id);
                return true;
            } else {
                if (skills.message) {
                    skills.message.PublishSkillUsed(EventReceiver.ItemOwner, target, this, false, "fail", result);
                }
                return false;
            }
        }


        private readonly Dictionary<int, SkillExecutor> mCachedExecutors = new Dictionary<int, SkillExecutor>();

        public SkillExecutor GetExecutor() {
            if(!mCachedExecutors.ContainsKey(data.Id)) {
                mCachedExecutors.Add(data.Id, SkillExecutor.Factory(data.Id)());
            }
            return mCachedExecutors[data.Id];
           // return SkillExecutor.Executor(data.Id);
        }

        public void Update( float deltaTime) {
            if (data.IsEmpty) {
                return;
            }

           // GetExecutor().Update(skills, deltaTime);

            mTimer -= deltaTime;
            mTimer = Mathf.ClampLess(mTimer, -1);
        }



        public static PlayerSkill Empty(PlayerSkills skills)
        {
            return new PlayerSkill(skills);
        }

        public bool IsEmpty
        {
            get
            {
                return data.IsEmpty;
            }
        }

        public void OnSourceFire(NebulaObject sourcePlayer)
        {
            if(this.data != null && this.data.IsEmpty == false)
            {

            }
        }

        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();

            info.Add((int)SPC.Id, (this.data != null) ? this.data.Id : -1);
            //has or not skill
            info.Add((int)SPC.HasSkill, ((this.data != null) && (this.data.IsEmpty == false)));
            //skill on or off currently
            info.Add((int)SPC.IsOn, (this.data != null) ? this.isOn : false);
            //skill timer
            info.Add((int)SPC.Timer, mTimer );
            //skill cooldown( may differ from skill data cooldown)
            info.Add((int)SPC.Cooldown, cooldown);
            //skill type
            info.Add((int)SPC.SkillType, (this.data != null) ? (byte)this.data.Type : (byte)SkillType.OneUse);
            return info;
        }

        public Hashtable DumpInfo() {
            var hash = new Hashtable();
            if(data != null ) {
                hash.Add("data", data.GetInfo());
                if(data.Id >= 0) {
                    hash.Add("ID", data.Id.ToString("X8"));
                } else {
                    hash.Add("ID", data.Id.ToString());
                }
            } else {
                hash.Add("ID", data.Id.ToString());
            }
            hash.Add("is_on", isOn);
            hash.Add("timer", mTimer);
            hash.Add("cooldown", cooldown);

            return hash;
        }

        public T GetDataInput<T>(string key, T defaultValue) {
            return data.Inputs.GetValue<T>(key, defaultValue);
        }

        public float GetFloatInput(string key) {
            return GetDataInput<float>(key, 0f);
        }

        public int GetIntInput(string key) {
            return GetDataInput<int>(key, 0);
        }
    }
}

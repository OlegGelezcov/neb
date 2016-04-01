using Space.Game.Resources;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using Nebula.Engine;
using Space.Game.Skills;
using Space.Game;
using Space.Database;
using Nebula.Server.Components;
using System.Collections.Concurrent;
using Nebula.Game.Skills;
using GameMath;
using ExitGames.Logging;
using Nebula.Game.Bonuses;
using Nebula.Database;
using Nebula.Drop;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    [REQUIRE_COMPONENT(typeof(MmoMessageComponent))]
    [REQUIRE_COMPONENT(typeof(ShipEnergyBlock))]
    public class PlayerSkills : NebulaBehaviour, IInfo
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private const string RESPAWN_SKILL_ID = "000003F9";
        private const string SKILL_00000435 = "00000435";

        private const string INCREASE_MAX_HP_FOR_CRIT_ID = "000003FC";
        private const int INCREASE_MAX_HP_FOR_CRIT_BUFF_TAG = 2001;
        private const string MOVE_DAMAGE_FROM_TARGET_TO_ME_SKILL_ID = "00000404";
        private const string SKILL_00000415 = "00000415";
        private const string SKILL_00000417 = "00000417";
        private const string SKILL_0000041C = "0000041C";
        private const string SKILL_00000453 = "00000453";


        private readonly ActiveSkillUpdater m3F7 = new ActiveSkillUpdater();
        private readonly ActiveSkillUpdater m3F8 = new ActiveSkillUpdater();
        private readonly ActiveSkillUpdater m404 = new ActiveSkillUpdater();
        private readonly ActiveSkillUpdater m415 = new ActiveSkillUpdater();
        private readonly ActiveSkillUpdater m41C = new ActiveSkillUpdater();
        private readonly ActiveSkillUpdater m432 = new ActiveSkillUpdater();
        private int m439ID;
        private int m43AID;
        private int m451ID;
        private int m456ID;
        private int m45CID;

        public class DamageReceiver {
            public DamagableObject target { get; set; }
            public float expireTime { get; set; }
            public float damagePercent { get; set; }
        }

        private ConcurrentDictionary<string, DamageReceiver> mDamageHealReceivers = new ConcurrentDictionary<string, DamageReceiver>();

        private Dictionary<int, PlayerSkill> skills;
        private float mUpdateModelSkillsTimer = 10f;

        //link to player ship(not bot)
        private BaseShip mPlayerShip;
        public PlayerBonuses bonuses { get; private set; }

        public MmoMessageComponent message { get; private set; }
        public ShipEnergyBlock energy { get; private set; }

        //last skill was successfully used
        private int mLastSkill = -1;

        //number of uses in sequence one skill
        private int mSequenceSkillCounter = 0;
        private DamagableObject mDamagable;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            foreach(var pSkill in skills) {
                if(pSkill.Value != null ) {
                    hash.AddOrReplace(pSkill.Key.ToString(), pSkill.Value.DumpInfo());
                } else {
                    hash.AddOrReplace(pSkill.Key.ToString(), new Hashtable());
                }
            }
            return hash;
        }

        public int GetSlotBySkillId(int skill) {
            foreach(var kvp in Skills ) {
                if(kvp.Value != null ) {
                    if(kvp.Value.data != null ) {
                        if(kvp.Value.data.Id == skill ) {
                            return kvp.Key;
                        }
                    }
                }
            }
            return -1;
        }

        public void Init(SkillsComponentData data) {

        }

        public override void Awake() {
            this.skills = new Dictionary<int, PlayerSkill>{
                {0, PlayerSkill.Empty(this)},
                {1, PlayerSkill.Empty(this)},
                {2, PlayerSkill.Empty(this)},
                {3, PlayerSkill.Empty(this)},
                {4, PlayerSkill.Empty(this)},
                {5, PlayerSkill.Empty(this)}
            };
        }

        private bool started { get; set; } = false;

        public override void Start() {
            mPlayerShip = GetComponent<BaseShip>();
            bonuses = GetComponent<PlayerBonuses>();
            mDamagable = GetComponent<DamagableObject>();
            message = GetComponent<MmoMessageComponent>();
            energy = GetComponent<ShipEnergyBlock>();

            m439ID = SkillExecutor.SkilIDFromHexString("00000439");
            m43AID = SkillExecutor.SkilIDFromHexString("0000043A");
            m451ID = SkillExecutor.SkilIDFromHexString("00000451");
            m456ID = SkillExecutor.SkilIDFromHexString("00000456");
            m45CID = SkillExecutor.SkilIDFromHexString("0000045C");
            started = true;
        }

        public void Load() {
            log.InfoFormat("PlayerSkills Load() [dy]");

            if (GetComponent<MmoActor>()) {
                string characterID = (string)nebulaObject.Tag((byte)PlayerTags.CharacterId);

                var app = nebulaObject.mmoWorld().application;
                bool isNew = false;
                var dbSkills = SkillDatabase.instance(app).LoadSkills(characterID, resource as Res, out isNew);
                if(!isNew) {
                    if(dbSkills.skills != null ) {
                        Parse(dbSkills.skills);
                    }
                }

                UpdateSkills(GetComponent<PlayerShip>().shipModel);
                GetComponent<MmoActor>().EventOnSkillsUpdated();
            }
        }

        public int skillCount {
            get {
                int count = 0;
                foreach(var s in skills) {
                    if(s.Value.IsEmpty == false) {
                        count++;
                    }
                }
                return count;
            }
        }

        public bool SetSkill( int index, int skillId)
        {
            if(index >= 0 && index < 6 )
            {
                this.skills[index].SetData(nebulaObject.world.Resource().Skills.Skill(skillId));
                return true;
            }
            return false;
        }

        public bool HasSkill(int ID) {
            if(ID == -1) { return false; }
            foreach(var skill in skills) {
                if(skill.Value.data.Id == ID) {
                    return true;
                }
            }
            return false;
        }

        public PlayerSkill GetSkillByPosition(int index)
        {
            if(index >= 0 && index < 6)
            {
                return this.skills[index];
            }
            return PlayerSkill.Empty(this);
        }

        public PlayerSkill GetSkillById(int id) {
            if(id == -1) {
                return null;
            }
            foreach(var skillPair in skills) {
                if(skillPair.Value.data.Id == id) {
                    return skillPair.Value;
                } 
            }
            return null;
        }

        public bool UseSkill(int index, NebulaObject target) {
            if (index >= 0 && index < 6) {
                return this.skills[index].Use(target);
            }
            return false;
        }

        public override void Update(float deltaTime) {

            if (nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }

           foreach(var skill in skills) {
                skill.Value.Update(deltaTime);
            }

            mUpdateModelSkillsTimer -= deltaTime;
            if(mUpdateModelSkillsTimer <= 0f) {
                mUpdateModelSkillsTimer = 10;
                if(mPlayerShip) {
                    UpdateSkills(mPlayerShip.shipModel);
                }
            }

            m3F7.Update(deltaTime);
            m3F8.Update(deltaTime);
            m404.Update(deltaTime);
            m415.Update(deltaTime);
            m41C.Update(deltaTime);
            m432.Update(deltaTime);
            //m3FD.Update(deltaTime);
        }

        private void UpdateSkillAtIndex(int index, ShipModel model) {
            var world = nebulaObject.world as MmoWorld;
            var resSkills = world.Resource().Skills;

            if (model.SlotForSkillIndex(index).HasModule) {
                if (model.SlotForSkillIndex(index).Module.HasSkill) {
                    if (skills[index].IsEmpty || skills[index].data.Id != model.SlotForSkillIndex(index).Module.Skill) {
                        skills[index].SetData(resSkills.Skill(model.SlotForSkillIndex(index).Module.Skill));
                    }
                } else {
                    if (!skills[index].IsEmpty) {
                        skills[index].SetData(SkillData.Empty);
                    }
                }
            }
        }

        public void UpdateSkills(ShipModel model)
        {
            if (!started) {
                Start();
            }

            for(int i = 0; i < 5; i++) {
                UpdateSkillAtIndex(i, model);
            }

            if(model.Sets.Skill != -1 && (skills[5].data.Id != model.Sets.Skill)) {
                skills[5].SetData(nebulaObject.resource.Skills.Skill(model.Sets.Skill));
            } else if(model.Sets.Skill == -1 && (!skills[5].IsEmpty)) {
                skills[5].SetData(SkillData.Empty);
            }
        }

        public List<int> slotsWithSkill {
            get {
                List<int> slots = new List<int>();
                foreach(var ps in Skills) {
                    if( (false ==ps.Value.IsEmpty)) {
                        slots.Add(ps.Key);
                    }
                }
                return slots;
            }
        }

        public Dictionary<int, PlayerSkill> Skills
        {
            get
            {
                return this.skills;
            }
        }

        public void Parse(Dictionary<int, int> savedSkills)
        {
            if (savedSkills == null)
            {
                return;
            }
            foreach(var s in savedSkills )
            {
                if (s.Key >= 0 && s.Key < 6)
                {
                    this.skills[s.Key].SetData(nebulaObject.world.Resource().Skills.Skill(s.Value));
                }
            }
        }

        public void Replace(PlayerSkills other)
        {
            foreach(var s in other.Skills)
            {
                this.skills[s.Key].SetData(s.Value.data);
                this.skills[s.Key].SetOn(false);
            }
        }

        /// <summary>
        /// Contains or not skill with skillId in slots
        /// </summary>
        /// <param name="skillId">Skill id to find in slots</param>
        /// <returns></returns>
        public bool Contains(int skillId)
        {
            foreach(var pair in this.skills)
            {
                if(pair.Value.data.Id == skillId)
                {
                    return true;
                }
            }
            return false;
        }



        public Hashtable GetInfo()
        {
            var result = new Hashtable();
            foreach (var pair in this.skills)
            {
                result.Add(pair.Key, pair.Value.GetInfo());
            }
            return result;
        }

        public void ParseInfo(Hashtable info)
        {
            foreach (DictionaryEntry entry in info)
            {
                int index = (int)entry.Key;
                int skillId = (int)entry.Value;
                SkillData skillData = nebulaObject.world.Resource().Skills.Skill(skillId);
                this.skills[index].SetData(skillData);
            }
        }

        public int SkillIndex(int skillId )
        {
            int index = -1;
            foreach( var s in this.skills)
            {
                if(s.Value.data.Id == skillId )
                {
                    index = s.Key;
                }
            }
            return index;
        }

        public void OnSourceFire()
        {
            if (this.skills != null)
            {
                foreach (var skill in this.skills)
                {
                    if (skill.Value != null)
                    {
                        skill.Value.OnSourceFire(nebulaObject);
                    }
                }
            }
        }

        public PlayerSkillsSave GetSave() {
            Dictionary<int, int> saveSkills = new Dictionary<int, int>();
            foreach(var ps in skills ) {
                saveSkills.Add(ps.Key, ps.Value.data.Id);
            }

            return new PlayerSkillsSave {
                characterID = GetComponent<PlayerCharacterObject>().characterId,
                skills = saveSkills
            };
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Skills;
            }
        }

        public void SetLastSkill(int lastSkill) {
            if(mLastSkill == lastSkill ) {
                mSequenceSkillCounter++;
            } else {
                mLastSkill = lastSkill;
                mSequenceSkillCounter = 1;
            }
        }

        public int lastSkill {
            get {
                return mLastSkill;
            }
        }

        public int sequenceSkillCounter {
            get {
                return mSequenceSkillCounter;
            }
        }

        public void SetSkillCounter(int skillCounter) {
            mSequenceSkillCounter = skillCounter;
        }

        public void AddDamageHealReceiver(DamageReceiver receiver) {
            if(mDamageHealReceivers.ContainsKey(receiver.target.nebulaObject.Id)) {
                DamageReceiver old;
                if(mDamageHealReceivers.TryRemove(receiver.target.nebulaObject.Id, out old)) {
                    mDamageHealReceivers.TryAdd(receiver.target.nebulaObject.Id, receiver);
                }
            }
        }

        /// <summary>
        /// Called when my health restored
        /// </summary>
        /// <param name="sourceObject">Object which restore health to me</param>
        /// <param name="hp">Number of hp restored</param>
        public void OnHealthRestored(NebulaObject sourceObject, float hp) {
            ConcurrentBag<string> removedKeys = new ConcurrentBag<string>();
            float time = Time.curtime();

            foreach(var pair in mDamageHealReceivers) {
                if(time > pair.Value.expireTime || (!pair.Value.target)) {
                    removedKeys.Add(pair.Key);
                }
            }

            if(removedKeys.Count > 0 ) {
                foreach(var id in removedKeys) {
                    DamageReceiver old;
                    mDamageHealReceivers.TryRemove(id, out old);
                }
            }

            var character = GetComponent<CharacterObject>();
            var race = GetComponent<RaceableObject>();

            foreach(var pair in mDamageHealReceivers) {
                float damage = hp * pair.Value.damagePercent;

                //pair.Value.target.ReceiveDamage(nebulaObject.Type, nebulaObject.Id, damage, character.workshop, character.level, race.race);
                if(pair.Value.target) {
                    WeaponHitInfo hit;
                    var shot = nebulaObject.Weapon().Fire(pair.Value.target.nebulaObject, out hit, SkillExecutor.SkilIDFromHexString("000007D5"), damage,  true, true);
                    nebulaObject.MmoMessage().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                }
                
            }

            if(sourceObject.Id != nebulaObject.Id) {
                if(HasSkill(m451ID)) {
                    var skill = GetSkillById(m451ID);
                    if(skill.isOn) {
                        float resistPc = skill.GetFloatInput("resist_pc");
                        float resistTime = skill.GetFloatInput("resist_time");
                        int stackCount = skill.GetIntInput("stack_count");

                        if(bonuses.GetBuffCountWithTag( BonusType.increase_resist_on_pc, m451ID) < stackCount) {
                            Buff buff = new Buff(Guid.NewGuid().ToString(), null, BonusType.increase_resist_on_pc, resistTime, resistPc);
                            buff.SetTag(m451ID);
                            bonuses.SetBuff(BonusType.increase_resist_on_pc, buff);
                        }
                    }
                }
            }
        }

        //Check for skill "000003F9" - when give chance regenerate health when object killed, check before sending OnWasKilled
        public bool RespawnBySkill() {

            log.InfoFormat("PlayerSkills.RespwanBySkill(): check blue");

            //only for players
            if(nebulaObject.IsPlayer()) {

                int skillID = SkillExecutor.SkilIDFromHexString(RESPAWN_SKILL_ID);

                //we must have this skill and this skill is on
                if(HasSkill(skillID)) {
                    var skill = GetSkillById(skillID);
                    if(skill.isOn) {
                        float respawnProb = skill.data.Inputs.GetValue<float>("resurrect_pc", 0f);
                        float regeneratedHp = skill.data.Inputs.GetValue<float>("hp_regen_pc", 0f);
                        float randomNumber = Rand.Float01();
                        log.InfoFormat("Respawn by skill check, respawn prob = {0:F1}, random number = {1:F1} blue", respawnProb, randomNumber);
                        if(randomNumber < respawnProb) {
                            var damagable = nebulaObject.Damagable();
                            damagable.ForceSetHealth(damagable.maximumHealth * regeneratedHp);
                            nebulaObject.MmoMessage().SendResurrect();
                            return true;
                        }
                    }
                }

                int skill417ID = SkillExecutor.SkilIDFromHexString(SKILL_00000417);
                if(HasSkill(skill417ID)) {
                    var skill = GetSkillById(skill417ID);
                    if(skill.isOn) {
                        float respawnProb = skill.data.Inputs.GetValue<float>("resurrect_pc", 0f);
                        float regeneratedHp = skill.data.Inputs.GetValue<float>("hp_regen_pc", 0f);
                        float randomNumber = Rand.Float01();
                        if(randomNumber < respawnProb) {
                            var damagable = nebulaObject.Damagable();
                            damagable.ForceSetHealth(damagable.maximumHealth * regeneratedHp);
                            nebulaObject.MmoMessage().SendResurrect();
                            return true;
                        }
                    }
                }

                int s435 = SkillExecutor.SkilIDFromHexString(SKILL_00000435);
                if(HasSkill(s435)) {
                    var skill = GetSkillById(s435);
                    if(skill.isOn) {
                        var resurrectProb = skill.GetFloatInput("resurrect_pc");
                        var hpPc = skill.GetFloatInput("hp_regen_pc");
                        if(Rand.Float01() < resurrectProb) {
                            var damagable = nebulaObject.Damagable();
                            damagable.ForceSetHealth(damagable.maximumHealth * hpPc);
                            nebulaObject.MmoMessage().SendResurrect();
                            return true;
                        }
                    }
                }

                int s453 = SkillExecutor.SkilIDFromHexString(SKILL_00000453);
                if(HasSkill(s453)) {
                    var skill = GetSkillById(s453);
                    if(skill.isOn) {
                        var resurrectProb = skill.GetFloatInput("resurrect_pc");
                        var hpPc = skill.GetFloatInput("hp_regen_pc");
                        if(Rand.Float01() < resurrectProb) {
                            var damagable = nebulaObject.Damagable();
                            damagable.ForceSetHealth(damagable.maximumHealth * hpPc);
                            nebulaObject.MmoMessage().SendResurrect();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //message handler
        public void OnEnemyDeath(NebulaObject enemy) {
            log.InfoFormat("Message = {0} yellow", ComponentMessages.OnEnemyDeath);
            //Handle skill 3F7 when enemy dead
            if(m3F7.active) {
                var damagable = nebulaObject.Damagable();
                if(damagable) {
                    damagable.RestoreHealth(nebulaObject, damagable.maximumHealth * m3F7.value);
                }
            }


            if(m3F8.active) {
                var damagable = nebulaObject.Damagable();
                if (damagable) {
                    damagable.RestoreHealth(nebulaObject, damagable.maximumHealth * m3F8.value);
                    //3F8 don't stack
                    m3F8.Deactivate();
                }
            }

            //use 439 skill
            if(HasSkill(m439ID)) {
                var skill = GetSkillById(m439ID);
                if(skill.isOn) {
                    (skill.GetExecutor() as Skill_00000439).OnEnemyDeath(this);
                }
            }
        }

        //message handler on critical hit
        public void OnCriticalHit() {
            log.InfoFormat("Message: OnCriticalHit yellow");
            if (HasSkill(SkillExecutor.SkilIDFromHexString(INCREASE_MAX_HP_FOR_CRIT_ID))) {
                var skill = GetSkillById(SkillExecutor.SkilIDFromHexString(INCREASE_MAX_HP_FOR_CRIT_ID));
                if (skill != null) {
                    if (skill.isOn) {
                        float hpPc = skill.GetDataInput<float>("hp_pc", 0f);
                        float hpTime = skill.GetDataInput<float>("hp_time", 0f);
                        int stackCount = skill.GetDataInput<int>("stack_count", 0);

                        int buffCount = bonuses.GetBuffCountWithTag(BonusType.increase_max_hp_on_pc, INCREASE_MAX_HP_FOR_CRIT_BUFF_TAG);
                        if (buffCount < stackCount) {
                            log.InfoFormat("set buff to max hp green");
                            Buff buff = new Buff(Guid.NewGuid().ToString(), null, BonusType.increase_max_hp_on_pc, hpTime, hpPc);
                            buff.SetTag(INCREASE_MAX_HP_FOR_CRIT_BUFF_TAG);
                            bonuses.SetBuff(buff);
                        }
                    }
                }
            }

            if(HasSkill(m456ID)) {
                var skill = GetSkillById(m456ID);
                if(skill.isOn) {
                    float dmgPc = skill.GetFloatInput("dmg_pc");
                    float enCostPc = skill.GetFloatInput("en_pc");
                    float time = skill.GetFloatInput("time");
                    Buff dmgBuff = new Buff(skill.id, null, BonusType.increase_damage_on_pc, time, dmgPc);
                    Buff energyCostBuff = new Buff(skill.id, null, BonusType.decrease_energy_cost_on_pc, time, enCostPc);
                    bonuses.SetBuff(dmgBuff);
                    bonuses.SetBuff(energyCostBuff);
                }
            }
        }

        public void OnMakeHeal(float healValue) {
            //when we healing other object, we restore part of health self
            if(m415.active) {
                float restoredValue = healValue * m415.value;
                var damagable = nebulaObject.Damagable();
                if(damagable) {
                    damagable.RestoreHealth(nebulaObject, restoredValue);
                }
            }
        }



        public void OnCriticalHeal(float critHeal) {
            if(m41C.active) {
                var skill = GetSkillById(SkillExecutor.SkilIDFromHexString(SKILL_0000041C));
                if(skill != null ) {
                    (skill.GetExecutor() as Skill_0000041C).Make(nebulaObject, skill, m41C.value);
                }
            }
        }

        /// <summary>
        /// Called by ally player target when he is receive damage, and move damage to me if 404 skill active
        /// </summary>
        /// <param name="inputDamage"></param>
        /// <param name="outputDamage"></param>
        /// <returns></returns>
        public bool MoveDamageFromAlly(float inputDamage, ref float outputDamage) {
            outputDamage = inputDamage;
            if(m404.active) {
                float eatedDamage = inputDamage * m404.value;
                outputDamage = Mathf.ClampLess(inputDamage - eatedDamage, 0);

                nebulaObject.Damagable().SubHealth(eatedDamage);

                log.InfoFormat("eat ally damage = {0}", eatedDamage);

                return true;
            }
            return false;
        }

        public void ModifyDamage(DamagableObject target, InputDamage inputDamage) {

            if(nebulaObject.Type == (byte)ItemType.Avatar) {
                if(HasSkill(m43AID)) {
                    var skill = GetSkillById(m43AID);
                    if(skill.isOn) {
                        var m43a = skill.GetExecutor() as Skill_0000043A;
                        if(target.health < target.maximumHealth * m43a.hpPc ) {
                            if(false == Mathf.Approximately(m43a.dmgMult, 0f)) {
                                log.InfoFormat("modify damage on {0}% with skill 43A", m43a.dmgMult);
                                inputDamage.Mult(m43a.dmgMult);
                                //return inputDamage;
                            }
                        }
                    }
                }
            }

            //return inputDamage;
        }


        public float ModifyCritChance(DamagableObject target, float inputCritChance) {
            if(HasSkill(m45CID)) {
                var skill = GetSkillById(m45CID);
                if(skill.isOn) {
                    float healpPc = skill.GetFloatInput("hp_pc");
                    if(target.health < healpPc * target.maximumHealth) {
                        float critChanceMult = skill.GetFloatInput("crit_chance_mult");
                        return inputCritChance * critChanceMult;
                    }
                }
            }
            return inputCritChance;
        }



        public void OnMakeFire(WeaponHitInfo hit) {
            if(m432.active) {
                if(mDamagable) {
                    float hpRestore = mDamagable.maximumHealth * m432.value;
                    mDamagable.RestoreHealth(nebulaObject, hpRestore);
                }
            }
        }

        //called by skill to set 3F7 skill parameters
        public void Set3F7(float duration, float hpPC) {
            m3F7.Activate(duration, hpPC);
        }

        public void Set3F8(float duration, float hpPC) {
            m3F8.Activate(duration, hpPC);
        }

        public void Set404(float duration, float damagePc) {
            m404.Activate(duration, damagePc);
        }

        public void Set415(float duration, float healPc ) {
            m415.Activate(duration, healPc);
        }

        //activate heal other when make crit heal skill
        public void Set41C(float duration, float healPc) {
            m41C.Activate(duration, healPc);
        }

        public void Set432(float duration, float healPc) {
            m432.Activate(duration, healPc);
        }

        //public void Set3FD(float duration, float increasePc) {
        //    m3FD.Activate(duration, increasePc);
        //}

        //public float GetIncreaseHealingPc() {
        //    if(m3FD.active) {
        //        return m3FD.value;
        //    }
        //    return 0f;
        //}


        public class ActiveSkillUpdater {
            public bool active { get; private set; }
            public float timer { get; private set; }
            public float value { get; private set; }

            public void Activate(float dur, float val) {
                timer = dur;
                value = val;
                active = true;
            }

            public void Deactivate() {
                active = false;
                timer = 0f;
            }

            public void Update(float deltaTime) {
                if (active) {
                    timer -= deltaTime;
                    if (timer <= 0f) {
                        active = false;
                        value = 0f;
                    }
                }
            }
        }
    }


}

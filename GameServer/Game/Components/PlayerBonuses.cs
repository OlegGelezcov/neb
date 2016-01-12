using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Server.Components;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Nebula.Game.Components {

    public class PlayerBonuses : NebulaBehaviour, IDatabaseObject
    {
        private ConcurrentDictionary<BonusType, PlayerBonus> mBonuses = new ConcurrentDictionary<BonusType, PlayerBonus>();
        private List<BonusType> emptyBonuses = new List<BonusType>();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private List<BonusType> mSpeedDebuffs;
        private BonusesComponentData mInitData;

        private void InitVariables() {
            if(mSpeedDebuffs == null ) {
                mSpeedDebuffs = BuffUtils.GetDebuffsForParameter(BuffParameter.speed).ToList();
            }
        }
        public void Init(BonusesComponentData data) {
            mInitData = data;
            InitVariables();
        }

        public override void Start() {
            InitVariables();
        }

        public void Respawn() {
            mBonuses.Clear();
        }

        public void RemoveAnyBuff() {
            BonusType removedKey = BonusType.block_heal;
            bool found = false;
            foreach(var pBonus in mBonuses) {
                if(IsBuff(pBonus.Key)) {
                    if(pBonus.Value.hasAny  ) {
                        removedKey = pBonus.Key;
                        found = true;
                    }
                }
            }
            if(found) {
                PlayerBonus removedBonus = null;
                if(mBonuses.TryRemove(removedKey, out removedBonus)) {
                    log.InfoFormat("PlayerBonuses.RemoveAnyBuff(): removed buff = {0} [yellow]", removedKey);
                }
            }
        }

        private bool IsBuff(BonusType type) {
            return !BuffUtils.IsDebuff(type);
        }

        public bool ContainsAny(BonusType[] types) {
            foreach(var type in types) {
                if(mBonuses.ContainsKey(type)) {
                    if(!Mathf.Approximately(Value(type), 0f)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public  ConcurrentDictionary<BonusType, PlayerBonus> bonuses {
            get {
                return mBonuses;
            }
            set {
                mBonuses = value;
            }
        }

        public bool Contains(string buffId) {
            foreach (var bonus in this.bonuses) {
                if (bonus.Value.Contains(buffId)) {
                    return true;
                }
            }
            return false;
        }

        public BuffSearchResult Contains(BonusType type, string buffId, int tag ) {
            BuffSearchResult result = BuffSearchResult.NotContains;
            if(bonuses.ContainsKey(type)) {
                result = bonuses[type].Contains(buffId, tag);
            }
            return result;
        }

        
        public float Value(BonusType type) {
            if(bonuses.ContainsKey(type)) {
                return bonuses[type].value;
            }
            return 0f;
        }

        public bool Contains(BonusType bonusType, string buffId) {
            if(!bonuses.ContainsKey(bonusType)) {
                return false;
            } else {
                if(bonuses[bonusType].Contains(buffId)) {
                    return true;
                } else {
                    return false;
                }
            }
        }

      
        public PlayerBonus GetBonus(BonusType bonus)
        {
            PlayerBonus result = null;
            bonuses.TryGetValue(bonus, out result);
            return result;
        }

        public int GetBuffCountWithTag(BonusType bonus, int buffTag) {
            var b = GetBonus(bonus);
            if(b != null ) {
                return b.GetBuffCountWithTag(buffTag);
            }
            return 0;
        }

        public void MultInterval(BonusType type, float mult) {
            var bonus = GetBonus(type);
            if(bonus != null ) {
                bonus.MultInterval(mult);
            }
        }



        public void SetBuff(BonusType type, Buff buff)
        {
            if(mSpeedDebuffs.Contains(type)) {
                if (BuffNotZero(BonusType.speed_debuff_immunity)) {
                    log.InfoFormat("exist speed debuff immunity, ignore speed debuff, yellow");
                    return;
                }
                if (BuffNotZero(BonusType.decrease_time_of_negative_speed_buffs)) {
                    buff.MultInterval(Value(BonusType.decrease_time_of_negative_speed_buffs));
                }
            }


            bool success = true;
            if(bonuses.ContainsKey(type) == false) {
                success = bonuses.TryAdd(type, new PlayerBonus(type));
            }
            if(success) {
                PlayerBonus bon = null;
                if(bonuses.TryGetValue(type, out bon)) {
                    bon.SetBuff(buff);
                }
            }
        }

        public void SetBuff(Buff buff) {
            SetBuff(buff.buffType, buff);
        }

        public Buff GetBuff(BonusType bonusType, string buffId )
        {
            PlayerBonus bon = null;
            if(bonuses.TryGetValue(bonusType, out bon)) {
                return bon.GetBuff(buffId);
            }
            return null;
        }

        public void RemoveBuff(BonusType type, string buffId)
        {
            PlayerBonus bon = null;
            if(bonuses.TryGetValue(type, out bon)) {
                bon.RemoveBuff(buffId);
            }
        }

        /// <summary>
        /// Remove any positive Bonus Type from object
        /// </summary>
        public void RemoveAnyPositiveBuff() {
            bool buffFounded = false;
            BonusType foundedBuffType = BonusType.block_heal;

            foreach(var pBonus in bonuses ) {
                if(BuffUtils.IsBuff(pBonus.Key)) {
                    foundedBuffType = pBonus.Key;
                    buffFounded = true;
                    break;
                }
            }

            if(buffFounded) {
                PlayerBonus removedBonus = null;
                bonuses.TryRemove(foundedBuffType, out removedBonus);
            }
        }



        public bool RemoveBuffs(BonusType type) {
            PlayerBonus old = null;
            return bonuses.TryRemove(type, out old);
        }

        public bool BuffNotZero(BonusType bonusType)
        {
            return (!Mathf.IsZero(Value(bonusType)));
        }

        public override void Update(float deltaTime)
        {
            if (nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }

            foreach(var bonusPair in bonuses) {
                bonusPair.Value.Update(deltaTime);
            }
            props.SetProperty((byte)PS.Bonuses, GetInfo());
        }

        public Hashtable GetInfo()
        {
            emptyBonuses.Clear();
            Hashtable result = new Hashtable();
            foreach (var b in this.bonuses)
            {
                if(b.Value.count > 0 ) {
                    result.Add((int)b.Key, b.Value.value);
                } else {
                    emptyBonuses.Add(b.Key);
                }

            }

            PlayerBonus removedBonus = null;
            foreach(var emptyBonusType in emptyBonuses) {
                bonuses.TryRemove(emptyBonusType, out removedBonus);
            }
            return result;
        }

        public void ScaleDebuffInterval(float mult) {
            foreach(var bonusPair in bonuses) {
                if(BuffUtils.IsDebuff(bonusPair.Key)) {
                    bonusPair.Value.ScaleBuffInterval(mult);
                }
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Bonuses;
            }
        }

        public float maxEnergyCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_max_energy_on_cnt);
                result -= Value(BonusType.decrease_max_energy_on_cnt);
                return result;
            }
        }

        public float speedPcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_speed_on_pc);
                result -= Value(BonusType.decrease_speed_on_pc);
                return result;
            }
        }

        public float speedCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_speed_on_cnt);
                result -= Value(BonusType.decrease_speed_on_cnt);
                return result;
            }
        }

        public float damagePcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_damage_on_pc);
                result -= Value(BonusType.decrease_damage_on_pc);
                return result;
            }
        }

        public float damageCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_damage_on_cnt);
                result -= Value(BonusType.decrease_damage_on_cnt);
                return result;
            }
        }

        public float resistPcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_resist_on_pc);
                result -= Value(BonusType.decrease_resist_on_pc);
                return result;
            }
        }

        public float resistCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_resist_on_cnt);
                result -= Value(BonusType.decrease_resist_on_cnt);
                return result;
            }
        }

        public float optimalDistanceCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_optimal_distance_on_cnt);
                result -= Value(BonusType.decrease_optimal_distance_on_cnt);
                return result;
            }
        }

        public float optimalDistancePcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_optimal_distance_on_pc);
                result -= Value(BonusType.decrease_optimal_distance_on_pc);
                return result;
            }
        }

        public float cooldownCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_cooldown_on_cnt);
                result -= Value(BonusType.decrease_cooldown_on_cnt);
                return result;
            }
        }

        public float cooldownPcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_cooldown_on_pc);
                result -= Value(BonusType.decrease_cooldown_on_pc);
                return result;
            }
        }

        public float critDamageCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_crit_damage_on_cnt);
                result -= Value(BonusType.decrease_crit_damage_on_cnt);
                return result;
            }
        }

        public float critDamagePcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_crit_damage_on_pc);
                result -= Value(BonusType.decrease_crit_damage_on_pc);
                return result;
            }
        }

        public float critChanceCntBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_crit_chance_on_cnt);
                result -= Value(BonusType.decrease_crit_chance_on_cnt);
                return result;
            }
        }

        public float critChancePcBonus {
            get {
                float result = 0f;
                result += Value(BonusType.increase_crit_chance_on_pc);
                result -= Value(BonusType.decrease_crit_chance_on_pc);
                return result;
            }
        }

        public float maxHpCntBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_max_hp_on_cnt);
                sum -= Value(BonusType.decrease_max_hp_on_cnt);
                return sum;
            }
        }

        public float maxHpPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_max_hp_on_pc);
                sum -= Value(BonusType.decrease_max_hp_on_pc);
                return sum;
            }
        }

        public float healingSpeedPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_healing_speed_on_pc);
                sum -= Value(BonusType.decrease_healing_speed_on_pc);
                return sum;
            }
        }

        public float healingPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_healing_on_pc);
                sum -= Value(BonusType.decrease_healing_on_pc);
                return sum;
            }
        }

        public float healingCntBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_healing_on_cnt);
                sum -= Value(BonusType.decrease_healing_on_cnt);
                return sum;
            }
        }

        public float expPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_exp_on_pc);
                return sum;
            }
        }

        public float inputDamagePcBonus {
            get {
                float sum = 0.0f;
                sum += Value(BonusType.increase_input_damage_on_pc);
                sum -= Value(BonusType.decrease_input_damage_on_pc);
                sum = Mathf.ClampLess(sum, 0.0f);
                return sum;
            }
        }

        public float reflectionPc {
            get {
                float sum = 0f;
                sum += Value(BonusType.reflection_pc);
                return sum;
            }
        }

        public float restoreHpAtSecPcBonus {
            get {
                float sum = 0.0f;
                sum += Value(BonusType.restore_hp_at_sec_on_pc);
                return sum;
            }
        }

        public float restoreHpAtSecCntBonus {
            get {
                float sum = 0.0f;
                sum += Value(BonusType.restore_hp_at_sec_on_cnt);
                return sum;
            }
        }

        public float absrodDamagePcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.absorb_damage_pc);
                return Mathf.Clamp01(sum);
            }
        }

        public float convertAbsorbedDamageToHpPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.convert_absorbed_damage_to_hp_pc);
                return sum;
            }
        }

        public float vampirismPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.vampirism_pc);
                return sum;
            }
        }

        public float pvpPointsPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_pvp_points);
                return sum;
            }
        }

        public float creditsPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_credits);
                return sum;
            }
        }

        public float energyRegenPcBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_energy_regen_on_pc);
                sum -= Value(BonusType.decrease_energy_regen_on_pc);
                return sum;
            }
        }

        public float energyRegenCntBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.increase_energy_regen_on_cnt);
                sum -= Value(BonusType.decrease_energy_regen_on_cnt);
                return sum;
            }
        }

        public float autoLootBonus {
            get {
                float sum = 0f;
                sum += Value(BonusType.auto_loot_chest);
                return sum;
            }
        }

        public bool isImmuneToDamage {
            get {
                return (false == Mathf.Approximately(Value(BonusType.damage_immunity), 0f));
            }
        }

        public float energyCostPcBonus {
            get {
                return Value(BonusType.increase_energy_cost_on_pc) - Value(BonusType.decrease_energy_cost_on_pc);
            }
        }

        public float energyCostCntBonus {
            get {
                return Value(BonusType.increase_energy_cost_on_cnt) - Value(BonusType.decrease_energy_cost_on_cnt);
            }
        }

        public float dronStrengthPcBonus {
            get {
                return Value(BonusType.increase_dron_strength_on_pc) - Value(BonusType.decrease_dron_strength_on_pc);
            }
        }

        public float dronStrengthCntBonus {
            get {
                return Value(BonusType.increase_dron_strength_on_cnt) - Value(BonusType.decrease_dron_strength_on_cnt);
            }
        }

        public class BuffInfo {
            public BonusType bonusType;
            public float time;
            public float value;
        }

        public ConcurrentBag<BuffInfo> GetAllDebuffInfo() {
            ConcurrentBag<BuffInfo> result = new ConcurrentBag<BuffInfo>();
            foreach(var pBonus in bonuses) {
                if(BuffUtils.IsDebuff(pBonus.Key)) {
                    foreach(var pBuff in pBonus.Value.GetBuffInfoCollection()) {
                        result.Add(pBuff);
                    }
                }
            }
            return result;
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}

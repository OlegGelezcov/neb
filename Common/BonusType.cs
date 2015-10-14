// BonusType.cs
// Nebula
//
// Created by Oleg Zheleztsov on Saturday, September 26, 2015 3:27:20 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Common {
    /// <summary>
    /// All buff types in game
    /// </summary>
    public enum BonusType : byte
    {
        /// <summary>
        /// Decreaste speed om value %
        /// </summary>
        decrease_speed_on_pc,           //decrease speed from base value in percent [0, 1]
        /// <summary>
        /// increase speed on value %
        /// </summary>
        increase_speed_on_pc,           //increase speed on in percent [0, 1]
        /// <summary>
        /// Decrease speed on value UNITS
        /// </summary>
        decrease_speed_on_cnt,          //decrese speed on count 
        /// <summary>
        /// Increase speed on value UNITS
        /// </summary>
        increase_speed_on_cnt,           //increase speed on count
        
        /// <summary>
        ///float percent [0-1] based which increase damage on value percent from some base value 
        /// </summary>
        increase_damage_on_pc,
        /// <summary>
        /// float percent [0-1] based which decrease damage on value percent from some base value
        /// </summary>
        decrease_damage_on_pc,
        /// <summary>
        /// Increase damage on COUNT value
        /// </summary>
        increase_damage_on_cnt,
        /// <summary>
        /// Decrease damage on COUNT value
        /// </summary>
        decrease_damage_on_cnt,
        /// <summary>
        /// Full damage immunity
        /// </summary>
        damage_immunity,

        /// <summary>
        /// Decrease max energy on COUNT
        /// </summary>
        increase_max_energy_on_cnt,
        /// <summary>
        /// Decrease maxmimum energy on COUNT
        /// </summary>
        decrease_max_energy_on_cnt,

        //restore_hp_on_pc_for_sec,

        /// <summary>
        /// Increase damage resistance on value %
        /// </summary>
        increase_resist_on_pc,
        /// <summary>
        /// Decrease damage resistane on value %
        /// </summary>
        decrease_resist_on_pc,
        /// <summary>
        /// Increase damage resistance on value UNITS
        /// </summary>
        increase_resist_on_cnt,
        /// <summary>
        /// Decrease damage resistance on value UNITS
        /// </summary>
        decrease_resist_on_cnt,
        /// <summary>
        /// Increase cooldown on value UNITS
        /// </summary>
        increase_cooldown_on_cnt,
        /// <summary>
        /// Decrease cooldown on value UNITS
        /// </summary>
        decrease_cooldown_on_cnt,
        /// <summary>
        /// Increase cooldown on value %
        /// </summary>
        increase_cooldown_on_pc,
        /// <summary>
        /// Decrease cooldown on value %
        /// </summary>
        decrease_cooldown_on_pc,
        /// <summary>
        /// Increase optimal distance on value UNITS
        /// </summary>
        increase_optimal_distance_on_cnt,
        /// <summary>
        /// Decrease optimal distance on value UNITS
        /// </summary>
        decrease_optimal_distance_on_cnt,
        /// <summary>
        /// Increase optimal distance on value %
        /// </summary>
        increase_optimal_distance_on_pc,
        /// <summary>
        /// Decrease optimal distance on value %
        /// </summary>
        decrease_optimal_distance_on_pc,
        /// <summary>
        /// Increase critical damage on value %
        /// </summary>
        increase_crit_damage_on_pc,
        /// <summary>
        /// Decrease critical damage on value %
        /// </summary>
        decrease_crit_damage_on_pc,
        /// <summary>
        /// Increase critical damage on value UNITS
        /// </summary>
        increase_crit_damage_on_cnt,
        /// <summary>
        /// Decrease critical damage on value UNITS
        /// </summary>
        decrease_crit_damage_on_cnt,
        /// <summary>
        /// Increase critical chance on value %
        /// </summary>
        increase_crit_chance_on_pc,
        /// <summary>
        /// Decrease critical chance on value %
        /// </summary>
        decrease_crit_chance_on_pc,
        /// <summary>
        /// Increase critical chance on value UNITS
        /// </summary>
        increase_crit_chance_on_cnt,
        /// <summary>
        /// Decrease critical chance on value UNITS
        /// </summary>
        decrease_crit_chance_on_cnt,

        /// <summary>
        /// Decrease time of all speed debuffs on %
        /// </summary>
        decrease_time_of_negative_speed_buffs,

        /// <summary>
        /// Weapon is blocked
        /// </summary>
        block_weapon,

        /// <summary>
        /// Healing is blocked
        /// </summary>
        block_heal,

        /// <summary>
        /// Increase maximum HP on %
        /// </summary>
        increase_max_hp_on_pc,
        /// <summary>
        /// Decrease maximum HP on %
        /// </summary>
        decrease_max_hp_on_pc,
        /// <summary>
        /// Increase maximum HP on value UNITS
        /// </summary>
        increase_max_hp_on_cnt,
        /// <summary>
        /// Decrease maximum HP on value UNITS
        /// </summary>
        decrease_max_hp_on_cnt,
        /// <summary>
        /// Increase input healing on value %
        /// </summary>
        increase_healing_speed_on_pc,
        /// <summary>
        /// Decrease input healing on value %
        /// </summary>
        decrease_healing_speed_on_pc,
        /// <summary>
        /// Immunity to all speed debuffs casted on me
        /// </summary>
        speed_debuff_immunity,
        /// <summary>
        /// Increase healing to target on value %
        /// </summary>
        increase_healing_on_pc,
        /// <summary>
        /// Decrease healing to target on value %
        /// </summary>
        decrease_healing_on_pc,
        /// <summary>
        /// Increase healing to target on value UNITS
        /// </summary>
        increase_healing_on_cnt,
        /// <summary>
        /// Decrease healing to target on value UNITS
        /// </summary>
        decrease_healing_on_cnt,
        /// <summary>
        /// Block resists ( decorated skill - no any effect, need call BlockResist() on ship)
        /// </summary>
        block_resist,

        /// <summary>
        /// Increase cost of energy for using skills on value %
        /// </summary>
        increase_energy_cost_on_pc,
        /// <summary>
        /// Decrease const of energy for using skills on value %
        /// </summary>
        decrease_energy_cost_on_pc,
        /// <summary>
        /// Increase cost of energy for using skills on value UNITS
        /// </summary>
        increase_energy_cost_on_cnt,
        /// <summary>
        /// Decrease cost of energy for using skills on value UNITS
        /// </summary>
        decrease_energy_cost_on_cnt,

        increase_dron_strength_on_pc,
        decrease_dron_strength_on_pc,
        increase_dron_strength_on_cnt,
        decrease_dron_strength_on_cnt
    }

    /// <summary>
    /// Buff parameter group
    /// </summary>
    public enum BuffParameter {
        speed,
        damage,
        max_energy,
        resist,
        cooldown,
        optimal_distance,
        crit_damage,
        crit_chance,
        max_hp,
        hp,
        healing,
        energy_cost,
        dron_strength
    }

    public static class BuffUtils {

        /// <summary>
        /// Check buff type is debuff - negative effect
        /// </summary>
        /// <param name="bonusType">Buff type to check</param>
        /// <returns>True - is buff us debuff, else false</returns>
        public static bool IsDebuff(BonusType bonusType) {
            switch (bonusType) {
                case BonusType.decrease_speed_on_cnt:
                case BonusType.decrease_speed_on_pc:
                case BonusType.decrease_damage_on_cnt:
                case BonusType.decrease_damage_on_pc:
                case BonusType.decrease_max_energy_on_cnt:
                case BonusType.decrease_resist_on_cnt:
                case BonusType.decrease_resist_on_pc:
                case BonusType.increase_cooldown_on_cnt:
                case BonusType.increase_cooldown_on_pc:
                case BonusType.decrease_optimal_distance_on_cnt:
                case BonusType.decrease_optimal_distance_on_pc:
                case BonusType.decrease_crit_damage_on_cnt:
                case BonusType.decrease_crit_damage_on_pc:
                case BonusType.decrease_crit_chance_on_cnt:
                case BonusType.decrease_crit_chance_on_pc:
                case BonusType.block_heal:
                case BonusType.block_weapon:
                case BonusType.decrease_max_hp_on_cnt:
                case BonusType.decrease_max_hp_on_pc:
                case BonusType.decrease_healing_speed_on_pc:
                case BonusType.decrease_healing_on_cnt:
                case BonusType.decrease_healing_on_pc:
                case BonusType.block_resist:
                case BonusType.increase_energy_cost_on_pc:
                case BonusType.increase_energy_cost_on_cnt:
                case BonusType.decrease_dron_strength_on_cnt:
                case BonusType.decrease_dron_strength_on_pc:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBuff(BonusType bonusType) {
            return (false == IsDebuff(bonusType));
        }

        /// <summary>
        /// Get all debuffs for buff group
        /// </summary>
        /// <param name="prm">Buff type group </param>
        /// <returns>Array of all debuffs for buff group parameter</returns>
        public static BonusType[] GetDebuffsForParameter(BuffParameter prm) {
            switch (prm) {
                case BuffParameter.cooldown:
                    return new BonusType[] { BonusType.increase_cooldown_on_cnt, BonusType.increase_cooldown_on_pc };
                case BuffParameter.damage:
                    return new BonusType[] { BonusType.decrease_damage_on_cnt, BonusType.decrease_damage_on_pc };
                case BuffParameter.max_energy:
                    return new BonusType[] { BonusType.decrease_max_energy_on_cnt };
                case BuffParameter.optimal_distance:
                    return new BonusType[] { BonusType.decrease_optimal_distance_on_cnt, BonusType.decrease_optimal_distance_on_pc };
                case BuffParameter.resist:
                    return new BonusType[] { BonusType.decrease_resist_on_cnt, BonusType.decrease_resist_on_pc,  BonusType.block_resist };
                case BuffParameter.speed:
                    return new BonusType[] { BonusType.decrease_speed_on_cnt, BonusType.decrease_speed_on_pc };
                case BuffParameter.crit_damage:
                    return new BonusType[] { BonusType.decrease_crit_damage_on_pc, BonusType.decrease_crit_damage_on_cnt };
                case BuffParameter.crit_chance:
                    return new BonusType[] { BonusType.decrease_crit_chance_on_cnt, BonusType.decrease_crit_chance_on_pc };
                case BuffParameter.max_hp:
                    return new BonusType[] { BonusType.decrease_max_hp_on_cnt, BonusType.decrease_max_hp_on_pc };
                case BuffParameter.hp:
                    return new BonusType[] { BonusType.decrease_healing_speed_on_pc };
                case BuffParameter.healing:
                    return new BonusType[] { BonusType.decrease_healing_on_cnt, BonusType.decrease_healing_on_pc };
                case BuffParameter.energy_cost:
                    return new BonusType[] { BonusType.increase_energy_cost_on_pc, BonusType.increase_energy_cost_on_cnt };
                case BuffParameter.dron_strength:
                    return new BonusType[] { BonusType.decrease_dron_strength_on_pc, BonusType.decrease_dron_strength_on_cnt };
                default:
                    return new BonusType[] { };

            }
        }
        /// <summary>
        /// Return array of buffs for buff type parameter group
        /// </summary>
        /// <param name="prm">Parameter group</param>
        /// <returns>Array of buffs for parameter group</returns>
        public static BonusType[] GetBuffsForParameter(BuffParameter prm) {
            switch(prm) {
                case BuffParameter.cooldown:
                    return new BonusType[] { BonusType.decrease_cooldown_on_cnt, BonusType.decrease_cooldown_on_pc };
                case BuffParameter.damage:
                    return new BonusType[] { BonusType.increase_damage_on_cnt, BonusType.increase_damage_on_pc };
                case BuffParameter.max_energy:
                    return new BonusType[] { BonusType.increase_max_energy_on_cnt };
                case BuffParameter.optimal_distance:
                    return new BonusType[] { BonusType.increase_optimal_distance_on_cnt, BonusType.increase_optimal_distance_on_pc };
                case BuffParameter.resist:
                    return new BonusType[] { BonusType.increase_resist_on_cnt, BonusType.increase_resist_on_pc };
                case BuffParameter.speed:
                    return new BonusType[] { BonusType.increase_speed_on_cnt, BonusType.increase_speed_on_pc };
                case BuffParameter.crit_damage:
                    return new BonusType[] { BonusType.increase_crit_damage_on_cnt, BonusType.increase_crit_damage_on_pc };
                case BuffParameter.crit_chance:
                    return new BonusType[] { BonusType.increase_crit_chance_on_cnt, BonusType.increase_crit_chance_on_pc };
                case BuffParameter.max_hp:
                    return new BonusType[] { BonusType.increase_max_hp_on_cnt, BonusType.increase_max_hp_on_pc };
                case BuffParameter.hp:
                    return new BonusType[] { BonusType.increase_healing_speed_on_pc };
                case BuffParameter.healing:
                    return new BonusType[] { BonusType.increase_healing_on_cnt, BonusType.increase_healing_on_pc };
                case BuffParameter.energy_cost:
                    return new BonusType[] { BonusType.decrease_energy_cost_on_pc, BonusType.decrease_energy_cost_on_cnt };
                case BuffParameter.dron_strength:
                    return new BonusType[] { BonusType.increase_dron_strength_on_cnt, BonusType.increase_dron_strength_on_pc };
                default:
                    return new BonusType[] { };
            }
        }
    }
}



namespace Nebula.Resources {
    using UnityEngine;
    using System.Collections;
    using Nebula.Client.Res;
    using Common;

    public class BuffDescriptionResolver {

        private StringSubCache<BonusType> subCache = new StringSubCache<BonusType>();

        public string Resolve(ResBuffData buff, float value) {
            switch (buff.bonusType) {
                case BonusType.damage_immunity:
                    return subCache.String(buff.bonusType, buff.description);
                case BonusType.decrease_cooldown_on_cnt:
                case BonusType.decrease_crit_chance_on_cnt:
                case BonusType.decrease_crit_damage_on_cnt:
                case BonusType.decrease_damage_on_cnt:
                case BonusType.decrease_max_energy_on_cnt:
                case BonusType.decrease_optimal_distance_on_cnt:
                case BonusType.decrease_resist_on_cnt:
                case BonusType.decrease_speed_on_cnt:
                case BonusType.increase_cooldown_on_cnt:
                case BonusType.increase_crit_chance_on_cnt:
                case BonusType.increase_crit_damage_on_cnt:
                case BonusType.increase_damage_on_cnt:
                case BonusType.increase_max_energy_on_cnt:
                case BonusType.increase_optimal_distance_on_cnt:
                case BonusType.increase_resist_on_cnt:
                case BonusType.increase_speed_on_cnt:
                case BonusType.increase_max_hp_on_cnt:
                case BonusType.decrease_max_hp_on_cnt:
                    return string.Format(subCache.String(buff.bonusType, buff.description), Mathf.RoundToInt(value));
                case BonusType.decrease_cooldown_on_pc:
                case BonusType.decrease_crit_chance_on_pc:
                case BonusType.decrease_crit_damage_on_pc:
                case BonusType.decrease_damage_on_pc:
                case BonusType.decrease_optimal_distance_on_pc:
                case BonusType.decrease_resist_on_pc:
                case BonusType.decrease_speed_on_pc:
                case BonusType.increase_cooldown_on_pc:
                case BonusType.increase_crit_chance_on_pc:
                case BonusType.increase_crit_damage_on_pc:
                case BonusType.increase_damage_on_pc:
                case BonusType.increase_optimal_distance_on_pc:
                case BonusType.increase_resist_on_pc:
                case BonusType.increase_speed_on_pc:
                case BonusType.increase_max_hp_on_pc:
                case BonusType.decrease_max_hp_on_pc:
                    return string.Format(subCache.String(buff.bonusType, buff.description), Mathf.RoundToInt(value * 100));
                case BonusType.decrease_time_of_negative_speed_buffs:
                    return subCache.String(buff.bonusType, buff.description);
                default:
                    return subCache.String(buff.bonusType, buff.description);

            }
        }
    }

}
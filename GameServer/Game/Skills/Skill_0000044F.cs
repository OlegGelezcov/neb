using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000044F : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if(!source) {
                return false;
            }

            int dronCnt = skill.data.Inputs.Value<int>("dron_cnt");
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            float hpTime = skill.data.Inputs.Value<float>("hp_time");
            float radius = skill.data.Inputs.Value<float>("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                hpPc *= 2;
            }

            var sourceChar = source.GetComponent<CharacterObject>();
            var sourceDamagable = source.GetComponent<DamagableObject>();
            var sourceBonuses = source.Bonuses();


            var items = (source.world as MmoWorld).GetItems((it) => {
                if(it.GetComponent<DamagableObject>() && it.GetComponent<CharacterObject>() ) {
                    if(source.transform.DistanceTo(it.transform) < radius  && (source.Id != it.Id ) ) {
                        if(sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Friend ) {
                            return true;
                        }
                    }
                }
                return false;
            });

            int freeDrons = dronCnt - items.Count;

            int counter = 0;

            string id = source.Id + skill.data.Id.ToString();

            foreach (var p in items ) {
                var d = p.Value.GetComponent<DamagableObject>();
                float hpRestor = d.maximumHealth * hpPc ;
                hpRestor = hpRestor * (1f + sourceBonuses.dronStrengthPcBonus) + sourceBonuses.dronStrengthCntBonus;
                d.SetRestoreHPPerSec(hpRestor / hpTime, hpTime, id);
                info.Add(p.Key, p.Value.Type);
                counter++;
                if(counter >= dronCnt ) {
                    break;
                }
            }

            float sumRestore = 0;
            float partHP = sourceDamagable.maximumHealth * hpPc;
            for(int i = 0; i < freeDrons; i++) {
                sumRestore += partHP;
            }

            if( sumRestore > 0f ) {
                sourceDamagable.SetRestoreHPPerSec(sumRestore / hpTime, hpTime, id);
            }
            return true;
        }
    }
}

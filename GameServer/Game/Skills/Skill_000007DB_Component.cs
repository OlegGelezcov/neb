using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game.Resources;

namespace Nebula.Game.Skills {
    public class Skill_000007DB_Component : NebulaBehaviour {

        private float mTimer;
        private float mSpeedPc;
        private float cdPc;
        private float mRadius;
        private SkillData mData;

        private CharacterObject mCharacter;


        public override int behaviourId {
            get {
                return (int)ComponentID.Skill_7DB;
            }
        }

        /// <summary>
        /// Need call before start
        /// </summary>
        /// <param name="data">Data from resources for skill 7DB</param>
        public void SetSkill(SkillData data) {
            mData = data;
            mTimer = data.Inputs.GetValue<float>("time", 0f);
            mSpeedPc = data.Inputs.GetValue<float>("speed_pc", 0f);
            cdPc = data.Inputs.GetValue<float>("cd_pc", 0f);
            mRadius = data.Inputs.GetValue<float>("radius", 0f);
        }

        /// <summary>
        /// Cache character component
        /// </summary>
        public override void Start() {
            mCharacter = GetComponent<CharacterObject>();
        }

        public override void Update(float deltaTime) {
            //first check for expires
            mTimer -= deltaTime;
            if(mTimer <= 0f ) {
                nebulaObject.RemoveComponent<Skill_000007DB_Component>();
                return;
            }

            //find attackable items in world
            var items = nebulaObject.mmoWorld().GetItems((item) => {
                var distance = nebulaObject.transform.DistanceTo(item.transform);
                if (distance < mRadius) {
                    var damagable = item.Damagable();
                    var bonuses = item.Bonuses();
                    var character = item.Character();
                    if (damagable && bonuses && character) {

                        var relation = mCharacter.RelationTo(character);
                        if (relation == FractionRelation.Enemy || relation == FractionRelation.Neutral) {
                            return true;
                        }
                    }
                }
                return false;
            });

            //set debuffs on attackable items
            foreach(var pItem in items ) {
                var item = pItem.Value;
                var itemBonuses = item.Bonuses();
                Buff speeddebuff = new Buff(mData.Id.ToString(), null, BonusType.decrease_speed_on_pc, mTimer, mSpeedPc, () => {
                    return (item.transform.DistanceTo(nebulaObject.transform) <= mRadius);
                });
                Buff cooldownDebuff = new Buff(mData.Id.ToString(), null, BonusType.increase_cooldown_on_pc, mTimer, cdPc, () => {
                    return (item.transform.DistanceTo(nebulaObject.transform) <= mRadius);
                });
                itemBonuses.SetBuff(speeddebuff, speeddebuff.owner);
                itemBonuses.SetBuff(cooldownDebuff, cooldownDebuff.owner);
            }
        }
    }
}

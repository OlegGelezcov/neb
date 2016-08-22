using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game.Resources;

namespace Nebula.Game.Skills {
    public class Skill_0000045E_Component : NebulaBehaviour {

        private float mTimer;
        private float mSpeedPc;
        private float mRadius;
        private Vector3 mCenter;
        private SkillData mData;

        private CharacterObject mCharacter;

        public override int behaviourId {
            get {
                return (int)ComponentID.Skill_45E;
            }
        }

        public void SetSkill(Vector3 position, float radius, SkillData data) {
            mData = data;
            mTimer = data.Inputs.GetValue<float>("speed_time", 0f);
            mSpeedPc = data.Inputs.GetValue<float>("speed_pc", 0f);
            mRadius = radius;
            mCenter = position;
        }

        public override void Start() {
            mCharacter = GetComponent<CharacterObject>();
        }

        public override void Update(float deltaTime) {
            mTimer -= deltaTime;
            if(mTimer <= 0f) {
                nebulaObject.RemoveComponent<Skill_0000045E_Component>();
                return;
            }

            var items = nebulaObject.mmoWorld().GetItems((item) => {
                var distance = Vector3.Distance(mCenter, item.transform.position);
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

            foreach(var pItem in items) {
                var item = pItem.Value;
                var itemBonuses = item.Bonuses();
                Buff speedDebuff = new Buff(mData.Id.ToString(), null, BonusType.decrease_speed_on_pc, mTimer, mSpeedPc, () => {
                    return (Vector3.Distance(mCenter, item.transform.position) <= mRadius);
                });
                itemBonuses.SetBuff(speedDebuff, nebulaObject);
            }

        }
    }
}

using GameMath;
using Nebula.Game.Components;

namespace Nebula.Game.Pets {
    public class PetCombatState : PetBaseState {

        public readonly float kSpeed = 25.0f;
        private readonly float kConvergencePc = 0.7f;

        private PlayerTarget m_OwnerTarget;
        private DamagableObject m_OwnerDamagable;
        private PlayerTarget m_PetTarget;
        private PetWeapon m_PetWeapon;


        public PetCombatState(PetObject obj) : base(obj) {
            m_OwnerTarget = pet.owner.Target();
            m_OwnerDamagable = pet.owner.Damagable();
            m_PetTarget = pet.GetComponent<PlayerTarget>();
            m_PetWeapon = pet.GetComponent<PetWeapon>();
        }

        public override PetState name {
            get {
                return PetState.Combat;
            }
        }

        public override void Update(float deltaTime) {
            if(false == m_OwnerTarget.inCombat) {
                pet.SetState(new PetIdleState(pet));
            }
            if(pet.info.damageType == Common.WeaponDamageType.heal) {
                pet.SetState(new PetIdleState(pet));
            }

            if(!m_PetTarget.targetObject) {
                var enemy = m_OwnerTarget.anyEnemySubscriber;
                if(enemy) {
                    m_PetTarget.SetTarget(enemy);
                    MoveToTarget(deltaTime);
                    FireAtTarget();
                } else {
                    pet.SetState(new PetIdleState(pet));
                }
            } else {
                MoveToTarget(deltaTime);
                FireAtTarget();
            }
        }

        private void MoveToTarget(float deltaTime) {
            Vector3 direction = (m_PetTarget.targetObject.transform.position - pet.transform.position);

            if(direction.magnitude > m_PetWeapon.optimalDistance * kConvergencePc) {
                Vector3 moving = kSpeed * direction.normalized * deltaTime;
                pet.Move(pet.transform.position, pet.transform.rotation, pet.transform.position + moving, pet.ComputeRotation(direction.normalized, 0.5f, deltaTime).eulerAngles, kSpeed);
            }
        }

        private void FireAtTarget() {
            if(m_PetWeapon.ready) {
                m_PetWeapon.MakeShot();
            }
        }
    }
}

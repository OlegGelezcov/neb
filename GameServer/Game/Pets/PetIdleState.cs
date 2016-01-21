using GameMath;
using Nebula.Game.Components;

namespace Nebula.Game.Pets {
    public class PetIdleState : PetBaseState {
        private const float UPDATE_OFFSET_COOLDOWN = 30;
        private const float MIN_DISTANCE = 5f;
        private const float MAX_DISTANCE = 20f;
        private const float ROTATION_SPEED = 0.5f;
        public const float OFFSET_RADIUS = 17;

        private float m_OffsetChangeTimer = UPDATE_OFFSET_COOLDOWN;
        private Vector3 m_Offset;

        private PlayerTarget m_OwnerTarget;
        private DamagableObject m_OwnerDamagable;
        private PetWeapon m_PetWeapon;
        private PlayerTarget m_PetTarget;


        public PetIdleState(PetObject obj) : base(obj) {
            m_OwnerTarget = pet.owner.GetComponent<PlayerTarget>();
            m_OwnerDamagable = pet.owner.GetComponent<DamagableObject>();
            m_PetWeapon = pet.GetComponent<PetWeapon>();
            m_PetTarget = pet.GetComponent<PlayerTarget>();
        }

        public override PetState name {
            get {
                return PetState.Idle;
            }
        }

        private void UpdateOffset() {
            m_Offset = Rand.UnitVector3() * OFFSET_RADIUS;
        }

        private Vector3 targetPoint {
            get {
                return pet.owner.transform.position + m_Offset;
            }
        }

        public override void Update(float deltaTime) {
            IdleMove(deltaTime);
            CheckNewState();
        }

        private void CheckNewState() {
            if (pet.info.damageType == Common.WeaponDamageType.damage) {
                if (m_OwnerTarget.inCombat) {
                    pet.SetState(new PetCombatState(pet));
                }
            } else if (pet.info.damageType == Common.WeaponDamageType.heal) {
                if (m_OwnerDamagable.health < m_OwnerDamagable.maximumHealth) {
                    if (m_PetTarget.noTarget || m_PetTarget.IsNotTarget(pet.owner.Id)) {
                        m_PetTarget.SetTarget(pet.owner);
                    }
                    m_PetWeapon.MakeShot();
                }
            }
        }

        private void IdleMove(float deltaTime) {
            m_OffsetChangeTimer -= deltaTime;
            if (m_OffsetChangeTimer <= 0f) {
                m_OffsetChangeTimer = UPDATE_OFFSET_COOLDOWN;
                UpdateOffset();
            }

            var direction = targetPoint - pet.transform.position;
            float distance = direction.magnitude;
            if (distance <= MIN_DISTANCE) {
                //UpdateOffset();
                //var nDir = direction.normalized;
                //var oPos = transform.position;
                //var oRot = transform.rotation;
                //var nPos = transform.position + nDir * m_Movable.speed * deltaTime;
                //var nRot = ComputeRotation(nDir, ROTATION_SPEED, deltaTime);
                //Move(oPos, oRot, nPos, nRot.eulerAngles, m_Movable.speed);
            } else {

                var nDir = direction.normalized;
                var oPos = pet.transform.position;
                var oRot = pet.transform.rotation;

                float modSpeed = distance / 1; //m_Movable.speed * (distance - MIN_DISTANCE) / (MAX_DISTANCE - MIN_DISTANCE);

                float moving = modSpeed * deltaTime;

                if (moving > distance) {
                    moving = distance * 0.9f;
                }

                if (distance > MAX_DISTANCE) {
                    moving += (distance - MAX_DISTANCE);
                    if (moving > distance) {
                        moving = distance * 0.9f;
                    }
                }

                if (Mathf.Approximately(moving, 0f)) {
                    moving = 0f;
                }

                float spFinal = 0;
                if (deltaTime > 0) {
                    spFinal = moving / deltaTime;
                }

                var nPos = pet.transform.position + nDir * moving;


                var nRot = pet.ComputeRotation(nDir, ROTATION_SPEED, deltaTime).eulerAngles;
                //if(owner) {
                //    nRot = owner.transform.rotation;
                //}
                pet.Move(oPos, oRot, nPos, nRot, spFinal);
            }
        }
    }
}

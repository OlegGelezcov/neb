//#define USE_SKILLS
using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Inventory.DropList;
using Nebula.Server;
using Nebula.Server.Components;
using Space.Game;
using Space.Game.Objects;
using Space.Server;
using System;
using System.Collections;

namespace Nebula.Game.Components.BotAI {

    [REQUIRE_COMPONENT(typeof(CharacterObject))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    [REQUIRE_COMPONENT(typeof(PlayerTarget))]
    [REQUIRE_COMPONENT(typeof(MovableObject))]
    [REQUIRE_COMPONENT(typeof(BaseWeapon))]
    public class CombatBaseAI  : BaseAI {
        private const float MIN_AGRO = 80;
        private const float MAX_AGRO = 350;

        public const float RESET_TARGET_INTERVAL = 20;
        private static ILogger log = LogManager.GetCurrentClassLogger();
        public NpcObjectOwner objectOwner { get; private set; }
        protected BaseWeapon mWeapon;
        protected CharacterObject mCharacter;
        protected MovableObject mMovable;
        protected PlayerTarget mTarget;
        private ShipWeapon mShipWeapon;
        private BotObject mBotObject;
        public enum MovingNearTarget { Circle, LIne }

        protected float mChestLiveDuration;
        protected bool mDead = false;
        private float mShotCooldown = 3.0f;
        private float mShotTimer;
        private float mWaitTimer = 5;
        private float mResetTargetTimer = RESET_TARGET_INTERVAL;
        private MovingNearTarget mMovNearTargetType;
        private CombatAIType combatAIType;
        private Vector3 mStartPosition;
        private bool mReturningToStartPosition = false;
        private bool mUseHitProbForAgro = false;
        

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["chest_life_interval"] = mChestLiveDuration.ToString();
            hash["is_dead"] = mDead.ToString();
            hash["try_shot_cooldown"] = mShotCooldown.ToString();
            hash["shot_timer"] = mShotTimer.ToString();
            hash["wait_timer"] = mWaitTimer.ToString();
            hash["reset_target_timer"] = mResetTargetTimer.ToString();
            hash["moving_in_battle_type"] = mMovNearTargetType.ToString();
            hash["combat_ai_type"] = combatAIType.ToString();
            hash["start_position"] = mStartPosition.ToString();
            hash["is_in_returning_to_start_position"] = mReturningToStartPosition.ToString();
            hash["use_hit_prob_for_agro"] = mUseHitProbForAgro.ToString();
            return hash;
        }

        public void Init(CombatBaseAIComponentData data) {
            base.Init(data);
            mUseHitProbForAgro = data.useHitProbForAgro;
        }
#if USE_SKILLS
        private List<string> skills = new List<string> { "0000042A", "0000042B", "0000042C", "0000042D", "0000042E", "0000042F",
        "0000040C", "0000040D", "0000040E", "0000040F", "00000410", "00000411", "000003EE", "000003EF", "000003F0",
        "000003F1", "000003F2", "000003F3" };
#endif
        public override void Start() {
            base.Start();
            mWeapon = RequireComponent<BaseWeapon>();
            mMessage = RequireComponent<MmoMessageComponent>();
            mCharacter = RequireComponent<CharacterObject>();
            mTarget = RequireComponent<PlayerTarget>();
            mMovable = GetComponent<MovableObject>();
            mChestLiveDuration = nebulaObject.world.Resource().ServerInputs.GetValue<float>("chest_life");
            //log.InfoFormat("chest life = {0}", mChestLiveDuration);
            mShotTimer = mShotCooldown;

            mDead = false;

            combatAIType = aiType as CombatAIType;
            if(combatAIType == null ) {
                log.Error("CombatBasseAI must have CombatAIType, but not simple AIType");
            }
            mShipWeapon = GetComponent<ShipWeapon>();
            if(Rand.Int() % 2 == 0 ) {
                mMovNearTargetType = MovingNearTarget.Circle;
            }  else {
                mMovNearTargetType = MovingNearTarget.LIne;
            }
            mStartPosition = nebulaObject.transform.position;

#if USE_SKILLS
            //--------------------------TESTING------------------------------------
            var ship = GetComponent<BaseShip>();
            if (ship) {
                string sSkill = skills[Rand.Int(skills.Count - 1)];
                ship.shipModel.Slot(ShipModelSlotType.CB).Module.SetSkill(SkillExecutor.SkilIDFromHexString(sSkill));
                mSkills = GetComponent<PlayerSkills>();
                mSkills.UpdateSkills(ship.shipModel);
            }
            //--------------------------------------------------------------------
#endif

            mBotObject = GetComponent<BotObject>();
        }

        private bool isTurret {
            get {
                if(mBotObject != null && mBotObject.isTurret) {
                    return true;
                }
                return false;
            }
        }

        protected virtual Vector3 GetStartPosition() {
            return mStartPosition;
        }

        private bool ValidateTarget() {
            var targetCharacter = mTarget.targetObject.GetComponent<CharacterObject>();
            if(!targetCharacter) {
                mTarget.Clear();
                return false;
            }
            if((mCharacter.RelationTo(targetCharacter) != FractionRelation.Enemy ) &&
                (mCharacter.RelationTo(targetCharacter) != FractionRelation.Neutral)) {
                mTarget.Clear();
                return false;
            }
            return true;
        }

        public override void Update(float deltaTime) {

            try {

                if ( nebulaObject.mmoWorld().playerCountOnStartFrame == 0) {
                    return;
                }

                if(mWaitTimer > 0f) {
                    mWaitTimer -= deltaTime;
                }

                if(mReturningToStartPosition) {
                    MoveToStartPosition(deltaTime);
                    return;
                } else {

                }

                if (mTarget.hasTarget) {

                    if (!ValidateTarget()) {
                        return;
                    }

                    bool targetDestroyed = (!mTarget.targetObject);
                    if (targetDestroyed) {
                        mTarget.Clear();
                        return;
                    } else {

                        float hitProb = mWeapon.HitProbTo(mTarget.targetObject);
                        mResetTargetTimer -= deltaTime;
                        if (hitProb < 0.1f && (mResetTargetTimer <= 0f)) {
                            mTarget.Clear();
                            OnStartIdle(deltaTime);
                            return;
                        } else {

                            if (NeedReturn()) {
                                mTarget.Clear();
                                StartReturn();
                                return;
                            }

                            if (combatAIType.battleMovingType == AttackMovingType.AttackPurchase) {
                                var direction = transform.DirectionTo(mTarget.targetObject.transform);
                                if (hitProb < 1f) {

                                    var newPos = transform.position + direction * mMovable.speed * deltaTime;
                                    var newRot = ComputeRotation(direction, mRotationSpeed, deltaTime);

                                    Move(transform.position, transform.rotation, newPos, newRot.eulerAngles, mMovable.speed);
                                } else {
                                    /*
                                    var newPos = transform.position;
                                    var newRot = ComputeRotation(direction, mRotationSpeed, deltaTime);
                                    Move(transform.position, transform.rotation, newPos, newRot.eulerAngles, mMovable.speed);
                                    */
                                    if (mMovNearTargetType == MovingNearTarget.Circle) {
                                        MoveAtCircleAboutTarget(deltaTime);
                                    } else if(mMovNearTargetType == MovingNearTarget.LIne) {
                                        MoveAtDirectionNearTarget(deltaTime);
                                    }
                                }
                            } else if ( combatAIType.battleMovingType == AttackMovingType.AttackIdle ) {
                                OnDoIdle(deltaTime);
                            } else if(combatAIType.battleMovingType == AttackMovingType.AttackStay) {
                                //when stay at attacking only rotate to target
                                var direction = transform.DirectionTo(mTarget.targetObject.transform);
                                var newPos = transform.position;
                                var newRot = ComputeRotation(direction, mRotationSpeed, deltaTime);
                                Move(transform.position, transform.rotation, newPos, newRot.eulerAngles, mMovable.speed);
                            }

                            mShotTimer -= deltaTime;
                            if(mShotTimer <= 0f ) {
                                mShotTimer = mShotCooldown;
                                if (mWaitTimer <= 0f) {
#if USE_SKILLS
                                    if (mSkills) {
                                        if (mSkills.GetSkillByPosition(0).ready) {
                                            mSkills.UseSkill(0, mTarget.targetObject);
                                        }
                                    }
#else
                                        WeaponHitInfo hit;
                                        var shotInfo = mWeapon.Fire(out hit);
                                        mMessage.SendShot(EventReceiver.ItemSubscriber, shotInfo);
#endif
                                }
                            }
                        }
                    }
                } else {
                    if (!mDead) {
                        if (FindCombatTarget()) {
                            return;
                        }
                        OnDoIdle(deltaTime);
                    }
                }
            }catch(Exception exception) {
                log.Error(exception);
                log.Error(exception.StackTrace);
            }
        }

        private void StartReturn() {

            
            mReturningToStartPosition = true;
            nebulaObject.SendMessage(ComponentMessages.OnReturnStateChanged, mReturningToStartPosition);
        }

        private bool NeedReturn() {
            if(isTurret) {
                return false;
            }

            float distance = (GetStartPosition() - transform.position).magnitude;
            return (distance > 500);
        }

        private void MoveToStartPosition(float deltaTime) {
            Vector3 direction = (GetStartPosition() - transform.position).normalized;
            var newPos = transform.position + direction * mMovable.speed * deltaTime;
            var newRot = ComputeRotation(direction, mRotationSpeed, deltaTime);
            Move(transform.position, transform.rotation, newPos, newRot.eulerAngles, mMovable.speed);
            float dist = (GetStartPosition() - transform.position).magnitude;
            if(dist < 3) {
                mReturningToStartPosition = false;
                nebulaObject.SendMessage(ComponentMessages.OnReturnStateChanged, mReturningToStartPosition);
            }
        }

        float rotAng = 10f * 3.14f / 160f;

        private void MoveAtCircleAboutTarget(float deltaTime) {
            float dist = transform.DistanceTo(mTarget.targetObject.transform);
            float ang = rotAng * deltaTime;
            Vector3 vec = transform.position - mTarget.targetObject.transform.position;
            Vector3 newVec = new Vector3(vec.X * Mathf.Cos(ang) + vec.Z * Mathf.Sin(ang), vec.Y, -vec.X * Mathf.Sin(ang) + vec.Z * Mathf.Cos(ang)).normalized * dist;
            Vector3 newPosss = mTarget.targetObject.transform.position + newVec;
            Vector3 direction = (newPosss - transform.position).normalized;

            var newPos = transform.position + direction * mMovable.speed * deltaTime;
            var newRot = ComputeRotation(direction, mRotationSpeed, deltaTime);

            Move(transform.position, transform.rotation, newPos, newRot.eulerAngles, mMovable.speed);
        }

        private Vector3 supportDir = new Vector3(0, 0, 1);
        private void MoveAtDirectionNearTarget(float deltaTime) {
            //float dist = supportDistance;
            //float realDist = transform.DistanceTo(mTarget.targetObject.transform);
            //if(realDist > dist) {
            //    supportDir = (mTarget.transform.position - transform.position).normalized;
            //}
            //var newPos = transform.position + supportDir * mMovable.speed * deltaTime;
            var newRot = ComputeRotation((mTarget.targetObject.transform.position - transform.position).normalized, mRotationSpeed, deltaTime);
            Move(transform.position, transform.rotation, transform.position, newRot.eulerAngles, mMovable.speed);
        }

        private float supportDistance {
            get {
                if(mShipWeapon) {
                    return mShipWeapon.optimalDistance;
                }
                return 50;
            }
        }

        public void SetObjectOwner(NpcObjectOwner owner) {
            objectOwner = owner;
        }

        public void Death() {

            //log.InfoFormat("create chest at position = {0}", transform.position);

            var damagable = GetComponent<DamagableObject>();

            if (damagable != null) {
                if (damagable.killed && damagable.createChestOnKilling) {
                    log.InfoFormat("generate chest for bot of level = {0} yellow", mCharacter.level);

                    Difficulty difficulty = Difficulty.none;
                    if (nebulaObject.HasTag((byte)PS.Difficulty)) {
                        difficulty = (Difficulty)(byte)nebulaObject.Tag((byte)PS.Difficulty);
                    }


                    if (ChestUtils.RollDropChest(difficulty)) {

                        log.InfoFormat("Successfully rolled chest for npc difficulty = {0} yellow", difficulty);

                        ChestSourceInfo sourceInfo = new ChestSourceInfo { hasWorkshop = true, level = mCharacter.level, sourceWorkshop = (Workshop)mCharacter.workshop, difficulty = difficulty };
                        var dropListComponent = GetComponent<DropListComponent>();
                        ItemDropList itemDropList = null;
                        if(dropListComponent != null ) {
                            itemDropList = dropListComponent.dropList;
                        }

                        ObjectCreate.Chest(
                            nebulaObject.world as MmoWorld,
                            transform.position,
                            mChestLiveDuration,
                            GetComponent<DamagableObject>().damagers,
                            sourceInfo,
                            itemDropList
                            ).AddToWorld();
                    }
                }
            }

            mDead = true;

            if(objectOwner != null ) {
                objectOwner.HandleDeath(nebulaObject);
            }

            (nebulaObject.world as MmoWorld).npcManager.SetDestroyTime(nebulaObject.Id, Time.curtime());

            (nebulaObject as Item).Destroy();
        }

        protected virtual bool FindCombatTarget() {

            FractionType selfFraction = (FractionType)mCharacter.fraction;

            if(mReturningToStartPosition) {
                return false;
            }

            if(selfFraction == FractionType.BotNeutral) {
                return false;
            }


            var it = (nebulaObject.world as MmoWorld).GetItem((item) => {
                //don't look at self
                if (item.Id == nebulaObject.Id) {
                    return false;
                }
                //don't look invisible items
                if(item.invisible) {
                    return false;
                }

                var itemDamagable = item.GetComponent<DamagableObject>();
                var itemCharacter = item.GetComponent<CharacterObject>();

               
                if (itemDamagable && itemCharacter) {
                    var itemFraction = (FractionType)itemCharacter.fraction;
                    if (nebulaObject.resource.fractionResolver.RelationFor(selfFraction).RelationTo(itemFraction) == FractionRelation.Enemy) {

                        float distance = transform.DistanceTo(item.transform);

                        if (mUseHitProbForAgro) {
                            float hitProb = mWeapon.HitProbTo(item);
                            if (hitProb >= 0.5f && distance < MAX_AGRO ) {
                                return true;
                            }
                        } else {
                            float d = transform.DistanceTo(item.transform);
                            if (d < Mathf.Clamp(0.5f * mWeapon.optimalDistance, MIN_AGRO, MAX_AGRO)) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            });
            if(it) {
                mStartPosition = transform.position;
                mTarget.SetTarget(it);
                return true;
            }
            return false;
        }

        public void OnNewDamage(DamageInfo info) {
            if (mTarget != null) {
                if (!mTarget.hasTarget) {
                    NebulaObject attacker;
                    if ((nebulaObject.world as MmoWorld).TryGetObject((byte)info.DamagerType, info.DamagerId, out attacker)) {

                        mResetTargetTimer = RESET_TARGET_INTERVAL;
                        mTarget.SetTarget(attacker);
                    }
                }
            }
        }

    }
}

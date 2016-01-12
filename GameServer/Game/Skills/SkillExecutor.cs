using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Pets;
using Space.Server;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Skills {

    public abstract class SkillExecutor
    {


        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public abstract bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info);

        //private static Dictionary<int, SkillExecutor> executors;
        private static Dictionary<int, System.Func<SkillExecutor>> executorFactory;

        private static EmptySkillExecutor emptyExecutor;

        //public virtual void Update(PlayerSkills skills, float deltaTime) {

        //}


        static SkillExecutor() {
            emptyExecutor = new EmptySkillExecutor();

            //executors = new Dictionary<int, SkillExecutor>();
            //executors.Add(SkilIDFromHexString("000003E8"), new Skill_000003E8());
            //executors.Add(SkilIDFromHexString("000003E9"), new Skill_000003E9());
            //executors.Add(SkilIDFromHexString("000003F5"), new Skill_000003F5());
            //executors.Add(SkilIDFromHexString("000003EE"), new Skill_000003EE());
            //executors.Add(SkilIDFromHexString("000003FB"), new Skill_000003FB());
            //executors.Add(SkilIDFromHexString("00000400"), new Skill_00000400());
            //executors.Add(SkilIDFromHexString("000003EA"), new Skill_000003EA());
            //executors.Add(SkilIDFromHexString("000003EB"), new Skill_000003EB());
            //executors.Add(SkilIDFromHexString("000003EC"), new Skill_000003EC());
            //executors.Add(SkilIDFromHexString("000003ED"), new Skill_000003ED());
            //executors.Add(SkilIDFromHexString("000003EF"), new Skill_000003EF());
            //executors.Add(SkilIDFromHexString("00000406"), new Skill_00000406());
            //executors.Add(SkilIDFromHexString("00000407"), new Skill_00000407());
            //executors.Add(SkilIDFromHexString("0000040C"), new Skill_0000040C());
            //executors.Add(SkilIDFromHexString("0000040D"), new Skill_0000040D());
            //executors.Add(SkilIDFromHexString("00000412"), new Skill_00000412());
            //executors.Add(SkilIDFromHexString("00000413"), new Skill_00000413());
            //executors.Add(SkilIDFromHexString("00000418"), new Skill_00000418());
            //executors.Add(SkilIDFromHexString("00000419"), new Skill_00000419());
            //executors.Add(SkilIDFromHexString("0000041E"), new Skill_0000041E());
            //executors.Add(SkilIDFromHexString("0000041F"), new Skill_0000041F());
            //executors.Add(SkilIDFromHexString("00000424"), new Skill_00000424());
            //executors.Add(SkilIDFromHexString("00000425"), new Skill_00000425());
            //executors.Add(SkilIDFromHexString("0000042A"), new Skill_0000042A());
            //executors.Add(SkilIDFromHexString("0000042B"), new Skill_0000042B());
            //executors.Add(SkilIDFromHexString("00000430"), new Skill_00000430());
            //executors.Add(SkilIDFromHexString("00000431"), new Skill_00000431());
            //executors.Add(SkilIDFromHexString("00000436"), new Skill_00000436());
            //executors.Add(SkilIDFromHexString("00000437"), new Skill_00000437());
            //executors.Add(SkilIDFromHexString("0000043C"), new Skill_0000043C());
            //executors.Add(SkilIDFromHexString("0000043D"), new Skill_0000043D());
            //executors.Add(SkilIDFromHexString("00000442"), new Skill_00000442());
            //executors.Add(SkilIDFromHexString("00000443"), new Skill_00000443());
            //executors.Add(SkilIDFromHexString("00000448"), new Skill_00000448());
            //executors.Add(SkilIDFromHexString("00000449"), new Skill_00000449());
            //executors.Add(SkilIDFromHexString("0000044E"), new Skill_0000044E());
            //executors.Add(SkilIDFromHexString("0000044F"), new Skill_0000044F());
            //executors.Add(SkilIDFromHexString("0000045A"), new Skill_0000045A());
            //executors.Add(SkilIDFromHexString("0000045B"), new Skill_0000045B());
            //executors.Add(SkilIDFromHexString("00000454"), new Skill_00000454());
            //executors.Add(SkilIDFromHexString("00000455"), new Skill_00000455());
            //executors.Add(SkilIDFromHexString("000003F4"), new Skill_000003F4());
            //executors.Add(SkilIDFromHexString("000003FA"), new Skill_000003FA());
            //executors.Add(SkilIDFromHexString("00000401"), new Skill_00000401());
            //executors.Add(SkilIDFromHexString("000007D0"), new Skill_000007D0());
            //executors.Add(SkilIDFromHexString("000007D5"), new Skill_000007D5());
            //executors.Add(SkilIDFromHexString("000007DA"), new Skill_000007DA());
            //executors.Add(SkilIDFromHexString("000007DF"), new Skill_000007DF());

            executorFactory = new Dictionary<int, System.Func<SkillExecutor>>();
            executorFactory.Add(SkilIDFromHexString("000003E8"),()=> new Skill_000003E8());
            executorFactory.Add(SkilIDFromHexString("000003E9"),()=> new Skill_000003E9());
            executorFactory.Add(SkilIDFromHexString("000003F5"),()=> new Skill_000003F5());
            executorFactory.Add(SkilIDFromHexString("000003EE"),()=> new Skill_000003EE());
            executorFactory.Add(SkilIDFromHexString("000003FB"),()=> new Skill_000003FB());
            executorFactory.Add(SkilIDFromHexString("00000400"),()=> new Skill_00000400());
            executorFactory.Add(SkilIDFromHexString("000003EA"),()=> new Skill_000003EA());
            executorFactory.Add(SkilIDFromHexString("000003EB"),()=> new Skill_000003EB());
            executorFactory.Add(SkilIDFromHexString("000003EC"),()=> new Skill_000003EC());
            executorFactory.Add(SkilIDFromHexString("000003ED"),()=> new Skill_000003ED());
            executorFactory.Add(SkilIDFromHexString("000003EF"),()=> new Skill_000003EF());
            executorFactory.Add(SkilIDFromHexString("00000406"),()=> new Skill_00000406());
            executorFactory.Add(SkilIDFromHexString("00000407"),()=> new Skill_00000407());
            executorFactory.Add(SkilIDFromHexString("0000040C"),()=> new Skill_0000040C());
            executorFactory.Add(SkilIDFromHexString("0000040D"),()=> new Skill_0000040D());
            executorFactory.Add(SkilIDFromHexString("00000412"),()=> new Skill_00000412());
            executorFactory.Add(SkilIDFromHexString("00000413"),()=> new Skill_00000413());
            executorFactory.Add(SkilIDFromHexString("00000418"),()=> new Skill_00000418());
            executorFactory.Add(SkilIDFromHexString("00000419"),()=> new Skill_00000419());
            executorFactory.Add(SkilIDFromHexString("0000041E"),()=> new Skill_0000041E());
            executorFactory.Add(SkilIDFromHexString("0000041F"),()=> new Skill_0000041F());
            executorFactory.Add(SkilIDFromHexString("00000424"),()=> new Skill_00000424());
            executorFactory.Add(SkilIDFromHexString("00000425"),()=> new Skill_00000425());
            executorFactory.Add(SkilIDFromHexString("0000042A"),()=> new Skill_0000042A());
            executorFactory.Add(SkilIDFromHexString("0000042B"),()=> new Skill_0000042B());
            executorFactory.Add(SkilIDFromHexString("00000430"),()=> new Skill_00000430());
            executorFactory.Add(SkilIDFromHexString("00000431"),()=> new Skill_00000431());
            executorFactory.Add(SkilIDFromHexString("00000436"),()=> new Skill_00000436());
            executorFactory.Add(SkilIDFromHexString("00000437"),()=> new Skill_00000437());
            executorFactory.Add(SkilIDFromHexString("0000043C"),()=> new Skill_0000043C());
            executorFactory.Add(SkilIDFromHexString("0000043D"),()=> new Skill_0000043D());
            executorFactory.Add(SkilIDFromHexString("00000442"),()=> new Skill_00000442());
            executorFactory.Add(SkilIDFromHexString("00000443"),()=> new Skill_00000443());
            executorFactory.Add(SkilIDFromHexString("00000448"),()=> new Skill_00000448());
            executorFactory.Add(SkilIDFromHexString("00000449"),()=> new Skill_00000449());
            executorFactory.Add(SkilIDFromHexString("0000044E"),()=> new Skill_0000044E());
            executorFactory.Add(SkilIDFromHexString("0000044F"),()=> new Skill_0000044F());
            executorFactory.Add(SkilIDFromHexString("0000045A"),()=> new Skill_0000045A());
            executorFactory.Add(SkilIDFromHexString("0000045B"),()=> new Skill_0000045B());
            executorFactory.Add(SkilIDFromHexString("00000454"),()=> new Skill_00000454());
            executorFactory.Add(SkilIDFromHexString("00000455"),()=> new Skill_00000455());
            executorFactory.Add(SkilIDFromHexString("000003F4"),()=> new Skill_000003F4());
            executorFactory.Add(SkilIDFromHexString("000003FA"),()=> new Skill_000003FA());
            executorFactory.Add(SkilIDFromHexString("00000401"),()=> new Skill_00000401());
            executorFactory.Add(SkilIDFromHexString("000007D0"),()=> new Skill_000007D0());
            executorFactory.Add(SkilIDFromHexString("000007D5"),()=> new Skill_000007D5());
            executorFactory.Add(SkilIDFromHexString("000007DA"),()=> new Skill_000007DA());
            executorFactory.Add(SkilIDFromHexString("000007DF"),()=> new Skill_000007DF());
            executorFactory.Add(SkilIDFromHexString("000003F0"), () => new Skill_000003F0());
            executorFactory.Add(SkilIDFromHexString("000003F1"), () => new Skill_000003F1());
            executorFactory.Add(SkilIDFromHexString("000003F2"), () => new Skill_000003F2());
            executorFactory.Add(SkilIDFromHexString("000003F3"), () => new Skill_000003F3());
            executorFactory.Add(SkilIDFromHexString("000003F6"), () => new Skill_000003F6());
            executorFactory.Add(SkilIDFromHexString("000003F7"), () => new Skill_000003F7());
            executorFactory.Add(SkilIDFromHexString("000003F8"), () => new Skill_000003F8());
            executorFactory.Add(SkilIDFromHexString("000003FC"), () => new EmptySkillExecutor());
            executorFactory.Add(SkilIDFromHexString("000003FD"), () => new Skill_000003FD());
            executorFactory.Add(SkilIDFromHexString("000003FE"), () => new Skill_000003FE());
            executorFactory.Add(SkilIDFromHexString("000003FF"), () => new Skill_000003FF());
            executorFactory.Add(SkilIDFromHexString("00000402"), () => new Skill_00000402());
            executorFactory.Add(SkilIDFromHexString("00000403"), () => new Skill_00000403());
            executorFactory.Add(SkilIDFromHexString("00000404"), () => new Skill_00000404());
            executorFactory.Add(SkilIDFromHexString("00000405"), () => new Skill_00000405());
            executorFactory.Add(SkilIDFromHexString("000007D1"), () => new Skill_000007D1());
            executorFactory.Add(SkilIDFromHexString("00000408"), () => new Skill_00000408());
            executorFactory.Add(SkilIDFromHexString("00000409"), () => new Skill_00000409());
            executorFactory.Add(SkilIDFromHexString("0000040A"), () => new Skill_0000040A());
            executorFactory.Add(SkilIDFromHexString("0000040B"), () => new Skill_0000040B());
            executorFactory.Add(SkilIDFromHexString("0000040E"), () => new Skill_0000040E());
            executorFactory.Add(SkilIDFromHexString("0000040F"), () => new Skill_0000040F());
            executorFactory.Add(SkilIDFromHexString("00000410"), () => new Skill_00000410());
            executorFactory.Add(SkilIDFromHexString("00000411"), () => new Skill_00000411());
            executorFactory.Add(SkilIDFromHexString("00000414"), () => new Skill_00000414());
            executorFactory.Add(SkilIDFromHexString("00000415"), () => new Skill_00000415());
            executorFactory.Add(SkilIDFromHexString("00000416"), () => new Skill_00000416());
            executorFactory.Add(SkilIDFromHexString("0000041A"), () => new Skill_0000041A());
            executorFactory.Add(SkilIDFromHexString("0000041B"), () => new Skill_0000041B());
            executorFactory.Add(SkilIDFromHexString("0000041C"), () => new Skill_0000041C());
            executorFactory.Add(SkilIDFromHexString("0000041D"), () => new Skill_0000041D());
            executorFactory.Add(SkilIDFromHexString("00000420"), () => new Skill_00000420());
            executorFactory.Add(SkilIDFromHexString("00000421"), () => new Skill_00000421());
            executorFactory.Add(SkilIDFromHexString("00000422"), () => new Skill_00000422());
            executorFactory.Add(SkilIDFromHexString("00000423"), () => new Skill_00000423());
            executorFactory.Add(SkilIDFromHexString("000007D6"), () => new Skill_000007D6());
            executorFactory.Add(SkilIDFromHexString("00000426"), () => new Skill_00000426());
            executorFactory.Add(SkilIDFromHexString("00000427"), () => new Skill_00000427());
            executorFactory.Add(SkilIDFromHexString("00000428"), () => new Skill_00000428());
            executorFactory.Add(SkilIDFromHexString("00000429"), () => new Skill_00000429());
            executorFactory.Add(SkilIDFromHexString("0000042C"), () => new Skill_0000042C());
            executorFactory.Add(SkilIDFromHexString("0000042D"), () => new Skill_0000042D());
            executorFactory.Add(SkilIDFromHexString("0000042E"), () => new Skill_0000042E());
            executorFactory.Add(SkilIDFromHexString("0000042F"), () => new Skill_0000042F());
            executorFactory.Add(SkilIDFromHexString("00000432"), () => new Skill_00000432());
            executorFactory.Add(SkilIDFromHexString("00000433"), () => new Skill_00000433());
            executorFactory.Add(SkilIDFromHexString("00000434"), () => new Skill_00000434());
            executorFactory.Add(SkilIDFromHexString("00000435"), () => new EmptySkillExecutor());
            executorFactory.Add(SkilIDFromHexString("00000438"), () => new Skill_00000438());
            executorFactory.Add(SkilIDFromHexString("00000439"), () => new Skill_00000439());
            executorFactory.Add(SkilIDFromHexString("0000043A"), () => new Skill_0000043A());
            executorFactory.Add(SkilIDFromHexString("0000043B"), () => new Skill_0000043B());
            executorFactory.Add(SkilIDFromHexString("0000043E"), () => new Skill_0000043E());
            executorFactory.Add(SkilIDFromHexString("0000043F"), () => new Skill_0000043F());
            executorFactory.Add(SkilIDFromHexString("00000440"), () => new Skill_00000440());
            executorFactory.Add(SkilIDFromHexString("00000441"), () => new Skill_00000441());
            executorFactory.Add(SkilIDFromHexString("000007DB"), () => new Skill_000007DB());
            executorFactory.Add(SkilIDFromHexString("000003F9"), () => new EmptySkillExecutor());
            executorFactory.Add(SkilIDFromHexString("00000444"), () => new Skill_00000444());
            executorFactory.Add(SkilIDFromHexString("00000445"), () => new Skill_00000445());
            executorFactory.Add(SkilIDFromHexString("00000446"), () => new Skill_00000446());
            executorFactory.Add(SkilIDFromHexString("00000447"), () => new Skill_00000447());
            executorFactory.Add(SkilIDFromHexString("0000044A"), () => new Skill_0000044A());
            executorFactory.Add(SkilIDFromHexString("0000044B"), () => new Skill_0000044B());
            executorFactory.Add(SkilIDFromHexString("0000044C"), () => new Skill_0000044C());
            executorFactory.Add(SkilIDFromHexString("0000044D"), () => new Skill_0000044D());
            executorFactory.Add(SkilIDFromHexString("00000450"), () => new Skill_00000450());
            executorFactory.Add(SkilIDFromHexString("00000451"), () => new Skill_00000451());
            executorFactory.Add(SkilIDFromHexString("00000452"), () => new Skill_00000452());
            executorFactory.Add(SkilIDFromHexString("00000453"), () => new Skill_00000453());
            executorFactory.Add(SkilIDFromHexString("00000456"), () => new Skill_00000456());
            executorFactory.Add(SkilIDFromHexString("00000457"), () => new Skill_00000457());
            executorFactory.Add(SkilIDFromHexString("00000458"), () => new Skill_00000458());
            executorFactory.Add(SkilIDFromHexString("0000045C"), () => new Skill_0000045C());
            executorFactory.Add(SkilIDFromHexString("0000045D"), () => new Skill_0000045D());
            executorFactory.Add(SkilIDFromHexString("0000045E"), () => new Skill_0000045E());
            executorFactory.Add(SkilIDFromHexString("0000045F"), () => new Skill_0000045F());
            executorFactory.Add(SkilIDFromHexString("000007E0"), () => new Skill_000007E0());
            executorFactory.Add(SkilIDFromHexString("00000459"), () => new Skill_00000459());
        }

        protected bool RollMastery(NebulaObject source) {
            var pm = source.GetComponent<PetManager>();
            if(pm) {
                return pm.RollMastery();
            }
            return false;
        }

        public static int SkilIDFromHexString(string str) {
            return int.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        //public static SkillExecutor Executor(int skillID) {
        //    if(executors.ContainsKey(skillID)) {
        //        return executors[skillID];
        //    }
        //    return emptyExecutor;
        //}

        public static System.Func<SkillExecutor> Factory(int skillID) {
            if(executorFactory.ContainsKey(skillID)) {
                return executorFactory[skillID];
            }
            return () => new EmptySkillExecutor();
        }

        /// <summary>
        /// Get items which allowed be targets on healing when used area healing
        /// </summary>
        /// <param name="source">Who healed</param>
        /// <param name="target">What center object</param>
        /// <param name="radius">Radius where see items</param>
        /// <returns>Return dictionary of founded items</returns>
        protected ConcurrentDictionary<string, Item> GetHealTargets(NebulaObject source, NebulaObject target, float radius ) {
            var sourceCharacter = source.Character();
            return source.mmoWorld().GetItems((item) => {
                float distance = target.transform.DistanceTo(item.transform);
                if (distance < radius) {
                    var itemDamagable = item.Damagable();
                    var itemCharacter = item.Character();
                    var itemBonuses = item.Bonuses();
                    bool allItemComponentsPresent = itemDamagable && itemCharacter && itemBonuses;
                    if (allItemComponentsPresent) {
                        if (item.Id != target.Id && (item.Id != source.Id)) {
                            var relation = sourceCharacter.RelationTo(itemCharacter);
                            if (relation == FractionRelation.Friend) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            });
        }

        //Check range heal to ally when using skill
        protected bool CheckForHealAlly(NebulaObject source) {
            var targetComponent = source.Target();
            var weaponComponent = source.Weapon();
            var characterComponent = source.Character();


            bool allSourceComponentsPresent = targetComponent && weaponComponent && characterComponent;

            if(allSourceComponentsPresent) {
                var targetObject = targetComponent.targetObject;
                if(targetObject) {
                    var targetBonuses = targetObject.Bonuses();
                    var targetCharacter = targetObject.Character();
                    var targetDamagable = targetObject.Damagable();
                    bool allTargetComponentsPresent = targetBonuses && targetCharacter && targetDamagable;
                    if(allTargetComponentsPresent) {
                        var relation = characterComponent.RelationTo(targetCharacter);
                        if(relation == FractionRelation.Friend) {
                            float distanceToTarget = source.transform.DistanceTo(targetObject.transform);
                            if(distanceToTarget <= weaponComponent.optimalDistance) {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        protected bool ShotToEnemyRestricted(NebulaObject source, PlayerSkill skill ) {
            return (!CheckForShotEnemy(source, skill));
        }

        protected bool CheckForShotEnemy(NebulaObject source, PlayerSkill skill) {
            var sourceTarget = source.GetComponent<PlayerTarget>();
            if (!sourceTarget.hasTarget) {
                log.InfoFormat("Skill {0} error: source don't have target", skill.data.Id.ToString("X8"));
                return false;
            }
            if (!sourceTarget.targetObject) {
                log.InfoFormat("Skill {0} error: source target object invalid", skill.data.Id.ToString("X8"));
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            if (!sourceWeapon) {
                log.InfoFormat("Skill {0} error: source don't has weapon", skill.data.Id.ToString("X8"));
                return false;
            }
            if (Mathf.Approximately(sourceWeapon.HitProbTo(sourceTarget.nebulaObject), 0f)) {
                log.InfoFormat("Skill {0} error: hit prob is 0", skill.data.Id.ToString("X8"));
                return false;
            }

            var targetBonuses = sourceTarget.targetObject.GetComponent<PlayerBonuses>();
            if (!targetBonuses) {
                log.InfoFormat("Skill {0} error: target don't has Bonuses component", skill.data.Id.ToString("X8"));
                return false;
            }

            var sourceCharacter = source.GetComponent<CharacterObject>();
            var targetCharacter = sourceTarget.targetObject.GetComponent<CharacterObject>();
            if (!sourceTarget || !targetCharacter) {
                log.InfoFormat("Skill {0} error: source or target don't has character component", skill.data.Id.ToString("X8"));
                return false;
            }


            if (sourceCharacter.RelationTo(targetCharacter) == FractionRelation.Friend) {
                log.InfoFormat("Skill {0} error: source and target in friend fraction, source fraction = {1}, target fraction = {2}",
                    skill.data.Id.ToString("X8"), (FractionType)(byte)sourceCharacter.fraction, (FractionType)(byte)targetCharacter.fraction);
                return false;
            }
            return true;
        }

        protected bool CheckForShotFriend(NebulaObject source, PlayerSkill skill ) {
            var sourceTarget = source.GetComponent<PlayerTarget>();
            if (!sourceTarget.hasTarget) {
                log.InfoFormat("Skill {0} error: source don't have target", skill.data.Id.ToString("X8"));
                return false;
            }
            if (!sourceTarget.targetObject) {
                log.InfoFormat("Skill {0} error: source target object invalid", skill.data.Id.ToString("X8"));
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            if (!sourceWeapon) {
                log.InfoFormat("Skill {0} error: source don't has weapon", skill.data.Id.ToString("X8"));
                return false;
            }
            if (Mathf.Approximately(sourceWeapon.HitProbTo(sourceTarget.nebulaObject), 0f)) {
                log.InfoFormat("Skill {0} error: hit prob is 0", skill.data.Id.ToString("X8"));
                return false;
            }

            var targetBonuses = sourceTarget.targetObject.GetComponent<PlayerBonuses>();
            if (!targetBonuses) {
                log.InfoFormat("Skill {0} error: target don't has Bonuses component", skill.data.Id.ToString("X8"));
                return false;
            }

            var sourceCharacter = source.GetComponent<CharacterObject>();
            var targetCharacter = sourceTarget.targetObject.GetComponent<CharacterObject>();
            if (!sourceTarget || !targetCharacter) {
                log.InfoFormat("Skill {0} error: source or target don't has character component", skill.data.Id.ToString("X8"));
                return false;
            }


            if (sourceCharacter.RelationTo(targetCharacter) == FractionRelation.Enemy) {
                log.InfoFormat("Skill {0} error: source and target in friend fraction, source fraction = {1}, target fraction = {2}",
                    skill.data.Id.ToString("X8"), (FractionType)(byte)sourceCharacter.fraction, (FractionType)(byte)targetCharacter.fraction);
                return false;
            }
            return true;
        }


        protected ConcurrentDictionary<string, Item> GetTargets(NebulaObject source, NebulaObject centerObject, float radius) {
            var sourceCharacter = source.Character();
            return source.mmoWorld().GetItems((item) => {
                if (centerObject.transform.DistanceTo(item.transform) < radius) {
                    var damagable = item.Damagable();
                    var bonuses = item.Bonuses();
                    var character = item.Character();
                    bool allComponentsPresent = damagable && bonuses && character;

                    if (allComponentsPresent) {
                        if (item.Id != centerObject.Id && (item.Id != source.Id)) {

                            var relation = sourceCharacter.RelationTo(character);
                            if (relation == FractionRelation.Enemy || relation == FractionRelation.Neutral) {
                                return true;
                            }
                            //}
                        }
                    }
                }
                return false;
            });
        }

        protected void ActionOnEnemyTargets(System.Action<Item> action, NebulaObject source, NebulaObject centerObject, float radius) {
            var items = GetTargets(source, centerObject, radius);
            foreach(var pItem in items ) {
                var item = pItem.Value;
                action(item);
            }
        }

    }
}

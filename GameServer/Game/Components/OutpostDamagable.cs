using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Components {
    public class OutpostDamagable : FixedInputDamageDamagableObject, IDatabaseObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private Outpost mFortification;
        private MainOutpost mOutpost;

        private float mAdditionalHP;

        private OutpostDamagableComponentData mInitData;


        public void Init(OutpostDamagableComponentData data) {
            mInitData = data;
            base.Init(data);
            mAdditionalHP = data.additionalHP;
        }

        public override void Start() {
            base.Start();
            mFortification = GetComponent<Outpost>();
            mOutpost = GetComponent<MainOutpost>();
        }

        private float constructMultiplier {
            get {
                if (mFortification) {
                    return mFortification.constructProgress;
                }
                if(mOutpost) {
                    return mOutpost.constructProgress;
                }
                return 1f;
            }
        }

        

        public override float maximumHealth {
            get {
                var character = GetComponent<CharacterObject>();

                if(character) {
                    return (baseMaximumHealth + mAdditionalHP * character.level) * constructMultiplier;
                } else {
                    return baseMaximumHealth * constructMultiplier ;
                }
            }
        }

        public override InputDamage ReceiveDamage(InputDamage inputDamage) {
            InputDamage dmg =  base.ReceiveDamage(inputDamage);
            if(mOutpost) {
                log.InfoFormat("outpost receive damage input = {0} output = {1} [blue]", inputDamage.damage, dmg.damage);
            }
            return dmg;
        }

        public override void Death() {

            var world = nebulaObject.world as MmoWorld;
            var levelCalc = nebulaObject.resource.Leveling;
            float npcLevel = damagerCollection.GetLevel(nebulaObject);

            foreach(var p in damagers ) {
                var damager = p.Value as DamageInfo;
                if(damager.DamagerType != ItemType.Avatar ) {
                    continue;
                }

                //give every player exp for destroy outpost
                NebulaObject player;
                if(world.TryGetObject((byte)damager.DamagerType, damager.DamagerId, out player)) {
                    var playerCharacter = player.GetComponent<PlayerCharacterObject>();
                    if(!playerCharacter) {
                        continue;
                    }

                    int playerLevel = levelCalc.LevelForExp(playerCharacter.exp);
                    int expToNextLevel = levelCalc.ExpToNextLevel(playerLevel);

                    float expPC = 50.0f / (50.0f + playerLevel * 200.0f);
                    float finalExp = expToNextLevel * expPC;

                    playerCharacter.AddExp((int)finalExp);

                    damagerCollection.GivePvpPoints(player, nebulaObject, npcLevel, playerLevel);

                    log.InfoFormat("Outpost damagable death, {0} on level {1} receive exp {2}", 
                        playerCharacter.login, playerLevel, (int)finalExp);
                }
            }
            (nebulaObject as GameObject).Destroy();
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}

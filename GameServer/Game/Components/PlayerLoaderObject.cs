using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Contracts;
using Nebula.Game.Pets;
using Nebula.Game.Utils;
using Space.Game;
using System;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(MmoActor))]
    [REQUIRE_COMPONENT(typeof(PlayerShip))]
    [REQUIRE_COMPONENT(typeof(PlayerSkills))]
    public class PlayerLoaderObject : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private bool mLoaded = false;
        private MmoActor player;

        public override void Start() {
            base.Start();
            player = GetComponent<MmoActor>();
        }

        public override void Update(float deltaTime) {
            if(!mLoaded) {
                mLoaded = true;
                Load();
            }
        }

        public void Load() {
            if (!mLoaded) {
                mLoaded = true;
                log.InfoFormat("PlayerLoader Object Load() [dy]");
                
                GetComponent<MmoActor>().Load();
                GetComponent<PlayerShip>().Load();
                GetComponent<PlayerSkills>().Load();
                GetComponent<MmoActor>().LoadOther();
                GetComponent<ShipWeapon>().Load();
                GetComponent<PassiveBonusesComponent>().Load();
                GetComponent<PlayerTimedEffects>().Load();
                GetComponent<PetManager>().Load();
                GetComponent<ContractManager>().Load();
                GetComponent<AchievmentComponent>().Load();
            }
        }

        public void Save(bool forceSaveToDB) {
            if(mLoaded) {
                if(player == null) {
                    return;
                }
                var character = GetComponent<PlayerCharacterObject>();
                if(character == null) {
                    return;
                }

                try {
                    var app = nebulaObject.mmoWorld().application;
                    InventoryDatabase.instance(app).SaveInventory(character.characterId, player.Inventory);
                    StationDatabase.instance(app).SaveStation(character.characterId, player.Station);
                    CharacterDatabase.instance(app).SaveCharacter(character.characterId, player.GetPlayerCharacter());
                    ShipModelDatabase.instance(app).SaveShipModel(character.characterId, GetComponent<PlayerShip>().shipModel);
                    SkillDatabase.instance(app).SaveSkills(character.characterId, GetComponent<PlayerSkills>().GetSave());
                    WeaponDatabase.instance(app).SaveWeapon(character.characterId, player.GetComponent<ShipWeapon>().GetSave());
                    PassiveBonusesDatabase.instance(app).SavePassiveBonuses(character.characterId, player.GetComponent<PassiveBonusesComponent>().GetSave());
                    TimedEffectsDatabase.instance(app).SaveTimedEffects(character.characterId, player.GetComponent<PlayerTimedEffects>().GetInfo());
                    PetDatabase.instance(app).SavePets(character.characterId, player.GetComponent<PetManager>().pets);
                    ContractDatabase.instance(app).SaveContracts(character.characterId, player.GetComponent<ContractManager>().GetSave());
                    AchievmentDatabase.instance(app).SaveAchievment(character.characterId, player.GetComponent<AchievmentComponent>().GetSave());
                } catch(Exception exception) {
                    log.Error("handled exception at Player Loaded Object");
                    log.Error(exception.Message);
                    log.Error(exception.StackTrace);
                }
            }
        }

        public void SaveTimedEffects() {
            log.InfoFormat("save timed effects....".Color(LogColor.orange));
            var character = GetComponent<PlayerCharacterObject>();
            if (character == null) {
                return;
            }
            var app = nebulaObject.mmoWorld().application;
            TimedEffectsDatabase.instance(app).SaveTimedEffects(character.characterId, player.GetComponent<PlayerTimedEffects>().GetInfo());
        }

        public void SaveInventory() {
            var character = GetComponent<PlayerCharacterObject>();
            if (character == null) {
                return;
            }
            var app = nebulaObject.mmoWorld().application;
            InventoryDatabase.instance(app).SaveInventory(character.characterId, player.Inventory);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.PlayerLoader;
            }
        }

        public bool loaded {
            get {
                return mLoaded;
            }
        }

    }
}

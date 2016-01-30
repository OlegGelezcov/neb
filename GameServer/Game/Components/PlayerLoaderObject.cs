using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Contracts;
using Nebula.Game.Pets;
using Nebula.Game.Utils;
using Space.Game;

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
                InventoryDatabase.instance.SaveInventory(character.characterId, player.Inventory);
                StationDatabase.instance.SaveStation(character.characterId, player.Station);
                CharacterDatabase.instance.SaveCharacter(character.characterId, player.GetPlayerCharacter());
                ShipModelDatabase.instance.SaveShipModel(character.characterId, GetComponent<PlayerShip>().shipModel);
                SkillDatabase.instance.SaveSkills(character.characterId, GetComponent<PlayerSkills>().GetSave());
                WeaponDatabase.instance.SaveWeapon(character.characterId, player.GetComponent<ShipWeapon>().GetSave());
                PassiveBonusesDatabase.instance.SavePassiveBonuses(character.characterId, player.GetComponent<PassiveBonusesComponent>().GetSave());
                TimedEffectsDatabase.instance.SaveTimedEffects(character.characterId, player.GetComponent<PlayerTimedEffects>().GetInfo());
                PetDatabase.instance.SavePets(character.characterId, player.GetComponent<PetManager>().pets);
                ContractDatabase.instance.SaveContracts(character.characterId, player.GetComponent<ContractManager>().GetSave());
            }
        }

        public void SaveTimedEffects() {
            log.InfoFormat("save timed effects....".Color(LogColor.orange));
            var character = GetComponent<PlayerCharacterObject>();
            if (character == null) {
                return;
            }
            TimedEffectsDatabase.instance.SaveTimedEffects(character.characterId, player.GetComponent<PlayerTimedEffects>().GetInfo());
        }

        public void SaveInventory() {
            var character = GetComponent<PlayerCharacterObject>();
            if (character == null) {
                return;
            }
            InventoryDatabase.instance.SaveInventory(character.characterId, player.Inventory);
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

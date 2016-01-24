using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game.Drop;
using Space.Game.Ship;
using System.Collections.Generic;
using System.Collections;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(BaseWeapon))]
    [REQUIRE_COMPONENT(typeof(CharacterObject))]
    [REQUIRE_COMPONENT(typeof(MovableObject))]
    public class BotShip : BaseShip {

        private BaseWeapon mWeapon;
        private CharacterObject mCharacter;
        private PlayerBonuses mBonuses;


        private Difficulty mDifficulty = Difficulty.none;

        private bool modelExist = false;
        private bool initialized = false;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["exists_model?"] = modelExist.ToString();
            hash["initialized?"] = initialized.ToString();
            hash["difficulty"] = mDifficulty.ToString();
            hash["damage_resistance"] = damageResistance.ToString();
            hash["capacity"] = holdCapacity.ToString();
            return hash;
        }



        public void Init(BotShipComponentData data) {
            mDifficulty = data.difficulty;

            if (!initialized) {
                Initialize();
                GenerateModel();
            }

        }

        public override void Start() {
            if (!initialized) {
                Initialize();
                GenerateModel();
            }
        }

        private void Initialize() {
            if(!initialized) {
                initialized = true;
                mCharacter = RequireComponent<CharacterObject>();
                mWeapon = RequireComponent<BaseWeapon>();
                mBonuses = GetComponent<PlayerBonuses>();

                SetModel(new ShipModel(resource));
                
            }
        }

        public override float damageResistance {
            get {
                //check if resist blocked
                if(blockResist.blocked) {
                    return 0f;
                }
                if(shipModel != null ) {
                    float val = shipModel.resistance;
                    if(mBonuses) {
                        val *= Mathf.Clamp01((1f + Mathf.Clamp(mBonuses.resistPcBonus, -1, 1)));
                        val += mBonuses.resistCntBonus;
                        val = Mathf.Clamp01(val);
                    }
                    return val;
                } else {
                    return 0f;
                }
            }
        }

        public override int holdCapacity {
            get {
                return 0;
            }
        }

        public Difficulty difficulty {
            get {
                return mDifficulty;
            }
        }

        public override void Update(float deltaTime) {

            base.Update(deltaTime);

            if(nebulaObject.mmoWorld().playerCountOnStartFrame == 0) {
                return;
            }

            nebulaObject.properties.SetProperty((byte)PS.AngleSpeed, 0.5f);
            nebulaObject.properties.SetProperty((byte)PS.Acceleration, 20.0f);
            nebulaObject.properties.SetProperty((byte)PS.ModulePrefabs, shipModel.GetModulePrefabs());
        }


        public void GenerateModel() {
            if (!modelExist) {
                modelExist = true;
                Initialize();
                var dropManager = DropManager.Get( resource);
                GenerateModule(dropManager, ShipModelSlotType.CB);
                GenerateModule(dropManager, ShipModelSlotType.CM);
                GenerateModule(dropManager, ShipModelSlotType.DF);
                GenerateModule(dropManager, ShipModelSlotType.DM);
                GenerateModule(dropManager, ShipModelSlotType.ES);
                props.SetProperty((byte)PS.ModelInfo, shipModel.ModelHash());
            }
        }

        private void GenerateModule(DropManager dropManager, ShipModelSlotType slotType) {
            ModuleDropper moduleDropper = null;
            ShipModule prevModule = null;
            var CB = resource.ModuleTemplates.RandomModule((Workshop)mCharacter.workshop, slotType);
            var CBParams = new ModuleDropper.ModuleDropParams(
                resource,
                CB.Id,
                mCharacter.level,
                mDifficulty,
                new Dictionary<string, int>(),
                ObjectColor.white,
                string.Empty
                );
            moduleDropper = dropManager.GetModuleDropper(CBParams);
            shipModel.SetModule(moduleDropper.Drop() as ShipModule, out prevModule);
        }

    }

    
}

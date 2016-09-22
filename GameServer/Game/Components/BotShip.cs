using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game.Drop;
using Space.Game.Ship;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(BaseWeapon))]
    [REQUIRE_COMPONENT(typeof(CharacterObject))]
    [REQUIRE_COMPONENT(typeof(MovableObject))]
    public class BotShip : BaseShip {

        private BaseWeapon mWeapon;
        private CharacterObject mCharacter;
        private PlayerBonuses mBonuses;


        protected Difficulty mDifficulty = Difficulty.none;

        private bool modelExist = false;
        protected bool initialized = false;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["exists_model?"] = modelExist.ToString();
            hash["initialized?"] = initialized.ToString();
            hash["difficulty"] = mDifficulty.ToString();
            hash["common_resistance"] = commonResist.ToString();
            hash["laser_resistance"] = laserResist.ToString();
            hash["acid_resistance"] = acidResist.ToString();
            hash["rocket_resistance"] = rocketResist.ToString();
            hash["capacity"] = holdCapacity.ToString();
            return hash;
        }



        public virtual void Init(BotShipComponentData data) {
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

        protected void Initialize() {
            if(!initialized) {
                initialized = true;
                mCharacter = RequireComponent<CharacterObject>();
                mWeapon = RequireComponent<BaseWeapon>();
                mBonuses = GetComponent<PlayerBonuses>();

                SetModel(new ShipModel(resource));
                
            }
        }

        public override float  commonResist {
            get {
                //check if resist blocked
                if(blockResist.blocked) {
                    return 0f;
                }
                if(shipModel != null ) {
                    float val = shipModel.commonResist;
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

        public override float acidResist {
            get {
                if(shipModel == null ) {
                    return 0.0f;
                }
                return shipModel.acidResist;
            }
        }

        public override float laserResist {
            get {
                if(shipModel == null ) {
                    return 0.0f;
                }
                return shipModel.laserResist;
            }
        }

        public override float rocketResist {
            get {
                if(shipModel == null ) {
                    return 0.0f;
                }
                return shipModel.rocketResist;
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

        protected virtual void GenerateModule(DropManager dropManager, ShipModelSlotType slotType) {
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

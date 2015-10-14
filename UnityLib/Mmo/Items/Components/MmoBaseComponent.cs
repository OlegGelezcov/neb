namespace Nebula.Mmo.Items.Components {
    using Common;

    public abstract class MmoBaseComponent {

        public static MmoBaseComponent CreateNew(ComponentID id) {
            switch (id) {
                case ComponentID.Activator: return new MmoActivatorComponent();
                case ComponentID.Asteroid: return new MmoAsteroidComponent();
                case ComponentID.Bonuses: return new MmoBonusesComponent();
                case ComponentID.Bot: return new MmoBotComponent();
                case ComponentID.Character: return new MmoCharacterComponent();
                case ComponentID.Chest: return new MmoChestComponent();
                case ComponentID.CombatAI: return new MmoCombatAIComponent();
                case ComponentID.Damagable: return new MmoDamagableComponent();
                case ComponentID.Energy: return new MmoEnergyComponent();
                case ComponentID.Event: return new MmoEventComponent();
                case ComponentID.EventedObject: return new MmoEventedObjectComponent();
                case ComponentID.Model: return new MmoModelComponent();
                case ComponentID.PirateStation: return new MmoPirateStationComponent();
                case ComponentID.Planet: return new MmoPlanetComponent();
                case ComponentID.Player: return new MmoPlayerComponent();
                case ComponentID.PlayerAI: return new MmoPlayerAIComponent();
                case ComponentID.Raceable: return new MmoRaceableComponent();
                case ComponentID.Ship: return new MmoShipComponent();
                case ComponentID.Skills: return new MmoSkillsComponent();
                case ComponentID.Target: return new MmoTargetComponent();
                case ComponentID.Weapon: return new MmoWeaponComponent();
                case ComponentID.Movable: return new MmoMovableComponent();
                case ComponentID.Outpost: return new MmoOutpostComponent();
                case ComponentID.Respawnable: return new MmoRespawnableComponent();
                case ComponentID.SharedChest: return new MmoSharedChestComponent();
                case ComponentID.Teleport: return new MmoTeleportComponent();
                case ComponentID.SubZone: return new MmoSubZoneComponent();
                case ComponentID.Station: return new MmoStationComponent();
                case ComponentID.MainOutpost: return new MmoMainOutpostComponent();
                case ComponentID.PassiveBonuses: return new MmoPassiveBonusesComponent();
                case ComponentID.MiningStation: return new MmoMiningStationComponent();

                default: return null;
            }
        }

        protected IMmoComponentContainer item { get; private set; }

        public void SetItem(IMmoComponentContainer item) {
            this.item = item;
        }

        public void Update() {
            if (item != null) {
                item.UpdateMmoComponent(this);
            }
        }

        public abstract ComponentID componentID { get; }
    }
}

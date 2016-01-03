using Common;
using Common.Space.Game.Resources;
using Nebula.Pets;
using Nebula.Resources;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using Space.Game.Ship;
using System.Collections.Generic;

namespace Space.Game {
    public interface IRes {
        WeaponData RandomWeapon(Workshop workshop);

        ColorInfo WeaponColor(ObjectColor color);

        ColorInfo ModuleColor(ObjectColor color);

        WeaponDropSettings WeaponSettings { get; }

        ModuleSettingsRes ModuleSettings { get; }

        ModuleSetRes Sets { get; }

        ModuleInfoStorage ModuleTemplates { get; }

        ServerInputsRes ServerInputs { get; }

        Dictionary<ShipModelSlotType, string> StartModule(Race race, Workshop workshop);

        MiscInventoryItemDataRes MiscItemDataRes {
            get;
        }

        Leveling Leveling { get; }

        SkillRes Skills { get; }

        MaterialRes Materials { get; }

        ColorInfoRes ColorRes { get; }

        //NpcTypeDataRes NpcTypes { get; }

        ZonesRes Zones { get; }

        NpcGroupRes NpcGroups { get; }

        //ResEvents Events { get; }

        AsteroidsRes Asteroids { get; }

        FractionResolver fractionResolver { get; }

        SkillDropping skillDropping { get; }

        string path { get; }

        float GetDifficultyMult(Difficulty d);

        ResSchemeCraftingMaterials CraftingMaterials {
            get;
        }
        ResPassiveBonuses PassiveBonuses { get; }

        ColorListCollection colorLists { get; }

        DropListCollection dropLists { get; }

        PetParameters petParameters { get; }

        PetSkillCollection petSkills { get; }
    }

}

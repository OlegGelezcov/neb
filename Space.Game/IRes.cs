using Common;
using Common.Space.Game.Resources;
using Nebula.Achievments;
using Nebula.Contracts;
using Nebula.Contracts.Inventory;
using Nebula.Inventory.DropList;
using Nebula.Pets;
using Nebula.Quests;
using Nebula.Quests.Dialogs;
using Nebula.Resources;
using Nebula.Resources.NpcSkills;
using Nebula.Resources.PlayerConstructions;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using Space.Game.Ship;
using System.Collections.Generic;

namespace Space.Game {
    public interface IRes : IContractResource, IContractItemCollection, IAchievmentResource {
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
        PetPassiveBonusCollection petPassiveBonuses { get; }
        CraftResourceObjectTable craftObjects { get; }
        ConstructionDataResource playerConstructions { get; }
        PredefinedDropLists predefinedDropLists { get; }
        NpcClassSkillsResource npcSkills { get;  }
        DifficultyRes difficulty { get; }
        Planet2OreMapRes planetOreMap { get; }
        QuestDataResource quests { get; }
        DialogDataResource dialogs { get; }
    }

}

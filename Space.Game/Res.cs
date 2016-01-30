namespace Space.Game {
    using Common;
    using Common.Space.Game.Resources;
    using Nebula;
    using Nebula.Inventory.DropList;
    using Nebula.Pets;
    using Nebula.Resources;
    using Nebula.Resources.NpcSkills;
    using Nebula.Resources.PlayerConstructions;
    using Space.Game.Resources;
    using Space.Game.Resources.Zones;
    using Space.Game.Ship;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;


    public class Res : IRes {

        private ConcurrentDictionary<Difficulty, float> mDifficultMult;

        //private static Res instance;
        private string basePath;

        public string path {
            get {
                return basePath;
            }
        }


        public Res(string basePath) {
            this.basePath = basePath;
            mDifficultMult = new ConcurrentDictionary<Difficulty, float>();
            mDifficultMult.TryAdd(Difficulty.easy, 0.5f);
            mDifficultMult.TryAdd(Difficulty.easy2, 0.7f);
            mDifficultMult.TryAdd(Difficulty.medium, 0.9f);
            mDifficultMult.TryAdd(Difficulty.none, 1);
            mDifficultMult.TryAdd(Difficulty.hard, 1.2f);
            mDifficultMult.TryAdd(Difficulty.boss, 1.5f);
            mDifficultMult.TryAdd(Difficulty.boss2, 2.0f);
        }

        public float GetDifficultyMult(Difficulty d) {
            float val = 1;
            if (mDifficultMult.TryGetValue(d, out val)) {
                return val;
            } else {
                return 1;
            }
        }
        public void Load() {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //this.SkillStorage = LoadSkills();

            skillDropping = new SkillDropping();
            skillDropping.Load(basePath);

            this.WeaponSettings = new WeaponDropSettings();
            if (!this.WeaponSettings.Load(basePath)) {
                throw new Exception("Error of loading weapon settings....");
            }



            this.ModuleSettings = new ModuleSettingsRes();
            if (!this.ModuleSettings.Load(basePath)) {
                throw new Exception("exception: module settings loading error");
            }

            this.Sets = new ModuleSetRes();
            if (!this.Sets.Load(basePath)) {
                throw new Exception("exception: error of loading module sets...");
            }


            this.ColorRes = new ColorInfoRes();
            this.ColorRes.Load(basePath);

            this.ModuleTemplates = new ModuleInfoStorage();
            this.ModuleTemplates.Load(basePath);

            this.ServerInputs = new ServerInputsRes();
            this.ServerInputs.Load(basePath, "Data/server_inputs.xml");

            //load materials
            this.Materials = new MaterialRes();
            this.Materials.Load(basePath);

            this.MiscItemDataRes = new MiscInventoryItemDataRes();
            this.MiscItemDataRes.Load(basePath);

            //load asteroids
            this.Asteroids = new AsteroidsRes();
            this.Asteroids.Load(basePath);

            this.Zones = new ZonesRes();
            this.Zones.Load(basePath);

            this.Skills = new SkillRes();
            this.Skills.Load(basePath);

            //this.NpcTypes = new NpcTypeDataRes();
            //if (false == this.NpcTypes.Load(basePath))
            //{
            //    throw new Exception("load npc types error");
            //}

            this.Weapons = new WeaponDataRes();
            this.Weapons.Load(basePath);

            this.Leveling = new Leveling();
            this.Leveling.Load(basePath);

            //this.Events = new ResEvents();
            //this.Events.Load(basePath);

            this.NpcGroups = new NpcGroupRes();
            this.NpcGroups.Load(basePath);
            if (!this.NpcGroups.Loaded) {
                throw new Exception("Npc Groups Resources loading error");
            }

            this.StartModules = new StartPlayerModuleRes();
            this.StartModules.Load(basePath);
            if (!this.StartModules.Loaded) {
                throw new Exception("StartModules loading error");
            }

            fractionResolver = new FractionResolver();
            fractionResolver.Load(basePath);

            CraftingMaterials = new ResSchemeCraftingMaterials();
            CraftingMaterials.Load(basePath);

            PassiveBonuses = new ResPassiveBonuses();
            PassiveBonuses.Load(basePath);

            colorLists = new ColorListCollection();
            colorLists.Load(Path.Combine(basePath, "Data/Drop/color_lists.xml"));

            dropLists = new DropListCollection();
            dropLists.Load(Path.Combine(basePath, "Data/Drop/drop_lists.xml"));

            petParameters = new PetParameters();
            petParameters.Load(Path.Combine(basePath, "Data/pets.xml"));

            petSkills = new PetSkillCollection();
            petSkills.Load(Path.Combine(basePath, "Data/pet_active_skills.xml"));

            petPassiveBonuses = new PetPassiveBonusCollection();
            (petPassiveBonuses as PetPassiveBonusCollection).Load(Path.Combine(basePath, "Data/passive_pet_bonuses.xml"));

            craftObjects = new CraftResourceObjectTable();
            craftObjects.Load(Path.Combine(basePath, "Data/Materials/craft_resource.xml"));

            playerConstructions = new ConstructionDataResource();
            playerConstructions.Load(Path.Combine(basePath, "Data/player_constructions.xml"));

            predefinedDropLists = new PredefinedDropLists();
            predefinedDropLists.Load(Path.Combine(basePath, "Data/Drop/predefined_dls.xml"));

            npcSkills = new NpcClassSkillsResource();
            npcSkills.Load(Path.Combine(basePath, "Data/npc_skill_table.xml"));
        }

        public NpcClassSkillsResource npcSkills {
            get;
            private set;
        }

        public PredefinedDropLists predefinedDropLists {
            get;
            private set;
        }
        public ConstructionDataResource playerConstructions {
            get;
            private set;
        }
        public CraftResourceObjectTable craftObjects {
            get;
            private set;
        }

        public PetPassiveBonusCollection petPassiveBonuses {
            get;
            private set;
        }

        public PetSkillCollection petSkills {
            get;
            private set;
        }

        public PetParameters petParameters {
            get;
            private set;
        }

        public DropListCollection dropLists {
            get;
            private set;
        }

        public ColorListCollection colorLists {
            get;
            private set;
        }

        public ResPassiveBonuses PassiveBonuses { get; private set; }

        public ResSchemeCraftingMaterials CraftingMaterials {
            get;
            private set;
        }

        public SkillDropping skillDropping {
            get;
            private set;
        }

        public FractionResolver fractionResolver {
            get;
            private set;
        }

        public ModuleSettingsRes ModuleSettings {
            get;
            private set;
        }

        public NpcGroupRes NpcGroups {
            get;
            private set;
        }

        public WeaponDataRes Weapons {
            get;
            private set;
        }

        public ServerInputsRes ServerInputs {
            get;
            private set;
        }

        //public SkillStorage SkillStorage {
        //    get;
        //    private set;
        //}

        public WeaponDropSettings WeaponSettings {
            get;
            private set;
        }

        public ModuleSetRes Sets {
            get;
            private set;
        }

        public ColorInfoRes ColorRes {
            get;
            private set;
        }

        public ModuleInfoStorage ModuleTemplates {
            get;
            private set;
        }

        public MaterialRes Materials {
            get;
            private set;
        }

        public MiscInventoryItemDataRes MiscItemDataRes {
            get;
            private set;
        }

        public AsteroidsRes Asteroids {
            get;
            private set;
        }



        public ZonesRes Zones {
            get;
            private set;
        }

        public SkillRes Skills {
            get;
            private set;
        }

        //public NpcTypeDataRes NpcTypes
        //{
        //    get;
        //    private set;
        //}

        public Leveling Leveling {
            get;
            private set;
        }

        //public ResEvents Events
        //{
        //    get;
        //    private set;
        //}

        public StartPlayerModuleRes StartModules {
            get;
            private set;
        }

        public WeaponData RandomWeapon(Workshop workshop) {
            return this.Weapons.RandomWeapon(workshop);
        }

        public ColorInfo WeaponColor(ObjectColor color) {
            return this.ColorRes.Color(ColoredObjectType.Weapon, color);
        }

        public ColorInfo ModuleColor(ObjectColor color) {
            return this.ColorRes.Color(ColoredObjectType.Module, color);
        }

        #region Loading functions
        //private SkillStorage LoadSkills() {
        //    string path = Path.Combine(basePath, "Data/Drop/skills.xml");
        //    XDocument document = XDocument.Load(path);
        //    var skills = document.Element("skills").Elements("skill").Select(e => {
        //        return new SkillInfo { id = e.Attribute("id").Value, type = CommonUtils.GetEnum<ShipModelSlotType>(e.Attribute("type").Value) };
        //    }).ToList();
        //    return new SkillStorage(skills);
        //}

        #endregion


        public Dictionary<ShipModelSlotType, string> StartModule(Race race, Workshop workshop) {
            if (!this.StartModules.Loaded) {
                return null;
            }
            return this.StartModules.StartModuleFor(race, workshop);
        }

        private static INebulaLogger _logger = new EmptyNebulaLogger();

        public static INebulaLogger logger {
            get {
                if (_logger == null) { _logger = new EmptyNebulaLogger(); }
                return _logger;
            }
        }

        public static void SetLogger(INebulaLogger log) {
            _logger = log;
        }
    }
}

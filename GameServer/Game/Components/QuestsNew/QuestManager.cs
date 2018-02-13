using Common;
using Nebula.Engine;
using Nebula.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ExitGames.Logging;
using ServerClientCommon;
using System.Collections.Concurrent;
using Space.Game.Ship;
using Space.Game.Objects;
using Nebula.Database;
using Space.Game;
using Space.Game.Inventory.Objects;
using Space.Game.Inventory;
using Space.Game.Drop;
using Space.Game.Resources;

namespace Nebula.Game.Components.Quests {
    public class QuestManager : NebulaBehaviour, IInfo, IQuestConditionContext {

        private const string LOG_TAG = "QM";
        private const float QUEST_START_INTERVAL = 20.0f;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<string> completedQuests = new List<string>();
        private readonly List<QuestInfo> startedQuests = new List<QuestInfo>();



        private readonly List<QuestInfo> candidatesToComplete = new List<QuestInfo>();
        

        private readonly ConcurrentDictionary<string, object> questVariables = new ConcurrentDictionary<string, object>();

        private MmoMessageComponent mmoMessageComponent = null;
        private RaceableObject raceableObject = null;
        private PlayerCharacterObject characterObject = null;



        private float questStartTimer = 0.0f;

        public bool IsLoaded { get; private set; } = false;

        public override void Awake() {
            base.Awake();
            logger.Info($"{LOG_TAG}: Awake()");
        }

        public override void Start() {
            base.Start();
            logger.Info($"{LOG_TAG}: Start()");
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            //logger.Info($"{LOG_TAG}: Update()");

            questStartTimer += deltaTime;
            if(questStartTimer >= QUEST_START_INTERVAL ) {
                questStartTimer -= QUEST_START_INTERVAL;
                if(TryStartQuest()) {
                    logger.Info($"{LOG_TAG}: some quest started");
                }
            }
        }


        public override int behaviourId => (int)ComponentID.QuestManager;


        public bool TryStartQuest() {
            if(startedQuests.Count == 0 ) {
                foreach(QuestData questData in resource.Quests.Quests) {
                    if (!IsCompleted(questData.Id)) {
                        if (questData.StartConditions.Check(this)) {
                            if (MmoMessage != null) {
                                QuestInfo quest = new QuestInfo(this, questData);
                                quest.SetState(QuestState.not_accepted);
                                startedQuests.Add(quest);
                                Hashtable hash = new Hashtable {
                                    { (int)SPC.NewQuest, quest.GetInfo() },
                                    { (int)SPC.Info, GetInfo() }
                                };
                                MmoMessage.ReceiveNewQuests(CustomEventCode.NewQuestNotAccepted, hash);
                                logger.Info($"{LOG_TAG}: New quest not accepted {quest.Id}, Counter => {quest.Counter}");
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool TryCompleteQuest(bool sendUpdate) {
            if(startedQuests.Count > 0 ) {
                candidatesToComplete.Clear();

                bool wasUpdate = false;

                foreach(QuestInfo questInfo in startedQuests) {

                    if (questInfo.State == QuestState.started) {
                        QuestData questData = questInfo.Data;
                        if (questData != null) {
                            QuestConditionCollection completeConditions = null;
                            if (questData.CompleteConditions.TryGetValue(PlayerRace, out completeConditions)) {

                                if (completeConditions != null) {

                                    if (completeConditions.Check(this)) {

                                        questInfo.AddCounter(1);

                                        if (questInfo.Counter >= completeConditions.Repeat) {
                                            candidatesToComplete.Add(questInfo);
                                            
                                        } else {
                                            wasUpdate = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                CraftedModule = null;
                KilledNpc = null;
                CreatedStructure = null;

                if(wasUpdate || sendUpdate) {
                    SendQuestsUpdate();
                }

                if(candidatesToComplete.Count > 0) {

                    foreach(QuestInfo candidateQuest in candidatesToComplete ) {
                        QuestConditionCollection completeConditions = null;
                        if (candidateQuest.Data.TryGetCompleteConditions(PlayerRace, out completeConditions)) {
                            foreach(QuestCondition condition in completeConditions.Conditions) {
                                if(condition.IsClearVariable) {
                                    condition.ResetVariable(this);
                                }
                            }
                        }

                        candidateQuest.SetState(QuestState.ready);
                        Hashtable hash = new Hashtable {
                            { (int)SPC.NewQuest, candidateQuest.GetInfo() },
                            { (int)SPC.Info, GetInfo() }
                        };

                        MmoMessage.ReceiveNewQuests(CustomEventCode.NewQuestReady, hash);
                        logger.Info($"{LOG_TAG}: Quest Ready => {candidateQuest.Id}, counter => {candidateQuest.Counter}");
                    }

                    return true;
                }
            }

            return false;

        }

        public void AcceptQuest(string id) {
            var quest = startedQuests.Find(q => q.Id == id && q.State == QuestState.not_accepted);
            if(quest != null ) {
                quest.SetState(QuestState.started);
                Hashtable hash = new Hashtable {
                    {(int)SPC.NewQuest, quest.GetInfo() },
                    {(int)SPC.Info, GetInfo() }
                };
                MmoMessage.ReceiveNewQuests(CustomEventCode.NewQuestStarted, hash);
                logger.Info($"{LOG_TAG}: quest started => {quest.Id}, counter => {quest.Counter}");
            }
        }

        public void RewardQuest(string id) {

            var quest = startedQuests.Find(q => q.Id == id && q.State == QuestState.ready);
            if(quest != null ) {
                startedQuests.Remove(quest);
                completedQuests.Add(quest.Id);
                foreach(var reward in quest.Data.Rewards.Rewards) {
                    RewardImpl(reward);
                }
                Hashtable hash = new Hashtable {
                    {(int)SPC.NewQuest, quest.GetInfo() },
                    {(int)SPC.Info, GetInfo() }
                };
                MmoMessage.ReceiveNewQuests(CustomEventCode.NewQuestCompleted, hash);
                logger.Info($"{LOG_TAG}: quest rewarded => {quest.Id}, counter => {quest.Counter}");
            }
            /*
            var quest = questsToReward.Find(q => q.Id == id);
            if(quest != null ) {
                questsToReward.Remove(quest);
                //Make Reward
                foreach(var reward in quest.Data.Rewards.Rewards) {
                    RewardImpl(reward);
                }
            }*/
        }



        private void RewardImpl(QuestReward reward) {
            switch(reward.Type) {
                case QuestRewardType.credits: {
                        GetComponent<MmoActor>()?.ActionExecutor?._AddCredits(reward.Count);
                    }
                    break;
                case QuestRewardType.exp: {
                        GetComponent<PlayerCharacterObject>()?.AddExp(reward.Count);
                    }
                    break;
                case QuestRewardType.nebula_credits: {
                        GetComponent<MmoActor>()?.ActionExecutor?.AddNebulaCredits(reward.Count);
                    }
                    break;
                case QuestRewardType.item: {
                        InventoryItemQuestReward itemQuestReward = reward as InventoryItemQuestReward;
                        if(itemQuestReward != null ) {
                            RewardItemImpl(itemQuestReward);
                        }
                    }
                    break;
            }
        }

        private void RewardItemImpl(InventoryItemQuestReward reward) {
            ServerInventoryItem targetItem = null;

            switch (reward.ObjectType) {
                case InventoryObjectType.Material: {
                        MaterialItemQuestReward materialReward = reward as MaterialItemQuestReward;
                        if(materialReward != null ) {
                            MaterialObject material = new MaterialObject(materialReward.OreId);
                            targetItem = new ServerInventoryItem(material, materialReward.Count);
                        }
                    }
                    break;
                case InventoryObjectType.Scheme: {
                        SchemeItemQuestReward schemeReward = reward as SchemeItemQuestReward;
                        if(schemeReward != null ) {
                            SchemeObject scheme = new SchemeObject(new SchemeObject.SchemeInitData(
                                id: Guid.NewGuid().ToString(),
                                name: string.Empty,
                                level: PlayerLevel,
                                workshop: PlayerWorkshop,
                                templateModuleId: resource.ModuleTemplates.Module(PlayerWorkshop, schemeReward.Slot).Id,
                                color: schemeReward.Color,
                                craftingMaterials: new Dictionary<string, int>(),
                                inSetID: string.Empty
                                ));
                            targetItem = new ServerInventoryItem(scheme, schemeReward.Count);
                        }
                    }
                    break;
                case InventoryObjectType.Weapon: {
                        WeaponItemQuestReward weaponReward = reward as WeaponItemQuestReward;
                        if (weaponReward != null) {
                            WeaponDropper.WeaponDropParams weaponDropParams = new WeaponDropper.WeaponDropParams(
                                resource: resource,
                                level: PlayerLevel,
                                workshop: PlayerWorkshop,
                                damageType: WeaponDamageType.damage,
                                difficulty: Difficulty.none
                                );
                            ColorInfo colorInfo = resource.ColorRes.Color(ColoredObjectType.Weapon, weaponReward.Color);
                            DropManager dropManager = DropManager.Get(resource);
                            WeaponDropper weaponDropper = dropManager.GetWeaponDropper(dropParams: weaponDropParams);
                            WeaponObject weapon = weaponDropper.DropWeapon(colorInfo);
                            targetItem = new ServerInventoryItem(weapon, weaponReward.Count);
                        }
                    }
                    break;
            }

            if(targetItem != null ) {
                GetComponent<MmoActor>()?.AddToStationInventory(item: targetItem, sendUpdateEvent: true);
            }
        }

        public void RestartQuests() {
            completedQuests.Clear();
            startedQuests.Clear();
            SendQuestsUpdate();
        }

        public void ForceStartQuest(string questId ) {
            if(completedQuests.Contains(questId)) {
                completedQuests.Remove(questId);
            }

            QuestData questData = resource.Quests.GetQuest(questId);
            if(questData != null ) {
                QuestInfo quest = new QuestInfo(this, questData);
                startedQuests.Clear();
                startedQuests.Add(quest);
                Hashtable hash = new Hashtable {
                    { (int)SPC.NewQuest, quest.GetInfo() },
                    { (int)SPC.Info, GetInfo() }
                };
                MmoMessage.ReceiveNewQuests(CustomEventCode.NewQuestStarted, hash);
            }
        }

        public void SendQuestsUpdate() {
            Hashtable hash = new Hashtable {
                        { (int)SPC.Info, GetInfo() }
            };
            MmoMessage.ReceiveNewQuests(CustomEventCode.NewQuestUpdated, hash);
            logger.Info($"{LOG_TAG}: send quest update...");
        }

        private MmoMessageComponent MmoMessage {
            get {
                if(mmoMessageComponent == null ) {
                    mmoMessageComponent = GetComponent<MmoMessageComponent>();
                }
                return mmoMessageComponent;
            }
        }

        private RaceableObject RaceableObject {
            get {
                if (raceableObject == null) {
                    raceableObject = GetComponent<RaceableObject>();
                }
                return raceableObject;
            }
        }

        private PlayerCharacterObject CharacterObject {
            get {
                if(characterObject == null ) {
                    characterObject = GetComponent<PlayerCharacterObject>();
                }
                return characterObject;
            }
        }

        public bool IsCompleted(string questId ) {
            return completedQuests.Contains(questId);
        }



        private Race PlayerRace {
            get {
                if(RaceableObject != null ) {
                    return (Race)RaceableObject.race;
                }
                return Race.None;
            }
        }

        private Workshop PlayerWorkshop {
            get {
                if(CharacterObject != null ) {
                    return (Workshop)CharacterObject.workshop;
                }
                return Workshop.Arlen;
            }
        }

        #region IQuestConditionContext
        public IKilledNpc KilledNpc {
            get;
            private set;
        }

        public IKilledPlayer KilledPlayer {
            get;
            private set;
        }

        public int PlayerLevel {
            get {
                if(CharacterObject != null ) {
                    return CharacterObject.level;
                }
                return 1;
            }
        }


        public string CapturedSystem { get; private set; } = string.Empty;


        public T GetVariable<T>(string name, T defaultValue = default(T)) {
            object val;
            if (questVariables.TryGetValue(name, out val)) {
                try {
                    return (T)val;
                } catch (System.Exception e) {
                    logger.Info($"{LOG_TAG}: error of receiving quest variable {name} of type {typeof(T).Name}, exception {e.Message}");
                    return defaultValue;
                }
            } else {
                return defaultValue;
            }
        }

        public void AddIntVariable(string name, int count) {
            questVariables.AddOrUpdate(name, nm => count, (nm, old) => (int)old + count);
        }

        //public bool IsCreatedStructure(QuestStructureType type) {
        //    return false;
        //}

        public bool IsQuestCompleted(string questId) {
            return IsCompleted(questId);
        }

        public void ResetVariable<T>(string name) {
            if(questVariables.ContainsKey(name)) {
                object oldVal;
                if(questVariables.TryRemove(name, out oldVal)) {
                    
                }
            }
        } 

        public ICraftedModule CraftedModule { get; private set; }

        public ICreatedStructure CreatedStructure { get; private set; }

        #endregion

        private void SetVariable<T>(string name, T obj) {
            questVariables.AddOrUpdate(name, obj, (key, old) => obj);
            logger.Info($"{LOG_TAG}: set variable {name} to value {GetVariable<T>(name)}");
        }

        private Hashtable GetVariablesHash() {
            Hashtable hash = new Hashtable();
            foreach(var questVariable in questVariables) {
                hash.Add(questVariable.Key, questVariable.Value);
            }
            return hash;
        }

        #region IInfo
        public Hashtable GetInfo() {
            Hashtable hash = new Hashtable();
            hash.Add((int)SPC.CompletedQuests, completedQuests.ToArray());
            hash.Add((int)SPC.StartedQuests, startedQuests.Select(q => q.GetInfo()).ToArray());
            hash.Add((int)SPC.Variables, GetVariablesHash());
            return hash;
        }

        public void ParseInfo(Hashtable info) {
            completedQuests.Clear();
            if(info.ContainsKey((int)SPC.CompletedQuests)) {
                string[] completedArr = info[(int)SPC.CompletedQuests] as string[];
                if(completedArr != null) {
                    completedQuests.AddRange(completedArr);
                }
            }

            startedQuests.Clear();
            if(info.ContainsKey((int)SPC.StartedQuests)) {
                Hashtable[] startedArr = info[(int)SPC.StartedQuests] as Hashtable[];
                if(startedArr != null ) {
                    foreach(Hashtable hash in startedArr) {
                        QuestInfo quest = new QuestInfo(this);
                        quest.ParseInfo(hash);
                        if(quest.IsValid && quest.Data != null ) {
                            startedQuests.Add(quest);
                        }
                    }
                }
            }

            questVariables.Clear();
            if(info.ContainsKey((int)SPC.Variables)) {
                Hashtable varHash = info[(int)SPC.Variables] as Hashtable;
                if(varHash != null ) {
                    foreach(DictionaryEntry entry in varHash) {
                        questVariables.TryAdd(entry.Key.ToString(), entry.Value);
                    }
                }
            }
        }
        #endregion


        public QuestSave GetSave() {
            List<Hashtable> startedQuestsSave = new List<Hashtable>();
            foreach(QuestInfo startedQuest in startedQuests) {
                startedQuestsSave.Add(startedQuest.GetInfo());
            }
            List<string> completedQuestSave = new List<string>();
            foreach(string cQuest in completedQuests) {
                completedQuestSave.Add(cQuest);
            }
            return new QuestSave(completedQuestSave, startedQuestsSave, GetVariablesHash());
        }

        public void Load() {
            if (!IsLoaded) {
                bool isNewSave = false;
                PlayerCharacterObject playerCharacter = GetComponent<PlayerCharacterObject>();
                GameApplication application = nebulaObject.mmoWorld().application;
                QuestSave questSave = QuestDatabase.instance(application).LoadQuests(playerCharacter.characterId, out isNewSave);

                completedQuests.Clear();
                foreach (string cQuest in questSave.CompletedQuests) {
                    completedQuests.Add(cQuest);
                }

                startedQuests.Clear();
                foreach (Hashtable sQuest in questSave.StartedQuests) {
                    QuestInfo questInfo = new QuestInfo(this);
                    questInfo.ParseInfo(sQuest);
                    if (questInfo.IsValid) {
                        startedQuests.Add(questInfo);
                    }
                }

                questVariables.Clear();
                foreach(DictionaryEntry entry in questSave.QuestVariables) {
                    questVariables.TryAdd(entry.Key.ToString(), entry.Value);
                }

                IsLoaded = true;
            }
        }
        #region ComponentMessages
        public void OnEnemyDeath(NebulaObject enemy) {
            logger.Info($"{LOG_TAG}: enemy was killed {enemy.getItemType()}=>{enemy.Id}");

            if(enemy.getItemType() == ItemType.Bot) {
                var botComponent = enemy.GetComponent<BotObject>();
                if(botComponent?.getSubType() == BotItemSubType.StandardCombatNpc ) {
                    var enemyCharacterObject = enemy.GetComponent<CharacterObject>();
                    if(enemyCharacterObject != null ) {
                        Workshop eClass = (Workshop)enemyCharacterObject.workshop;
                        int eLevel = enemyCharacterObject.level;
                        var shipComponent = enemy.GetComponent<BotShip>();
                        if(shipComponent != null ) {
                            ObjectColor eColor = NebulaEnumUtils.GetColorForDifficulty(shipComponent.difficulty);
                            KilledNpc = new QuestKilledNpc(eLevel, eClass, eColor, botComponent.botGroup);
                            TryCompleteQuest(sendUpdate: true);
                        }
                    }
                }
            }
        }

        public void OnModuleCrafted(ShipModule module) {
            logger.Info($"{LOG_TAG}: module crafted {module.SlotType}");

            CraftedModule = new CraftedModule(module.SlotType, module.Level, module.Color);

            if(startedQuests.Count > 0 ) {
                bool needTryComplete = false;
                foreach(var quest in startedQuests) {
                    QuestConditionCollection conditions = null;
                    if(quest.Data.TryGetCompleteConditions(PlayerRace, out conditions)) {
                        if(conditions.HasCondition<ModuleCraftedQuestCondition>()) {
                            needTryComplete = true;
                        }
                    }
                }

                if(needTryComplete) {
                    TryCompleteQuest(sendUpdate:true);
                }
            }
        }

        public void OnAsteroidCollected(List<AsteroidContent> content ) {
            bool needTryComplete = false;
            foreach(var startedQuest in startedQuests) {
                QuestConditionCollection collection;
                if(startedQuest.Data.TryGetCompleteConditions(PlayerRace, out collection)) {
                    if(collection.HasCondition<CollectOreQuestCondition>()) {
                        var targetCondition = collection.GetCondition<CollectOreQuestCondition>();
                        foreach(var ore in content) {
                            if(ore.Material.Id == targetCondition.OreId ) {
                                AddIntVariable(targetCondition.OreId, ore.Count);
                                needTryComplete = true;
                            }
                        }
                    }
                }
            }
            if(needTryComplete) {
                TryCompleteQuest(sendUpdate:true);
            }
        }

        public void OnStructureCreated(QuestStructureType type) {
            bool needTryComplete = false;
            foreach(QuestInfo startedQuest in startedQuests ) {
                CreateStructureQuestCondition condition = null;
                if(startedQuest.Data.TryGetCompleteCondition<CreateStructureQuestCondition>(PlayerRace, out condition)) {

                    CreatedStructure = new CreatedStructure(type);
                    needTryComplete = true;
                }
            }
            if(needTryComplete) {
                TryCompleteQuest(sendUpdate: true);
            }
            logger.Info($"{LOG_TAG}: created structure {type}");
        }
        #endregion
    }

    public class CreatedStructure : ICreatedStructure {

        public QuestStructureType Type { get; private set; }

        public CreatedStructure(QuestStructureType type) {
            Type = type;
        }
    }

    public class CraftedModule : ICraftedModule {

        public ShipModelSlotType Slot { get; private set; }

        public int Level { get; private set; }

        public ObjectColor Color { get; private set; }

        public CraftedModule(ShipModelSlotType slot, int level, ObjectColor color ) {
            Slot = slot;
            Level = level;
            Color = color;
        }
    }

    public class QuestInfo : IInfo {

        public string Id { get; private set; }
        public int Counter { get; private set; }
        public QuestState State { get; private set; }

        private QuestData data = null;
        private NebulaBehaviour context = null;

        public void AddCounter(int cnt) {
            Counter += cnt;
        }

        public QuestInfo(NebulaBehaviour context) {
            this.context = context;
        }

        public QuestInfo(NebulaBehaviour context, QuestData questData ) {
            this.context = context;
            this.Id = questData.Id;
            this.data = questData;
            this.Counter = 0;
            this.State = QuestState.not_accepted;
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, Id },
                { (int)SPC.Counter, Counter },
                { (int)SPC.State, (int)(byte)State }
            };
        }

        public void ParseInfo(Hashtable info) {
            if(info.ContainsKey((int)SPC.Id)) {
                Id = (string)info[(int)SPC.Id];
            }
            if(info.ContainsKey((int)SPC.Counter)) {
                Counter = (int)info[(int)SPC.Counter];
            }
            if(info.Contains((int)SPC.State)) {
                State = (QuestState)(byte)(int)info[(int)SPC.State];
            }
        }

        public QuestData Data {
            get {
                if(data == null ) {
                    data = context.resource.Quests.GetQuest(Id);
                }
                return data;
            }
        }

        public void SetState(QuestState state) {
            QuestState oldState = State;
            State = state;

            if(State == QuestState.started && oldState != QuestState.started) {
                Counter = 0;
            }
            if(State == QuestState.not_accepted ) {
                Counter = 0;
            }
        }

        public bool IsValid => (!string.IsNullOrEmpty(Id));
    }


    public class QuestKilledNpc : IKilledNpc {

        public QuestKilledNpc(int level, Workshop workshop, ObjectColor color, string botGroup) {
            Level = level;
            Class = workshop;
            Color = color;
            BotGroup = botGroup;
            if(BotGroup == null  ) {
                BotGroup = string.Empty;
            }
        }

        public int Level { get; }

        public Workshop Class { get; }

        public ObjectColor Color { get; }

        public string BotGroup { get; } = string.Empty;
    }

    public class QuestSave {
        public List<string> CompletedQuests { get; private set; }
        public List<Hashtable> StartedQuests { get; private set; }
        public Hashtable QuestVariables { get; private set; }

        public QuestSave(List<string> completedQuests, List<Hashtable> startedQuests, Hashtable questVariables) {
            CompletedQuests = completedQuests;
            StartedQuests = startedQuests;
            QuestVariables = questVariables;
        }
    }
}

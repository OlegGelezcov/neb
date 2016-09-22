using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;
using Nebula.Engine;
using Nebula.Quests;
using Nebula.Game.Components.Quests.Dialogs;
using ExitGames.Logging;
using Space.Game;
using Nebula.Database;
using Nebula.Quests.Drop;
using Nebula.Game.Events;
using Space.Game.Inventory;
using Nebula.Game.Components.Activators;
using GameMath;
using Nebula.Quests.Actions;

namespace Nebula.Game.Components.Quests {
    public class QuestManager : NebulaBehaviour, IInfo, IQuestConditionTarget {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private readonly List<string> m_CompletedQuests = new List<string>();

        private readonly ConcurrentDictionary<string, ServerQuest> m_ActiveQuests = new ConcurrentDictionary<string, ServerQuest>();
        //private readonly List<string> m_ClearedQuests = new List<string>();

        private readonly List<string> m_SpecialNPCBadges = new List<string> { "tiran" };

        private RaceableObject m_RaceComponent;
        private DialogManager m_DialogComponent;
        private MmoMessageComponent m_MmoComponent;
        private MmoActor m_Player;
        private bool m_StartCalled = false;

        public override void Start() {
            if (!m_StartCalled) {
                m_StartCalled = true;
                base.Start();
                m_RaceComponent = GetComponent<RaceableObject>();
                m_DialogComponent = GetComponent<DialogManager>();
                m_MmoComponent = GetComponent<MmoMessageComponent>();
                m_Player = GetComponent<MmoActor>();
            }
        }

        public void Reset() {
            m_CompletedQuests.Clear();
            m_ActiveQuests.Clear();
            SendInfo();
        }

        public void Load() {
            Start();
            bool isNew = false;
            var character = GetComponent<PlayerCharacterObject>();
            var app = nebulaObject.mmoWorld().application;
            Hashtable questHash = QuestDatabase.instance(app).LoadQuests(character.characterId, resource as Res, out isNew);
            if(questHash != null ) {
                ParseInfo(questHash);
            }
        }

        public Hashtable GetInfo() {
            Hashtable activeQuestHash = new Hashtable();
            foreach(var kvp in m_ActiveQuests) {
                activeQuestHash.Add(kvp.Key, kvp.Value.GetInfo());
            }
            return new Hashtable {
                { (int)SPC.CompletedQuests, m_CompletedQuests.ToArray() },
                { (int)SPC.ActiveQuests, activeQuestHash }
            };
        }

        public void ParseInfo(Hashtable hash) {
            string[] completedQuests = hash.GetValue<string[]>((int)SPC.CompletedQuests, new string[] { });
            m_CompletedQuests.Clear();
            foreach(var completedQuest in completedQuests ) {
                m_CompletedQuests.Add(completedQuest);
            }

            Hashtable activeQuestHash = hash.GetValue<Hashtable>((int)SPC.ActiveQuests, new Hashtable());
            m_ActiveQuests.Clear();
            foreach(DictionaryEntry kvp in activeQuestHash ) {
                Hashtable questHash = kvp.Value as Hashtable;
                if(questHash != null ) {
                    ServerQuest quest = new ServerQuest(questHash);
                    m_ActiveQuests.TryAdd(quest.id, quest);
                }
            }
        }

        /// <summary>
        /// Start any allowed quests if no active quests
        /// </summary>
        private void StartAnyQuests() {
            if(m_ActiveQuests.Count == 0 ) {
                foreach(var qdata in resource.quests.GetQuests(race)) {
                    if(NotCompleted(qdata.Value.id)) {
                        if(TryStartQuest(qdata.Value.id)) {
                            break;
                        }
                    }
                }
            }
        }

        public bool TryStartQuest(string id, object userData = null) {
            if(NotCompleted(id)) {
                var questData = resource.quests.GetQuest(race, id);
                if(questData != null ) {
                    if(CheckConditions(this, null, questData.startConditions, userData)) {
                        ServerQuest quest = new ServerQuest(questData);
                        if(m_ActiveQuests.TryAdd(quest.id, quest)) {
                            nebulaObject.mmoWorld().OnEvent(new QuestStartedEvent(nebulaObject, quest.id));
                            //------------------------------------------------------
                            //here we are send update to client about quest starting
                            ReceiveEvent(CustomEventCode.QuestAccepted, quest.GetInfo());

                            TryCheckActiveQuests(null);

                            return true;
                        } else {
                            //s_Log.InfoFormat("start quest -> {0} error: fail add quest to dict", id);
                        }
                    } else {
                       // s_Log.InfoFormat("start quest -> {0} error: not valid start conditions", id);
                    }
                } else {
                    //s_Log.InfoFormat("start quest -> {0} error: not founded quest data", id);
                }
            } else {
                //s_Log.InfoFormat("start quest -> {0} error: quest already completed", id);
            }
            return false;
        }

        /// <summary>
        /// Restarting quest for debugging purposes
        /// </summary>
        /// <param name="questId"></param>
        public void RestartQuest(string questId, out RPCErrorCode errorCode) {
            errorCode = RPCErrorCode.Ok;
            QuestData questData = resource.quests.GetQuest(race, questId);
            if(questData != null ) {
                if(Completed(questId)) {
                    m_CompletedQuests.Remove(questId);
                }
                if(m_ActiveQuests.ContainsKey(questId)) {
                    ServerQuest oldQuest;
                    m_ActiveQuests.TryRemove(questId, out oldQuest);
                }
                ServerQuest newQuest = new ServerQuest(questData);
                m_ActiveQuests.TryAdd(newQuest.id, newQuest);
                nebulaObject.mmoWorld().OnEvent(new QuestStartedEvent(nebulaObject, newQuest.id));
                SendInfo();
                ReceiveEvent(CustomEventCode.QuestAccepted, newQuest.GetInfo());
                TryCheckActiveQuests(null);
            } else {
                errorCode = RPCErrorCode.QuestDataNotFound;
            }
        }

        
        public bool TryCheckActiveQuests(object userData = null) {
            bool anyCompleted = false;
            foreach(var kvp in m_ActiveQuests ) {
                var data = kvp.Value.GetData(race, resource);
                if(data == null ) {
                    kvp.Value.SetReady();
                    continue;
                }  else {
                    var completeConditions = data.completeConditions;
                    if(CheckConditions(this, kvp.Value, completeConditions, userData)) {
                        kvp.Value.SetReady();
                        anyCompleted = true;
                        //-----------------------------------------
                        //here send quest ready event------------
                        ReceiveEvent(CustomEventCode.QuestReady, kvp.Value.GetInfo());
                        //s_Log.InfoFormat("sended ready for quest: {0}".Orange(), kvp.Value.id);
                    }
                }
            }

            return anyCompleted;
        }

        public bool CompleteReadyQuest(string id) {
            ServerQuest quest = null;
            bool found = false;
            if(m_ActiveQuests.TryGetValue(id, out quest)) {
                if(quest.state == ServerQuestState.ready ) {
                    found = true;
                }
            }

            if(found) {
                ServerQuest removedQuest = null;
                if(m_ActiveQuests.TryRemove(id, out removedQuest)) {
                    if(NotCompleted(id)) {
                        m_CompletedQuests.Add(id);
                    }

                    var questData = removedQuest.GetData(race, resource);
                    if (m_DialogComponent != null && questData != null) {
                        //m_DialogComponent.ExecutePostActions(questData.postActions);
                        ExecutePostActionList(questData.postActions);
                    }
                    //----------------------------------------------------
                    //here send quest completed event
                    ReceiveEvent(CustomEventCode.QuestCompleted, removedQuest.GetInfo());

                    return true;
                }
            }

            return false;
        }

        public void ExecutePostActionList(List<PostAction> postActions) {
            if(m_DialogComponent != null && postActions != null ) {
                m_DialogComponent.ExecutePostActions(postActions);
            }
        }

        private void ReceiveEvent(CustomEventCode code, Hashtable hash) {
            if(m_MmoComponent != null ) {
                switch(code) {
                    case CustomEventCode.Quests: {
                            m_MmoComponent.ReceiveQuests(hash);
                        }
                        break;
                    case CustomEventCode.QuestAccepted: {
                            m_MmoComponent.ReceiveQuestAccepted(hash);
                        }
                        break;
                    case CustomEventCode.QuestReady: {
                            m_MmoComponent.ReceiveQuestReady(hash);
                        }
                        break;
                    case CustomEventCode.QuestCompleted: {
                            m_MmoComponent.ReceiveQuestCompleted(hash);
                        }
                        break;
                }
            }
        }

        public void SendInfo() {
            ReceiveEvent(CustomEventCode.Quests, GetInfo());
        }

        private bool CheckConditions(IQuestConditionTarget target, ServerQuest quest, List<QuestCondition> conditions, object userData = null) {
            bool valid = true;
            foreach(var condition in conditions ) {
                condition.SetQuest(quest);
                if(!condition.CheckCondition(target, userData)) {
                    valid = false;
                    break;
                } 
            }
            return valid;
        }

        public bool SetIntegerVariable(string name, int val) {
            bool setted = false;
            foreach (var kvp in m_ActiveQuests ) {
                if(kvp.Value.SetInteger(name, val)) {
                    setted = true;
                }
            }
            if(setted) {
                TryCheckActiveQuests(null);
            }
            return setted;
        }

       
        public class VariableChangeResult<T> {
            public ServerQuest quest;
            public T currentValue;
        }

        public void IncreaseInteger(string varName, int cnt) {
            var list = IncreaseVariable(varName, cnt);
            if (list != null) {
                foreach (var obj in list) {
                    var condition = obj.quest.GetData(race, resource).GetVariableValueCompleteCondition(varName);
                    if (condition != null) {
                        Hashtable hash = new Hashtable {
                            { (int)SPC.Quest, obj.quest.id },
                            { (int)SPC.ConditionName, condition.name },
                            { (int)SPC.Value, obj.currentValue },
                            { (int)SPC.VariableName, varName }
                        };
                        m_MmoComponent.ReceiveQuestConditionUpdate(hash);

                    }
                }
            }
        }

        private List<VariableChangeResult<int>> IncreaseVariable(string name, int inc ) {
            bool setted = false;
            List<VariableChangeResult<int>> list = null;
            foreach(var kvp in m_ActiveQuests ) {
                var quest = kvp.Value;
                int newVal = 0;
                if(quest.IncreaseInteger(name, inc, out newVal )) {
                    if(list == null) {
                        list = new List<VariableChangeResult<int>>();
                    }
                    list.Add(new VariableChangeResult<int> { quest = quest, currentValue = newVal });
                    setted = true;
                }
            }
            if(setted ) {
                TryCheckActiveQuests();
            }
            return list;
        }

        public void SetFloatVariable(string name, float val) {
            bool setted = false;
            foreach(var kvp in m_ActiveQuests) {
                if(kvp.Value.SetFloat(name, val)) {
                    setted = true;
                }
            }
            if(setted) {
                TryCheckActiveQuests(null);
            }
        }

        public bool SetBoolVariable(string name, bool val) {
            bool setted = false;
            foreach(var kvp in m_ActiveQuests ) {
                if(kvp.Value.SetBool(name, val)) {
                    //s_Log.InfoFormat("setted bool variable: {0}->{1} on quest: {2}".Lime(), name, val, kvp.Value.id);
                    setted = true;
                }
            }
            if(setted) {
                TryCheckActiveQuests(null);
            }
            return setted;
        }

        //Listen to world events
        public void OnEvent(BaseEvent evt ) {
            bool updated = false;
           // s_Log.Info("QuestManager.OnEvent()".Orange());


            //handle quests what depends from adding to inventory
            if(evt.eventType == EventType.InventoryItemsAdded) {
                if (evt.source.Id == nebulaObject.Id) {
                    //s_Log.InfoFormat("QuestManager.OnEvent() with inventory items added".Lightblue());
                    InventoryItemsAddedEvent iiaEvt = evt as InventoryItemsAddedEvent;
                    foreach (var item in iiaEvt.items) {
                        bool someUpdateOccured = CheckCountOfItemsGECondition(item);
                        if (!updated) {
                            updated = someUpdateOccured;
                        }
                    }
                }
            }
           
            //handle quests what depend from using quest item near activator
            if(evt.eventType == EventType.QuestItemUsed ) {
                if(evt.source.Id == nebulaObject.Id ) {
                    TryCheckActiveQuests((evt as QuestItemUsedEvent).itemId);
                }
            }

            updated = (updated || HandleTriggers(evt));

            if(updated) {
                TryCheckActiveQuests();
            }
        }

        private bool HandleTriggers(BaseEvent evt) {
            bool executed = false;
            foreach(var quest in m_ActiveQuests) {
                executed = (executed || quest.Value.ExecuteTriggers(this, evt));
            }
            return executed;
        }

        private bool CheckCountOfItemsGECondition(ServerInventoryItem item) {
            bool updated = false;

            foreach(var kvp in m_ActiveQuests) {
                var quest = kvp.Value;
                var conditions = quest.FilterCompleteConditions(QuestConditionName.COUNT_OF_ITEMS_GE, race, resource);
                //s_Log.InfoFormat("QuestManager.CheckCountOfItemsGECondition() found conditions: {0}".Lightblue(), conditions.Count);
                if(conditions.Count > 0 ) {
                    foreach(var condition in conditions ) {
                        CountOfItemsGECondition coigeCondition = condition as CountOfItemsGECondition;
                        if(item.Object.Id == coigeCondition.itemId && item.Object.Type == coigeCondition.itemType ) {

                            

                            int actualCount = m_Player.Inventory.ItemCount(item.Object.Type, item.Object.Id);
                            //s_Log.InfoFormat("QuestManager.CheckCountOfItemsGECondition() send condition update event to player, actual = {0}, expected = {1}".Lightblue(),
                                //actualCount, coigeCondition.value);

                            Hashtable updateConditionHash = new Hashtable {
                                { (int)SPC.Quest, quest.id },
                                { (int)SPC.ConditionName, coigeCondition.name },
                                { (int)SPC.ItemType, (byte)coigeCondition.itemType },
                                { (int)SPC.ItemId, coigeCondition.itemId },
                                { (int)SPC.ExpectedCount, coigeCondition.value },
                                { (int)SPC.ActualCount, actualCount }
                            };
                            m_MmoComponent.ReceiveQuestConditionUpdate(updateConditionHash);
                            updated = true;
                        }
                    }
                }
            }
            return updated;
        }

        #region IQuestConditionTarget
        public bool isSpace() {
            if(m_Player != null ) {
                return m_Player.atSpace;
            }
            return false;
        }

        public bool isStation() {
            if(m_Player != null ) {
                return m_Player.atStation;
            }
            return false;
        }

        public bool isQuestCompleted(string id) {
            return Completed(id);
        }

        public bool isDialogCompleted(string id) {
            if(m_DialogComponent != null ) {
                return m_DialogComponent.Completed(id);
            }
            return false;
        } 

        public bool isWorld(string worldId ) {
            var world = nebulaObject.mmoWorld();
            if(world != null ) {
                return world.GetID() == worldId;
            }
            return false;
        }
        #endregion

        public override int behaviourId {
            get {
                return (int)ComponentID.Quests;
            }
        }

        private bool NotCompleted(string id) {
            return (false == m_CompletedQuests.Contains(id));
        }

        private bool Completed(string id ) {
            return (false == NotCompleted(id));
        }

        private Race race {
            get {
                if(m_RaceComponent != null ) {
                    return m_RaceComponent.getRace();
                }
                return Race.None;
            }
        }

        public void OnPlayerLevel(int level) {
            SetIntegerVariable("level", level);
        }

        [ComponentMessage(true, Log = "QuestManager->OnEnterStation()#orange")]
        public void OnEnterStation() {
            SetBoolVariable("at_station", true);
            StartAnyQuests();
        }

        [ComponentMessage(true, Log ="QuestManager->OnStationExited()#orange")]
        public void OnStationExited() {
            SetBoolVariable("at_space", true);
            StartAnyQuests();
        }

        [ComponentMessage(true, Log = "QuestManager->OnEnemyDeath()#orange")]
        public void OnEnemyDeath(NebulaObject enemy) {
            SetBoolVariable("kill_enemy", true);


            string varName = "kill_npc_counter";
            if (enemy.badge != null) {
                if (m_SpecialNPCBadges.Contains(enemy.badge)) {
                    varName = enemy.badge;
                }
            }

            IncreaeKillNpcCounter(varName);
        }

        private void IncreaeKillNpcCounter(string varName) {
            var list = IncreaseVariable(varName, 1);
            if (list != null) {
                foreach (var obj in list) {
                    var condition = obj.quest.GetData(race, resource).GetVariableValueCompleteCondition(varName);
                    if (condition != null) {
                        Hashtable hash = new Hashtable {
                            { (int)SPC.Quest, obj.quest.id },
                            { (int)SPC.ConditionName, condition.name },
                            { (int)SPC.Value, obj.currentValue },
                            { (int)SPC.VariableName, varName }
                        };
                        m_MmoComponent.ReceiveQuestConditionUpdate(hash);

                    }
                }
            }
        }



        public int CountOfItems(InventoryObjectType itemType, string itemId) {
            if(m_Player == null ) {
                return 0;
            }
            return m_Player.Inventory.ItemCount(itemType, itemId);
        }

        public bool HasNearActivatorsWithBadge(string badge, bool sendError = true) {
            var activators = nebulaObject.mmoWorld().GetItems(item => {
                var activator = item.GetComponent<ActivatorObject>();
                if (activator && item.badge == badge) {
                    var dist = transform.DistanceTo(item.transform);
                    if (dist < activator.radius + item.size) {
                        return true;
                    }
                }
                return false;
            });
            bool result = (activators.Count > 0);
            if(!result) {
                if(m_MmoComponent) {
                    m_MmoComponent.ReceiveCode(RPCErrorCode.DistanceIsFar);
                }
            }
            return result;
        }

        public bool NearPoint(Vector3 point, float radius) {
            float mag = (transform.position - point).magnitude;

            //s_Log.InfoFormat("Check distance, real = {0}, expected = {1}, my position = {2}, target point = {3}".Purple(), mag, radius, transform.position, point);
            return mag < radius;
        }

        public List<DropInfo> activeDropInfoList {
            get {
                List<DropInfo> list = new List<DropInfo>();
                foreach(var aq in m_ActiveQuests ) {
                    var data = aq.Value.GetData(race, resource);
                    if(data != null ) {
                        if(data.dropInfoList.Count > 0 ) {
                            list.AddRange(data.dropInfoList);
                        }
                    }
                }
                return list;
            }
        }

        public Race GetRace() {
            return race;
        }

        public IRes GetResource() {
            return resource;
        }
    }
}

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

namespace Nebula.Game.Components.Quests {
    public class QuestManager : NebulaBehaviour, IInfo, IQuestConditionTarget {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private readonly List<string> m_CompletedQuests = new List<string>();

        private readonly ConcurrentDictionary<string, ServerQuest> m_ActiveQuests = new ConcurrentDictionary<string, ServerQuest>();
        //private readonly List<string> m_ClearedQuests = new List<string>();


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

        public bool TryStartQuest(string id, object userData = null) {
            if(NotCompleted(id)) {
                var questData = resource.quests.GetQuest(race, id);
                if(questData != null ) {
                    if(CheckConditions(this, null, questData.startConditions, userData)) {
                        ServerQuest quest = new ServerQuest(questData);
                        if(m_ActiveQuests.TryAdd(quest.id, quest)) {
                            //------------------------------------------------------
                            //here we are send update to client about quest starting
                            ReceiveEvent(CustomEventCode.QuestAccepted, quest.GetInfo());
                            return true;
                        } else {
                            s_Log.InfoFormat("start quest -> {0} error: fail add quest to dict", id);
                        }
                    } else {
                        s_Log.InfoFormat("start quest -> {0} error: not valid start conditions", id);
                    }
                } else {
                    s_Log.InfoFormat("start quest -> {0} error: not founded quest data", id);
                }
            } else {
                s_Log.InfoFormat("start quest -> {0} error: quest already completed", id);
            }
            return false;
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
                        s_Log.InfoFormat("sended ready for quest: {0}".Orange(), kvp.Value.id);
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
                        m_DialogComponent.ExecutePostActions(questData.postActions);
                    }
                    //----------------------------------------------------
                    //here send quest completed event
                    ReceiveEvent(CustomEventCode.QuestCompleted, removedQuest.GetInfo());

                    return true;
                }
            }

            return false;
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

        private void SetIntegerVariable(string name, int val) {
            bool setted = false;
            foreach (var kvp in m_ActiveQuests ) {
                if(kvp.Value.SetInteger(name, val)) {
                    setted = true;
                }
            }
            if(setted) {
                TryCheckActiveQuests(null);
            }
        }

        private void SetFloatVariable(string name, float val) {
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

        private void SetBoolVariable(string name, bool val) {
            bool setted = false;
            foreach(var kvp in m_ActiveQuests ) {
                if(kvp.Value.SetBool(name, val)) {
                    s_Log.InfoFormat("setted bool variable: {0}->{1} on quest: {2}".Lime(), name, val, kvp.Value.id);
                    setted = true;
                }
            }
            if(setted) {
                TryCheckActiveQuests(null);
            }
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

        [ComponentMessage(true, Log ="QuestManager->OnStationExited()#orange")]
        public void OnStationExited() {
            SetBoolVariable("at_space", true);
        }
    }
}

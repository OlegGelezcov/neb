using ExitGames.Logging;
using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Space.Game.Inventory;
using System.Collections;
using System.Collections.Concurrent;
using Common;
using Nebula.Game.Components.Quests;
using Nebula.Quests.Drop;
using Nebula.Inventory.Objects;
using ServerClientCommon;
using Space.Server;
using Nebula.Server.Components;
using Nebula.Game.Events;

namespace Nebula.Game.Components {
    public class QuestChest : NebulaBehaviour, IChest {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ServerInventoryItem>> m_Content = new ConcurrentDictionary<string, ConcurrentDictionary<string, ServerInventoryItem>>();

        private readonly float m_Duration = 4 * 60;
        private float m_Timer;
        private bool m_SubscribedToEvents = false;

        private List<string> m_Quests;

        public void Init(QuestChestComponentData data) {
            m_Quests = data.quests;
            if(props != null ) {
                props.SetProperty((byte)PS.QuestChest, m_Quests.CommaString());
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.QuestChest;
            }
        }

        public override void Start() {
            base.Start();
            m_Timer = m_Duration;
            if(m_Quests == null ) {
                m_Quests = new List<string>();
            }
            if (props != null) {
                props.SetProperty((byte)PS.QuestChest, m_Quests.CommaString());
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            m_Timer -= deltaTime;
            if(m_Timer < 0.0f ) {
                (nebulaObject as Item).Destroy();
            }
            if(!m_SubscribedToEvents) {
                var subscriber = nebulaObject.AddComponent<QuestChestEventSubscriber>();
                m_SubscribedToEvents = true;
            }
        }

        private bool IsNotFilledForPlayer(string playerId ) {
            return (!IsFilledForPlayer(playerId));
        }

        private bool IsFilledForPlayer(string playerId ) {
            return m_Content.ContainsKey(playerId);
        }

        private bool IsValidForQuest(string questId ) {
            if(m_Quests != null ) {
                return m_Quests.Contains(questId);
            }
            return false;
        }

        private bool IsValidForQuest(QuestChestDropSource source) {
            if(source != null ) {
                return IsValidForQuest(source.quest);
            }
            return false;
        }

        private ServerInventoryItem GenerateItemOnDictionary(DropItem dropItem) {
            if(dropItem != null ) {
                switch(dropItem.itemType ) {
                    case InventoryObjectType.quest_item: {
                            QuestObjectDropItem questDropItem = dropItem as QuestObjectDropItem;
                            if (questDropItem != null) {
                                QuestItemObject questItemObject = new QuestItemObject(dropItem.itemId, questDropItem.quest);
                                ServerInventoryItem serverInventoryItem = new ServerInventoryItem(questItemObject, questDropItem.count);
                                return serverInventoryItem;
                            }
                        }
                        break;
                    default:
                        return null;
                }
            }
            return null;
        }

        private void FillPlayer(string playerId ) {
            if (IsNotFilledForPlayer(playerId)) {
                bool success = false;
                NebulaObject playerObject;
                if (nebulaObject.mmoWorld().TryGetObject((byte)ItemType.Avatar, playerId, out playerObject)) {
                    var questManager = playerObject.GetComponent<QuestManager>();
                    if (questManager != null) {
                        var dropInfoList = questManager.activeDropInfoList;
                        ConcurrentDictionary<string, ServerInventoryItem> playerItems = new ConcurrentDictionary<string, ServerInventoryItem>();
                        foreach(var dropInfo in dropInfoList ) {
                            if(dropInfo.source.itempType == ItemType.QuestChest ) {
                                if(IsValidForQuest(dropInfo.source as QuestChestDropSource)) {
                                    var genItem = GenerateItemOnDictionary( dropInfo.item);
                                    if(genItem != null ) {
                                        playerItems.TryAdd(genItem.Object.Id, genItem);
                                    }
                                }
                            }
                        }
                        if(m_Content.TryAdd(playerId, playerItems)) {
                            success = true;
                        }
                    }
                }

                if(!success) {
                    m_Content.TryAdd(playerId, new ConcurrentDictionary<string, ServerInventoryItem>());
                }
            }
        }

        private void CheckAndFillPlayer(string playerId ) {
            if(IsNotFilledForPlayer(playerId)) {
                FillPlayer(playerId);
            }
        }

        #region IChest
        public object[] ContentRaw(string playerID) {
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if(TryGetActorObjects(playerID, out filteredObjects)) {
                object[] raw = new object[filteredObjects.Count];
                int index = 0;
                foreach(var p in filteredObjects ) {
                    raw[index++] = new Hashtable {
                        { (int)SPC.Count, p.Value.Count },
                        { (int)SPC.Info, p.Value.GetInfo() }
                    };
                }
                return raw;
            }
            return new object[] { };
        }

        public Hashtable GetInfoForActor(string actorId) {
            Hashtable hash = new Hashtable();
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if(TryGetActorObjects(actorId, out filteredObjects)) {
                foreach(var pair in filteredObjects ) {
                    hash.Add(pair.Value.Object.Id, pair.Value.GetInfo());
                }
            }
            hash.Add((int)SPC.Target, nebulaObject.Id);
            hash.Add((int)SPC.TargetType, nebulaObject.Type);
            return hash;
        }

        public bool TryGetActorObjects(string playerID, out ConcurrentDictionary<string, ServerInventoryItem> playerObjects) {
            CheckAndFillPlayer(playerID);
            return m_Content.TryGetValue(playerID, out playerObjects);
        }

        public bool TryGetObject(string playerID, string inventoryObjectID, out ServerInventoryItem obj) {
            CheckAndFillPlayer(playerID);
            obj = null;
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if(m_Content.TryGetValue(playerID, out filteredObjects)) {
                if(filteredObjects.TryGetValue(inventoryObjectID, out obj)) {
                    return true;
                }
            }
            return false;
        }

        public bool TryRemoveActorObjectids(string playerID, List<string> inventoryObjectIDs) {
            CheckAndFillPlayer(playerID);
            ConcurrentDictionary<string, ServerInventoryItem> filtered = null;
            bool result = true;
            if(m_Content.TryGetValue(playerID, out filtered)) {
                foreach(var id in inventoryObjectIDs ) {
                    ServerInventoryItem removed = null;
                    result = result && filtered.TryRemove(id, out removed);
                }
            }
            return result;
        }

        public bool TryRemoveObject(string playerID, string inventoryObjectID) {
            CheckAndFillPlayer(playerID);
            ConcurrentDictionary<string, ServerInventoryItem> filtered = null;
            if(m_Content.TryGetValue(playerID, out filtered)) {
                ServerInventoryItem removedObject;
                return filtered.TryRemove(inventoryObjectID, out removedObject);
            }
            return false;
        } 
        #endregion

        public bool HasQuest(string qId ) {
            if(m_Quests != null ) {
                return m_Quests.Contains(qId);
            }
            return false;
        }

        public void RegenerateContentForPlayer(string playerId ) {
            if(m_Content.ContainsKey(playerId)) {
                ConcurrentDictionary<string, ServerInventoryItem> items = null;
                if(m_Content.TryRemove(playerId, out items)) {
                    CheckAndFillPlayer(playerId);
                }
            }
        }
    }
}

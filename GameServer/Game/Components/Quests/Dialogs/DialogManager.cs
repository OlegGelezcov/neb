﻿/*
using Common;
using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;
using Nebula.Quests.Actions;
using Nebula.Quests.Dialogs;
using ExitGames.Logging;
using Nebula.Database;
using Space.Game;
using GameMath;
using Nebula.Inventory.Objects;
using Space.Game.Inventory;

namespace Nebula.Game.Components.Quests.Dialogs {

    public class DialogManager : NebulaBehaviour, IInfo, IPostActionTarget {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private readonly List<string> m_CompletedDialogs = new List<string>();

        private RaceableObject m_RaceComponent;
        private QuestManager m_QuestComponent;
        private MmoMessageComponent m_MmoComponent;
        private MmoActor m_Player;


        private bool m_StartCalled = false;

        public override void Start() {
            if (!m_StartCalled) {
                m_StartCalled = true;
                base.Start();
                m_RaceComponent = GetComponent<RaceableObject>();
                m_QuestComponent = GetComponent<QuestManager>();
                m_MmoComponent = GetComponent<MmoMessageComponent>();
                m_Player = GetComponent<MmoActor>();
            }
        }

        public void Reset() {
            m_CompletedDialogs.Clear();
            SendInfo();
        }

        public void Load() {
            Start();
            bool isNew = false;
            var character = GetComponent<PlayerCharacterObject>();
            var app = nebulaObject.mmoWorld().application;
            Hashtable dialogHash = DialogDatabase.instance(app).LoadDialogs(character.characterId, resource as Res, out isNew);
            if(dialogHash != null ) {
                ParseInfo(dialogHash);
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Dialogs;
            }
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.CompletedDialogs, m_CompletedDialogs.ToArray() }
            };
        }

        public void ParseInfo(Hashtable hash) {
            m_CompletedDialogs.Clear();
            string[] complDlgs = hash.GetValue<string[]>((int)SPC.CompletedDialogs, new string[] { });

            if(complDlgs != null ) {
                foreach(string dialog in complDlgs ) {
                    m_CompletedDialogs.Add(dialog);
                }
            }
        }

        public void CompleteDialog(string id) {
            //if not already completed
            if(IsNotCompleted(id)) {
                var dialogData = resource.dialogs.GetDialog(race, id);
                if(dialogData != null ) {
                    //add to completed collection
                    m_CompletedDialogs.Add(id);
                    //execute actions after completion
                    ExecutePostActions(this, dialogData.postActions);
                    //send event to player about completed
                    ReceiveDialogCompleted(id);
                }
            }
        }

        public void SendInfo() {
            if(m_MmoComponent != null ) {
                m_MmoComponent.ReceiveDialogs(GetInfo());
            }
        }

        private void ReceiveDialogCompleted(string dialogId) {
            if(m_MmoComponent != null ) {
                m_MmoComponent.ReceiveDialogCompleted(dialogId);
            }
        }

        public void ExecutePostActions(List<PostAction> postActions) {
            ExecutePostActions(this, postActions);
        }

        private void ExecutePostActions(IPostActionTarget target, List<PostAction> postActions ) {
            if(postActions == null ) {
                return;
            }

            foreach(var postAction in postActions ) {
                switch(postAction.name ) {
                    case PostActionName.START_QUEST: {
                            StartQuestPostAction startQuestAction = postAction as StartQuestPostAction;
                            if(startQuestAction != null ) {
                                target.StartQuest(startQuestAction.questId);
                            }
                        }
                        break;
                    case PostActionName.REMOVE_ITEM: {
                            if(player != null ) {
                                RemoveItemPostAction removeItemPostAction = postAction as RemoveItemPostAction;
                                int curCount = player.Inventory.ItemCount(removeItemPostAction.type, removeItemPostAction.id);
                                int deleteCount = Math.Min(curCount, removeItemPostAction.count);
                                if(deleteCount > 0 ) {
                                    player.Inventory.Remove(removeItemPostAction.type, removeItemPostAction.id, deleteCount);
                                    player.EventOnInventoryUpdated();
                                }
                            }
                        }
                        break;
                    case PostActionName.ADD_ITEM_TO_HANGAR_UNIQUE: {
                            if(player != null ) {
                                AddItemToHangarUniquePostAction act = postAction as AddItemToHangarUniquePostAction;
                                if (!player.Station.StationInventory.HasItem(act.type, act.id)) {

                                    if (player.Station.StationInventory.HasSlotsForItems(new List<string> { act.id })) {

                                        switch (act.type) {
                                            case InventoryObjectType.quest_item: {
                                                    QuestItemObject qObj = new QuestItemObject(act.id, act.quest);
                                                    player.Station.StationInventory.Add(qObj, act.count);
                                                    player.EventOnStationHoldUpdated();
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case PostActionName.SET_BOOL_VARIABLE: {
                            if(m_QuestComponent != null ) {
                                SetBoolVariablePostAction setBoolPostAction = postAction as SetBoolVariablePostAction;
                                if(setBoolPostAction != null  ) {
                                    if( m_QuestComponent.SetBoolVariable(setBoolPostAction.variableName, setBoolPostAction.variableValue) ) {
                                        s_Log.InfoFormat("set bool variable success".Lime());
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        #region IPostActionTarget
        public bool StartQuest(string id) {
            if(m_QuestComponent != null ) {
                if(m_QuestComponent.TryStartQuest(id)) {
                    s_Log.Info(string.Format("quest -> {0} started successfully", id).Green());
                    return true;
                } else {
                    s_Log.Info(string.Format("error when starting quest -> {0}", id));
                }
            }
            return false;
        } 
        #endregion

        private Race race {
            get {
                if(m_RaceComponent != null ) {
                    return m_RaceComponent.getRace();
                }
                return Race.None;
            }
        }

        private bool IsNotCompleted(string id ) {
            return (false == m_CompletedDialogs.Contains(id));
        }

        public bool Completed(string id) {
            return (false == IsNotCompleted(id));
        }

        private MmoActor player {
            get {
                if(m_Player == null ) {
                    m_Player = GetComponent<MmoActor>();
                }
                return m_Player;
            }
        }
    }
}
*/
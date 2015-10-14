using Common;
using ExitGames.Logging;
using NebulaCommon;
using NebulaCommon.Group;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using SelectCharacter.Characters;
using SelectCharacter.Events;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Group {
    public class GroupService {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private GroupCache mCache = new GroupCache();
        private SelectCharacterApplication mApplication;

        public GroupService(SelectCharacterApplication app) {
            mApplication = app;
        }

        /// <summary>
        /// Exiting from group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="characterID"></param>
        /// <returns></returns>
        public bool ExitGroup(string groupID, string characterID) {
            log.InfoFormat("exit group = {0}, character = {1}", groupID, characterID);

            NebulaCommon.Group.Group group;
            if(mCache.TryGetGroup(groupID, out group)) {
                if(group.HasMember(characterID)) {
                    if(group.IsLeader(characterID)) {
                        group.SetRandomLeaderExclude(characterID);
                    }
                    bool result = group.RemoveMember(characterID);
                    SendGroupUpdate(groupID);
                    CheckForRemoving(group);
                    if(result) {

                        SetGroupOnPeer(characterID, string.Empty);
                        //send message to character that its removed ok
                        SendGroupRemovedToCharacter(characterID, group.groupID);
                    }
                    return result;
                }
            }
            return false;
        }

        public void OnClientDisconnect(string characterID ) {
            NebulaCommon.Group.Group group;
            if(mCache.TryGetGroupForCharacter(characterID, out group)) {
                ExitGroup(group.groupID, characterID);
            }
        }

        private void CheckForRemoving(NebulaCommon.Group.Group group) {
            if(group.memberCount < 2) {
                mCache.TryRemoveGroup(group.groupID);
                foreach(var mem in group.members) {
                    SendGroupRemovedToCharacter(mem.Value.characterID, group.groupID); 
                }
                S2SSendGroupRemoved(group.groupID);
            }
        }

        public bool SetGroupOpened(string whoCharacterID, string groupID, bool opened) {
            NebulaCommon.Group.Group group;
            if (mCache.TryGetGroup(groupID, out group)) {
                if (group.IsLeader(whoCharacterID)) {
                    group.opened = opened;
                    SendGroupUpdate(groupID);
                    return true;
                }
            }
            return false;
        }

        public bool AllowInviteToGroup(string characterID ) {

            NebulaCommon.Group.Group group;

            if(!mCache.TryGetGroupForCharacter(characterID, out group ) ) {
                return true;
            }

            if(!group.allowNewMembers) {
                return false;
            }

            return group.IsLeader(characterID);
        }

        public bool IsSomeGroupMember(string characterID ) {
            NebulaCommon.Group.Group group;

            if (mCache.TryGetGroupForCharacter(characterID, out group)) {
                return true;
            }
            return false;
        }


        public bool RemoveMember( string groupID, string sourceCharacterID, string targetCharacterID) {
            NebulaCommon.Group.Group group;
            if(sourceCharacterID == targetCharacterID) { return false; }

            if(mCache.TryGetGroup(groupID, out group)) {
                if(group.IsLeader(sourceCharacterID)) {
                    bool success = group.RemoveMember(targetCharacterID);
                    SendGroupUpdate(groupID);
                    CheckForRemoving(group);
                    if(success) {
                        SendGroupRemovedToCharacter(targetCharacterID, groupID);
                        SetGroupOnPeer(targetCharacterID, string.Empty);
                    }
                    return success;
                }
            }
            return false;
        }

        public bool SetLeader(string groupID, string characterID ) {
            NebulaCommon.Group.Group group;
            if(mCache.TryGetGroup(groupID, out group)) {
                if(group.HasMember(characterID)) {
                    group.SetLeader(characterID);
                    SendGroupUpdate(groupID);
                    return true;
                }
            }
            return false;
        }

        public bool IsLeader(string groupID, string characterID ) {
            NebulaCommon.Group.Group group;
            if(mCache.TryGetGroup(groupID, out group)) {
                if(group.leader == null ) {
                    return false;
                }
                if(group.leader.characterID == characterID ) {
                    return true;
                }
            }
            return false;
        }

        public void HandleNotification(string gameRefID, string characterID, string login, Hashtable data) {
            GroupAction action = (GroupAction)(int)data[(int)SPC.Action];
            switch(action) {
                case GroupAction.CreateGroup:
                    CreateGroupFromNotification(gameRefID, characterID, login, data);
                    break;
                case GroupAction.AddToGroup:
                    AddCharacterToGroupNotification(gameRefID, characterID, login, data);
                    break;
                case GroupAction.RequestToGroup:
                    AccepCharacterToGroup(gameRefID, characterID, login, data);
                    break;
            }
        }

        public bool TryGetLeader(string groupID, out GroupMember leaderMember) {
            leaderMember = null;
            NebulaCommon.Group.Group group;
            if(!mCache.TryGetGroup(groupID, out group )) {
                log.Info("group not found");
                return false;
            }

            leaderMember = group.leader;
            if(leaderMember == null ) {
                log.Info("leader not found");
                return false;
            }

            return true;
        }

        private bool AccepCharacterToGroup(string gameRefID, string characterID, string login, Hashtable data) {
            string sourceGameRefID = data.Value<string>((int)SPC.GameRefId);
            string sourceCharacterID = data.Value<string>((int)SPC.CharacterId);
            string sourceLogin = data.Value<string>((int)SPC.Login);

            NebulaCommon.Group.Group group = null;
            if(mCache.TryGetGroupForCharacter(characterID, out group)) {
                if(group.IsLeader(characterID) && group.allowNewMembers ) {
                    if(AddCharacterToGroup(group, sourceGameRefID, sourceCharacterID, sourceLogin, false) ) {
                        SendGroupUpdate(group.groupID);
                        SetGroupOnPeer(characterID, group.groupID);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AddCharacterToGroupNotification(string gameRefID, string characterID, string login, Hashtable data) {
            string sourceGameRefID = data.Value<string>((int)SPC.GameRefId);
            string sourceCharacterID = data.Value<string>((int)SPC.CharacterId);
            string sourceLogin = data.Value<string>((int)SPC.Login);
            NebulaCommon.Group.Group group = null;
            if (mCache.TryGetGroupForCharacter(sourceCharacterID, out group)) {
                if (group.IsLeader(sourceCharacterID)) {
                    if (AddCharacterToGroup(group, gameRefID, characterID, login, false)) {
                        SendGroupUpdate(group.groupID);
                        SetGroupOnPeer(characterID, group.groupID);
                        return true;
                    }
                }
            }
            return false;
        }


        private bool CreateGroupFromNotification(string gameRefID, string characterID, string login, Hashtable data) {
            string sourceGameRefID = data.Value<string>((int)SPC.GameRefId);
            string sourceCharacterID = data.Value<string>((int)SPC.CharacterId);
            string sourceLogin = data.Value<string>((int)SPC.Login);

            NebulaCommon.Group.Group group = null;
            if(mCache.TryGetGroupForCharacter(sourceCharacterID, out group)) {
                if (group.IsLeader(sourceCharacterID)) {
                    if (AddCharacterToGroup(group, gameRefID, characterID, login, false)) {
                        SendGroupUpdate(group.groupID);
                        SetGroupOnPeer(characterID, group.groupID);
                        return true;
                    }
                }
            } else {
                var newGroup = new NebulaCommon.Group.Group { groupID = Guid.NewGuid().ToString() };
                bool leaderAdded = AddCharacterToGroup(newGroup, sourceGameRefID, sourceCharacterID, sourceLogin, true);
                bool otherAdded = AddCharacterToGroup(newGroup, gameRefID, characterID, login, false);
                mCache.TryAddGroup(newGroup);

                if(leaderAdded) {
                    SetGroupOnPeer(sourceCharacterID, newGroup.groupID);
                }
                if(otherAdded) {
                    SetGroupOnPeer(characterID, newGroup.groupID);
                }
                if(leaderAdded && otherAdded ) {
                    SendGroupUpdate(newGroup.groupID);
                    return true;
                }
            }
            return false;
        }

        private bool AddCharacterToGroup(NebulaCommon.Group.Group group, string gameRefID, string characterID, string login, bool asLeader) {
            if(group.allowNewMembers) {
                DbPlayerCharactersObject player;
                if(!mApplication.Players.TryGetPlayer(gameRefID, out player)) {
                    log.InfoFormat("player not found = {0}", gameRefID);
                    return false;
                }

                var character = player.GetCharacter(characterID);
                if(character == null ) {
                    log.InfoFormat("character not found = {0}", characterID);
                    return false;
                }

                bool result =  group.AddMember(new GroupMember {
                    characterID = characterID,
                    gameRefID = gameRefID,
                    isLeader = asLeader,
                    login = login,
                    worldID = character.WorldId,
                    exp = character.Exp,
                    workshop = character.Workshop});
                if(result) {
                    SetGroupOnPeer(characterID, group.groupID);
                }
                return result;

                //SendGroupUpdate(group.groupID);
            }
            return false;
        }


        public void SendGroupUpdate(string groupID) {
            NebulaCommon.Group.Group group;
            if (!mCache.TryGetGroup(groupID, out group)) {
                log.InfoFormat("group not found = {0}", groupID);
                return;
            }
            S2SSendGroupUpdate(group);
            SendGroupUpdateToClients(group);
        }

        private void SetGroupOnPeer(string characterID, string groupID) {
            SelectCharacterClientPeer peer;
            if(mApplication.Clients.TryGetPeerForCharacterId(characterID, out peer)) {
                peer.SetGroup(groupID);
            }
        }

        private void SendGroupUpdateToClients(NebulaCommon.Group.Group group) {        
            foreach(var member in group.members) {
                SelectCharacterClientPeer peer;
                if(mApplication.Clients.TryGetPeerForCharacterId(member.Value.characterID, out peer)) {
                    GroupUpdateEvent evt = new GroupUpdateEvent { groupHash = group.GetInfo() };
                    EventData evtData = new EventData((byte)SelectCharacterEventCode.GroupUpdateEvent, evt);
                    peer.SendEvent(evtData, new SendParameters());
                }
            }
        }

       private void SendGroupRemovedToCharacter(string characterID, string groupID ) {
            SelectCharacterClientPeer peer;
            if(mApplication.Clients.TryGetPeerForCharacterId(characterID, out peer)) {
                GroupRemovedEvent evt = new GroupRemovedEvent { Group = groupID };
                EventData eData = new EventData((byte)SelectCharacterEventCode.GroupRemovedEvent, evt);
                peer.SendEvent(eData, new SendParameters()); 
            }
        }

        private void S2SSendGroupUpdate(NebulaCommon.Group.Group group) {

            UpdateZoneForGroupMembers(group);

            S2SGroupUpdateEvent evt = new S2SGroupUpdateEvent { group = group.GetInfo() };
            EventData evtData = new EventData((byte)S2SEventCode.GroupUpdate, evt);
            mApplication.MasterPeer.SendEvent(evtData, new SendParameters());
        }

        private void S2SSendGroupRemoved(string groupID) {
            S2SGroupRemovedEvent evt = new S2SGroupRemovedEvent { Group = string.Empty };
            EventData evtData = new EventData((byte)S2SEventCode.GroupRemoved, evt);
            mApplication.MasterPeer.SendEvent(evtData, new SendParameters());
        }

        private void UpdateZoneForGroupMembers(NebulaCommon.Group.Group group) {
            foreach(var member in group.members) {
                DbPlayerCharactersObject player;
                if ( !mApplication.Players.TryGetPlayer(member.Value.gameRefID, out player) ) {
                    log.InfoFormat("UpdateZoneForGroupMembers(): player not found = {0}", member.Value.gameRefID);
                    continue;
                }
                var character = player.GetCharacter(member.Value.characterID);
                if(character == null ) {
                    log.InfoFormat("UpdateZoneForGroupMembers(): character not found = {0}", member.Value.characterID);
                    continue;
                }

                member.Value.worldID = character.WorldId;
            }
        }

        public Hashtable FindGroups(byte race) {
            return mCache.SearchGroups(mApplication, race);
        }
 
        public bool TryGetGroup(string groupID, out NebulaCommon.Group.Group group) {
            return mCache.TryGetGroup(groupID, out group);
        }
    }
}

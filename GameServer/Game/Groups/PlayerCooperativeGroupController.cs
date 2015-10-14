//using Common;
//using ExitGames.Logging;
//using Nebula.Game.Components;
//using ServerClientCommon;
//using Space.Server;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Space.Game.Groups {
//    public class PlayerCooperativeGroupController {

        

//        private readonly MmoActor player;
//        private CooperativeGroup currentGroup;
//        private GroupActionRequest request = new GroupActionRequest();
//        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

//        public PlayerCooperativeGroupController(MmoActor player) {
//            this.player = player;
//        }

//        private void SetCurrentGroup(CooperativeGroup g) {
//            this.currentGroup = g;
//        }

//        public bool SendRequest(GroupActionRequest r, out int errCode) {
//            errCode = 0;

//            if (request.HasRequest()) {
//                errCode = (int)LogicErrorCode.PLAYER_ALREADY_HAS_NOT_HANDLED_REQUEST;
//                return false;
//            }

//            if (player.Avatar == null || player.Avatar.Disposed) {
//                errCode = (int)LogicErrorCode.ITEM_NULL_OR_DISPOSED;
//                return false;
//            }

//            if(r.RequestType() == GroupActionRequestType.InviteToNewGroup ||
//                r.RequestType() == GroupActionRequestType.InviteToExistingGroup) {
//                    if (AttachedToGroup()) {
//                        errCode = (int)LogicErrorCode.PLAYER_ALREADY_ATTACHED_TO_SOME_GROUP;
//                        return false;
//                    }
//            }

//            this.request = r;
//            this.player.EventGroupRequest(this.request);
//            return true;
//        }

//        public void HandleRequestResponse(bool accept, Hashtable requestInfo, CooperativeGroups groups,  out int errCode) {
//            errCode = (int)LogicErrorCode.OK;
//            if (request == null) {
//                errCode = (int)LogicErrorCode.GROUP_ACTION_REQUEST_IS_NULL;
//                return;
//            }
//            if (!request.HasRequest()) {
//                errCode = (int)LogicErrorCode.PLAYER_NOT_HAS_GROUP_ACTION_REQUEST;
//                return;
//            }

//            byte returnedRequestType = requestInfo.GetValue<byte>((int)SPC.RequestType, (byte)GroupActionRequestType.None);
//            string returnedRequestId = requestInfo.GetValue<string>((int)SPC.Id, string.Empty);

//            if ((byte)this.request.RequestType() != returnedRequestType || this.request.RequestId() != returnedRequestId) {
//                errCode = (int)LogicErrorCode.RESPONSE_AND_REQUEST_TYPES_DONT_SAME;
//                return;
//            }

//            if (!accept && (returnedRequestType != (byte)GroupActionRequestType.ExcludePlayerFromGroup)) {
//                errCode = (int)LogicErrorCode.PLAYER_DONT_ACCEPT_GROUP_REQUEST;
//                this.request.ClearRequest();
//                return;
//            }

//            switch (this.request.RequestType()) {
//                case GroupActionRequestType.InviteToNewGroup: {
//                        string sourceItemId = request.Data().GetValue<string>((int)SPC.Id, string.Empty);
//                        Item sourceItem;
//                        if (!(this.player.World as MmoWorld).ItemCache.TryGetItem((byte)ItemType.Avatar, sourceItemId, out sourceItem)) {
//                            errCode = (int)LogicErrorCode.ITEM_NOT_FOUND;
//                            return;
//                        }
//                        MmoActor sourceActor = (sourceItem as MmoItem).Owner;
                        
//                    //if source actor don't have group create
//                        if (!sourceActor.GroupController().AttachedToGroup()) {
//                            var group = sourceActor.GroupController().CreateGroupAndAttachToGroupAsLeader(groups);
//                            if (group == null) {
//                                errCode = (int)LogicErrorCode.SOURCE_GROUP_IS_NULL;
//                                return;
//                            }

//                            this.AcceptAttachFromPlayer(sourceActor, out errCode);
//                            this.currentGroup.SendGroupUpdate();
//                        } else {
//                            //if source actor already has group try attach to existing group
//                            this.AttachToActorGroup(sourceActor, out errCode);
//                        }
//                        break;
//                    }
//                case GroupActionRequestType.InviteToExistingGroup: {

//                        string sourceItemId = request.Data().GetValue<string>((int)SPC.Id, string.Empty);
//                        Item sourceItem;
//                        if (!(this.player.World as MmoWorld).ItemCache.TryGetItem((byte)ItemType.Avatar, sourceItemId, out sourceItem)) {
//                            errCode = (int)LogicErrorCode.ITEM_NOT_FOUND;
//                            return;
//                        }
//                        MmoActor sourceActor = (sourceItem as MmoItem).Owner;
//                        this.AttachToActorGroup(sourceActor, out errCode);

//                        break;
//                    }
//                case GroupActionRequestType.ExitFromGroup: {
//                        this.RemoveFromGroup(groups, out errCode);
//                    if(currentGroup != null )
//                        this.currentGroup.SendGroupUpdate();
//                        this.player.EnqueueEventGroupUpdate(string.Empty, new Hashtable());
//                        this.SetCurrentGroup(null);
//                        break;
//                    }
//                case GroupActionRequestType.ExcludePlayerFromGroup: {
//                    if (currentGroup != null) {
//                            string characterID = player.GetComponent<PlayerCharacterObject>().characterId;
//                        currentGroup.HandleExcludeRequestResponse(characterID, accept, out errCode);
//                    }
//                    break;
//                    }
//            }

//            this.request.ClearRequest();
//        }

//        public void VoteForExclude(string whoId, string whoDisplayName, out int errorCode ) {
//            string characterID = player.GetComponent<PlayerCharacterObject>().characterId;
//            this.currentGroup.InitializeExcludeRequest(characterID, this.player.name, whoId, whoDisplayName, out errorCode);
//        }

//        private void AttachToActorGroup(MmoActor sourceActor, out int errCode) {
//            errCode = (int)LogicErrorCode.OK;
//            if (sourceActor.AttachedGroup() == null) {
//                errCode = (int)LogicErrorCode.SOURCE_GROUP_IS_NULL;
//                return;
//            }
//            if (sourceActor.AttachedGroup().MemberCount() >= CooperativeGroups.MAX_GROUP_MEMBERS) {
//                errCode = (int)LogicErrorCode.GROUP_IS_FULL;
//                return;
//            }
//            this.AcceptAttachFromPlayer(sourceActor, out errCode);
//            this.currentGroup.SendGroupUpdate();
//        }


//        public void JoinToOpenedGroup(CooperativeGroups groupCache, string groupId, out int errorCode) {
//            errorCode = (int)LogicErrorCode.OK;
//            if (this.AttachedToGroup()) {
//                errorCode = (int)LogicErrorCode.PLAYER_ALREADY_ATTACHED_TO_SOME_GROUP;
//                return;
//            }
//            CooperativeGroup targetGroup = groupCache.GetGroup(groupId);
//            if (targetGroup == null) {
//                errorCode = (int)LogicErrorCode.GROUP_NOT_FOUNDED;
//                return;
//            }

//            if (targetGroup.MemberCount() >= CooperativeGroups.MAX_GROUP_MEMBERS) {
//                errorCode = (int)LogicErrorCode.GROUP_IS_FULL;
//                return;
//            }

//            targetGroup.AddMember(new CooperativeGroupMember(this.player, false));
//            this.SetCurrentGroup(targetGroup);
//            targetGroup.SendGroupUpdate();
//        }

//        public void OnExitGame(CooperativeGroups groups) {
//            if (AttachedToGroup()) {
//                int errorCode = 0;
//                this.RemoveFromGroup(groups, out errorCode);
//                if(this.currentGroup != null )
//                    this.currentGroup.SendGroupUpdate();
//                this.SetCurrentGroup(null);
//            }
//        }

//        public void ExitFromCurrentGroup( CooperativeGroups groups) {
//            int errorCode;
//            this.RemoveFromGroup(groups, out errorCode);
//            if (currentGroup != null)
//                currentGroup.SendGroupUpdate();
//            this.player.EnqueueEventGroupUpdate(string.Empty, new Hashtable());
//            this.SetCurrentGroup(null);
//        }


//        //Free group if this player leader
//        public void FreeGroup(out int errorCode) {
//            errorCode = (int)LogicErrorCode.OK;

//            //check current group exists
//            if (this.currentGroup == null) {
//                errorCode = (int)LogicErrorCode.CURRENT_GROUP_IS_NULL;
//                return;
//            }

//            //check exists current member
//            CooperativeGroupMember myMember = null;
//            string characterID = player.GetComponent<PlayerCharacterObject>().characterId;
//            if (!currentGroup.TryGetMember(characterID, out myMember)) {
//                errorCode = (int)LogicErrorCode.GROUP_MEMBER_NOT_FOUND;
//                return;
//            }

//            //
//            if (!myMember.IsLeader()) {
//                errorCode = (int)LogicErrorCode.MEMBER_IS_NOT_LEADER;
//                return;
//            }

//            this.currentGroup.DetachMembersFromGroup();
//        }

//        public void SetLeaderToCharacter(string characterId, out int errorCode) {
//            errorCode = (int)LogicErrorCode.OK;

//            //check current group exists
//            if (this.currentGroup == null) {
//                errorCode = (int)LogicErrorCode.CURRENT_GROUP_IS_NULL;
//                return;
//            }

//            //check exists current member
//            CooperativeGroupMember myMember = null;
//            string characterID = player.GetComponent<PlayerCharacterObject>().characterId;
//            if (!currentGroup.TryGetMember(characterID, out myMember)) {
//                errorCode = (int)LogicErrorCode.GROUP_MEMBER_NOT_FOUND;
//                return;
//            }

//            //if me not leader exit
//            if (!myMember.IsLeader()) {
//                errorCode = (int)LogicErrorCode.MEMBER_IS_NOT_LEADER;
//                return;
//            }

//            if (myMember.CharacterId() == characterId) {
//                errorCode = (int)LogicErrorCode.I_AM_ALREDY_LEADER;
//                return;
//            }

//            //find target member
//            CooperativeGroupMember targetMember = null;
//            if (!currentGroup.TryGetMember(characterId, out targetMember)) {
//                errorCode = (int)LogicErrorCode.TARGET_MEMBER_NOT_FOUNDED;
//                return;
//            }

//            //change leadership
//            myMember.SetLeader(false);
//            targetMember.SetLeader(true);
//            //send group update
//            currentGroup.SendGroupUpdate();
//        }


//        public CooperativeGroup CreateGroupAndAttachToGroupAsLeader(CooperativeGroups groups) {

//            if (!AttachedToGroup()) {

//                CooperativeGroup newGroup = new CooperativeGroup();
                
//                if (!newGroup.AddMember(new CooperativeGroupMember(player, true))) {
//                    return null;
//                }

//                if (groups.AddGroup(newGroup)) {
//                    this.SetCurrentGroup(newGroup);

//                    return this.currentGroup;
//                }
//            }
//            return null;
//        }

//        public void AcceptAttachFromPlayer(MmoActor actor, out int errCode) {
//            errCode = 0;
//            if (AttachedToGroup()) {
//                errCode = 10;
//                return;
//            }

//            var targetGroup = actor.AttachedGroup();

//            if (targetGroup == null) {
//                errCode = 13;
//                return;
//            }

//            if (!targetGroup.AddMember(new CooperativeGroupMember(player, false))) {
//                errCode = 14;
//                return;
//            }

//            this.SetCurrentGroup(targetGroup);

//        }

//        public void RemoveFromGroup(CooperativeGroups groups, out int errCode) {
//            errCode = (int)LogicErrorCode.OK;

//            if (this.currentGroup == null) {
//                errCode = (int)LogicErrorCode.SOURCE_GROUP_IS_NULL;
//                return;
//            }

//            CooperativeGroupMember member;
//            string characterID = player.GetComponent<PlayerCharacterObject>().characterId;
//            if (!this.currentGroup.TryGetMember(characterID, out member)) {
//                errCode = (int)LogicErrorCode.GROUP_MEMBER_NOT_FOUND;
//                return;
//            }

//            if (!this.currentGroup.RemoveMember(member.CharacterId())) {
//                errCode = (int)LogicErrorCode.ERROR_OF_EXITING_FROM_GROUP;
//                return;
//            }

//            if (this.currentGroup.MemberCount() == 0) {
//                groups.RemoveGroup(this.currentGroup.Id());
//            } else if (this.currentGroup.MemberCount() == 1) {
//                this.currentGroup.DetachMembersFromGroup();
//                groups.RemoveGroup(this.currentGroup.Id());
//            } 
//            else if (member.IsLeader()) {
//                this.currentGroup.SetNewLeader();
//                this.currentGroup.SendGroupUpdate();
//            }
//        }

//        public bool AttachedToGroup() {
//            return currentGroup != null;
//        }

//        public CooperativeGroup Group() {
//            return this.currentGroup;
//        }

//        public string GroupId() {
//            if(currentGroup == null) {
//                return string.Empty;
//            }
//            return currentGroup.Id();
//        }

//        public void Detach() {
//            SetCurrentGroup(null);
//            this.player.EnqueueEventGroupUpdate(string.Empty, new Hashtable());
//        }

//        private bool IsLeader() {
//            string characterID = player.GetComponent<PlayerCharacterObject>().characterId;
//            return currentGroup.IsLeader(characterID);
//        }

//        public void SetGroupOpened(bool opened, out int errorCode) {
//            errorCode = (int)LogicErrorCode.OK;
//            if (!AttachedToGroup()) {
//                errorCode = (int)LogicErrorCode.CURRENT_GROUP_IS_NULL;
//                return;
//            }
//            if (!IsLeader()) {
//                errorCode = (int)LogicErrorCode.MEMBER_IS_NOT_LEADER;
//                return;
//            }
//            currentGroup.SetOpened(opened);
//            currentGroup.SendGroupUpdate();
//        }

//        public void SendChatMessageToGroup(Hashtable message) {
//            if (!AttachedToGroup()) {
//                log.Warn("Player not attached to group");
//            }
//            currentGroup.SendChatMessage(message);
//        }
//    }
//}

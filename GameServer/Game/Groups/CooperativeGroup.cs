//using Common;
//using ExitGames.Threading;
//using Space.Server;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Collections;
//using ServerClientCommon;

//namespace Space.Game.Groups {
//    public class CooperativeGroup : IDisposable, IInfoSource {

//        private readonly string id;
//        private Dictionary<string, CooperativeGroupMember> members;
//        private readonly ReaderWriterLockSlim readerWriterLock;
//        private readonly int maxLockMilliseconds;
//        private ExcludeMemberRequest excludeRequest = new ExcludeMemberRequest();
//        private bool isOpened;

//        public string Id() {
//            return this.id;
//        }

//        public CooperativeGroup() {
//            this.id = Guid.NewGuid().ToString();
//            this.members = new Dictionary<string, CooperativeGroupMember>();
//            this.readerWriterLock = new ReaderWriterLockSlim();
//            this.maxLockMilliseconds = Settings.MaxLockWaitTimeMilliseconds;
//            this.isOpened = true;
//        }


//        public bool AddMember(CooperativeGroupMember member) {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                if (this.members.ContainsKey(member.CharacterId())) {
//                    return false;
//                }
//                this.members.Add(member.CharacterId(), member);
//                return true;
//            }
//        }

//        public bool RemoveMember(string characterId) {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                if (this.excludeRequest.HasExcludeRequest) {
//                    this.excludeRequest.Answers.Remove(characterId);
//                }
//                return this.members.Remove(characterId);
//            }
//        }

//        public bool TryGetMember(string characterId, out CooperativeGroupMember member) {
//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                return this.members.TryGetValue(characterId, out member);
//            }
//        }

//        public bool IsLeader(string characterId) {
//            CooperativeGroupMember member;
//            if (!TryGetMember(characterId, out member)) {
//                return false;
//            }
//            return member.IsLeader();
//        }

//        public int MemberCount() {
//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                return this.members.Count;
//            }
//        }

//        public void Dispose() {
//            this.readerWriterLock.Dispose();
//            GC.SuppressFinalize(this);
//        }

//        public void SetOpened(bool opened) {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                this.isOpened = opened;
//            }
//        }

//        public bool Opened() {
//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                return this.isOpened;
//            }
//        }

//        ~CooperativeGroup() {
//            this.readerWriterLock.Dispose();
//        }

//        public void SetNewLeader() {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                foreach (var pm in this.members) {
//                    pm.Value.SetLeader(true);
//                    break;
//                }
//            }
//        }

//        private void SetLeaderNot(string characterId) {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                foreach (var pm in this.members) {
//                    if (pm.Key != characterId) {
//                        pm.Value.SetLeader(true);
//                        break;
//                    }
//                }
//            }
//        }

//        public void DetachMembersFromGroup() {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                excludeRequest.HasExcludeRequest = false;

//                foreach (var pm in this.members) {
//                    pm.Value.Detach();
//                }
//                this.members.Clear();
//            }
//        }

//        public System.Collections.Hashtable GetInfo() {
//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                Hashtable info = new Hashtable();
//                foreach (var pm in this.members) {
//                    info.Add(pm.Key, pm.Value.GetInfo());
//                }
//                //return info;
//                Hashtable result = new Hashtable();
//                result.Add((int)SPC.Members, info);
//                result.Add((int)SPC.Opened, this.isOpened);
//                result.Add((int)SPC.Id, this.id);
//                return result;
//            }
//        }

//        public Hashtable RequestSearchInfo() {
//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                Hashtable membersHash = new Hashtable();
//                foreach (var pm in this.members) {
//                    membersHash.Add(pm.Key, pm.Value.RequestSearchInfo());
//                }
//                return new Hashtable { 
//                    {(int)SPC.Members, membersHash }
//                };
//            }
//        }

//        public void SendGroupUpdate() {
//            var info = GetInfo();

//            foreach (var pm in members) {
//                pm.Value.SendGroupUpdate(Id(), info);
//            }
//        }


//        public void InitializeExcludeRequest(string fromId, string fromDisplayName, string whoId, string whoDisplayName, out int errorCode) {
//            errorCode = (int)LogicErrorCode.OK;
//            if (excludeRequest.HasExcludeRequest) {
//                errorCode = (int)LogicErrorCode.ALREADY_EXISTS_EXCLUDE_REQUEST;
//                return;
//            }

//            if (MemberCount() <= 2) {
//                errorCode = (int)LogicErrorCode.EXCLUDE_VOTE_DONT_ALLOW_WHEN_TWO_MEMBERS;
//                excludeRequest.HasExcludeRequest = false;
//                return;
//            }

//            if (fromId == whoId) {
//                errorCode = (int)LogicErrorCode.EXCLUDE_VOTE_FOR_SELF_DONT_ALLOWED;
//                excludeRequest.HasExcludeRequest = false;
//                return;
//            }

//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                excludeRequest.HasExcludeRequest = true;
//                excludeRequest.Answers.Clear();
//                excludeRequest.InitializerMemberId = fromId;
//                excludeRequest.ExcludeMemberId = whoId;

//                List<MmoActor> votePlayers = new List<MmoActor>();

//                foreach (var pm in this.members) {
//                    if (pm.Key != fromId && pm.Key != whoId) {
//                        excludeRequest.Answers.Add(pm.Key, new ExcludeAnswer { HasAnswer = false, MemberId = pm.Key });
//                        votePlayers.Add(pm.Value.Player());
//                    }
//                }

//                GroupActionRequest request = new GroupActionRequest();
//                request.SetRequest(GroupActionRequestType.ExcludePlayerFromGroup, Guid.NewGuid().ToString(),
//                    GroupActionRequest.ExcludeFromGroupRequestData(fromId, fromDisplayName, whoId, whoDisplayName));

//                foreach (var vp in votePlayers) {
//                    int errCode2 = (int)LogicErrorCode.OK;
//                    vp.GroupController().SendRequest(request, out errCode2);
//                    if (errCode2 != (int)LogicErrorCode.OK) {
//                        errorCode = errCode2;
//                    }
//                }
//            }

//            if (errorCode != (int)LogicErrorCode.OK) {
//                excludeRequest.HasExcludeRequest = false;
//            }
//        }

//        public void SendChatMessage(Hashtable info) {
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                foreach (var pMember in this.members) {
//                    pMember.Value.SendChatMessage(info);
//                }
//            }
//        }

//        public void HandleExcludeRequestResponse(string characterId, bool answer, out int errorCode) {

//            errorCode = (int)LogicErrorCode.OK;

//            if (!excludeRequest.HasExcludeRequest) {
//                errorCode = (int)LogicErrorCode.NO_EXCLUDE_REQUEST_IN_GROUP;
//                return;
//            }

//            bool allAnswered = false;
//            bool exclude = false;
//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                if (this.excludeRequest.Answers.ContainsKey(characterId)) {
//                    this.excludeRequest.Answers[characterId].Answer = answer;
//                    this.excludeRequest.Answers[characterId].HasAnswer = true;
//                }

                
//                if (!this.excludeRequest.CheckExclude(out allAnswered)) {
//                    if (allAnswered) {
//                        this.excludeRequest.HasExcludeRequest = false;
//                        this.excludeRequest.Answers.Clear();
//                        return;
//                    }
//                } else {
//                    exclude = true;
//                }
//            }

//            if (exclude && allAnswered) {

//                CooperativeGroupMember excludedMember;

//                if (TryGetMember(excludeRequest.ExcludeMemberId, out excludedMember)) {

//                    if (excludedMember.IsLeader()) {
//                        this.SetLeaderNot(excludeRequest.ExcludeMemberId);
//                    }

//                    excludedMember.Detach();

//                    RemoveMember(excludeRequest.ExcludeMemberId);

//                    excludedMember.Player().Chat.AddMessage(new Hashtable{
//                        {(int)SPC.ChatMessageGroup, (byte)ChatGroup.group },
//                        {(int)SPC.ChatMessage, "You dropped from group" },
//                        {(int)SPC.ChatSourceLogin, string.Empty },
//                        {(int)SPC.ChatSourceName, string.Empty },
//                        {(int)SPC.ChatMessageId, Guid.NewGuid().ToString() },
//                        {(int)SPC.ChatReceiverLogin, excludedMember.Player().name }
//                    });

//                    SendGroupUpdate();
//                }
//                this.excludeRequest.HasExcludeRequest = false;
//                this.excludeRequest.Answers.Clear();

//            }
//        }

//        public class ExcludeMemberRequest {
//            public bool HasExcludeRequest = false;
//            public string ExcludeMemberId = string.Empty;
//            public string InitializerMemberId = string.Empty;
//            public Dictionary<string, ExcludeAnswer> Answers = new Dictionary<string, ExcludeAnswer>();

//            public bool CheckExclude(out bool allAnswered) {
//                bool exclude = true;
//                allAnswered = true;
//                foreach (var a in Answers) {
//                    if (!a.Value.HasAnswer) {
//                        allAnswered = false;
//                        exclude = false;
//                        break;
//                    }
//                    if (false == a.Value.Answer) {
//                        exclude = false;
//                    }
//                }
//                return exclude;
//            }
 
//        }

//        public class ExcludeAnswer {
//            public bool HasAnswer = false;
//            public bool Answer = false;
//            public string MemberId = string.Empty;
//        }
//    }
//}

using Common;
using MongoDB.Driver.Builders;
using NebulaCommon;
using SelectCharacter.Chat;
using SelectCharacter.Notifications;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Friends {
    public class FriendService {

        public SelectCharacterApplication application { get; private set; }
        private readonly FriendCache mCache = new FriendCache();

        public FriendService(SelectCharacterApplication app) {
            application = app;
        }

        public DbObjectWrapper<PlayerFriends> GetFriendsObject(string gameRefID, string login) {
            DbObjectWrapper<PlayerFriends> data = null;
            if(mCache.TryGetPlayerFriends(gameRefID, out data)) {
                return data;
            }

            var dbFriends = application.DB.friends.FindOne(Query<PlayerFriends>.EQ(f => f.gameRefID, gameRefID));
            if(dbFriends != null ) {
                if(mCache.TryAddPlayerFriends(dbFriends)) {
                    if(mCache.TryGetPlayerFriends(gameRefID, out data)) {
                        return data;
                    }
                }
            } else {
                PlayerFriends newFriends = new PlayerFriends { gameRefID = gameRefID, login = login, friends = new Dictionary<string, Friend>() };
                application.DB.friends.Save(newFriends);
                if(mCache.TryAddPlayerFriends(newFriends)) {
                    if(mCache.TryGetPlayerFriends(gameRefID, out data)) {
                        return data;
                    }
                }
            }
            data = null;
            return null;
        }

        public void Update() {
            mCache.Save(application);
        }

        public void HandleNotification(Notification notification) {
            if(notification.subType == (int)NotificationSubType.RequestFriend) {
                Hashtable data = notification.data;
                if(data != null ) {
                    string fromGameRefID = data.GetValue<string>((int)SPC.SourceGameRefID, string.Empty);
                    string fromLogin = data.GetValue<string>((int)SPC.SourceLogin, string.Empty);
                    string toGameRefID = data.GetValue<string>((int)SPC.TargetGameRefID, string.Empty);
                    string toLogin = data.GetValue<string>((int)SPC.TargetLogin, string.Empty);

                    if(CommonUtils.AllNonEmty(new string[] { fromGameRefID, fromLogin, toGameRefID, toLogin } )) {

                        var sourceFriend = GetFriendsObject(fromGameRefID, fromLogin);

                        if(sourceFriend == null ) {
                            return;
                        }

                        var targetFriend = GetFriendsObject(toGameRefID, toLogin);
                        if(targetFriend == null ) {
                            return;
                        }

                        var sourcePlayer = application.Players.GetExistingPlayer(fromGameRefID);
                        if(sourcePlayer == null ) {
                            return;
                        }

                        var targetPlayer = application.Players.GetExistingPlayer(toGameRefID);
                        if(targetPlayer == null ) {
                            return;
                        }


                        
                        if(!sourceFriend.Data.IsFriend(targetFriend.Data)) {
                            sourceFriend.Data.AddFriend(toGameRefID, toLogin);
                            sourceFriend.Changed = true;
                            ChatMessage msg = new ChatMessage {
                                chatGroup = (int)ChatGroup.whisper, links = new List<ChatLinkedObject>(),
                                message = string.Format("{0} now friend with {1}", fromLogin, toLogin), messageID = Guid.NewGuid().ToString(),
                                sourceCharacterID = sourcePlayer.Data.SelectedCharacterId, sourceLogin = "Service", targetCharacterID = sourcePlayer.Data.SelectedCharacterId, targetLogin = fromLogin
                            };
                            application.Chat.SendMessage(msg);
                        }

                        if(!targetFriend.Data.IsFriend(sourceFriend.Data)) {
                            targetFriend.Data.AddFriend(fromGameRefID, fromLogin);
                            targetFriend.Changed = true;
                            ChatMessage msg = new ChatMessage {
                                chatGroup = (int)ChatGroup.whisper, links = new List<ChatLinkedObject>(),
                                message = string.Format("{0} now friend with {1}", toLogin, fromLogin), messageID = Guid.NewGuid().ToString(),
                                sourceCharacterID = targetPlayer.Data.SelectedCharacterId, sourceLogin = "Service", targetCharacterID = targetPlayer.Data.SelectedCharacterId, targetLogin = toLogin
                            };
                            application.Chat.SendMessage(msg);
                        }

                        application.Clients.SendGenericEventToGameref(fromGameRefID, new Events.GenericEvent {
                            subCode = (int)SelectCharacterGenericEventSubCode.FriendsUpdate,
                            data = GetFriendsInfo(fromGameRefID, fromLogin) });
                        application.Clients.SendGenericEventToGameref(toGameRefID, new Events.GenericEvent {
                            subCode = (int)SelectCharacterGenericEventSubCode.FriendsUpdate,
                            data = GetFriendsInfo(toGameRefID, toLogin) });
                        //application.SendEventToClient
                    }
                }
            }
        }

        public Hashtable GetFriendsInfo(string gameRefID, string login) {
            var friends = GetFriendsObject(gameRefID, login);
            if(friends != null) {
                return friends.Data.GetInfo(application);
            }
            return new Hashtable();
        }

        public bool RequestFriend(string fromGameRefID, string fromLogin, string toGameRefID, string toLogin) {
            var sourceFriends = GetFriendsObject(fromGameRefID, fromLogin);
            if(sourceFriends == null) {
                return false;
            }

            var targetFriends = GetFriendsObject(toGameRefID, toLogin);
            if(targetFriends == null ) {
                return false;
            }

            if(sourceFriends.Data.IsFriend(targetFriends.Data)) {
                return false;
            }

            var targetPlayer = application.Players.GetExistingPlayer(toGameRefID);
            if(targetPlayer == null ) {
                return false;
            }

            if (targetPlayer.Data.Characters == null) {
                return false;
            }

            if (string.IsNullOrEmpty(targetPlayer.Data.SelectedCharacterId)) {
                return false;
            }

            string uniqueID = fromGameRefID + fromLogin + toGameRefID + toLogin;
            Hashtable data = new Hashtable {
                { (int)SPC.SourceGameRefID, fromGameRefID },
                { (int)SPC.SourceLogin, fromLogin },
                { (int)SPC.TargetGameRefID, toGameRefID },
                { (int)SPC.TargetLogin, toLogin }
            };
            var notification = application.Notifications.Create(uniqueID, "s_note_friend_request",
                 data, NotficationRespondAction.YesDelete, NotificationSourceServiceType.Friends, NotificationSubType.RequestFriend);
            application.Notifications.SetNotificationToCharacter(targetPlayer.Data.SelectedCharacterId, notification);
            return true;
        }

        public void RemoveFriend(string sourceGameRefID, string sourceLogin, string targetGameRefID, string targetLogin) {
            var sourceFriends = GetFriendsObject(sourceGameRefID, sourceLogin);
            if (sourceFriends != null) {
                sourceFriends.Data.RemoveFriend(targetGameRefID);
                sourceFriends.Changed = true;
            }

            var targetFriends = GetFriendsObject(targetGameRefID, targetLogin);
            if (targetFriends != null) {
                targetFriends.Data.RemoveFriend(sourceGameRefID);
                targetFriends.Changed = true;
            }

            if (sourceFriends != null) {
                application.Clients.SendGenericEventToGameref(sourceGameRefID, new Events.GenericEvent {
                    subCode = (int)SelectCharacterGenericEventSubCode.FriendsUpdate,
                    data = sourceFriends.Data.GetInfo(application) });
            }
            if (targetFriends != null) {
                application.Clients.SendGenericEventToGameref(targetGameRefID, new Events.GenericEvent {
                    subCode = (int)SelectCharacterGenericEventSubCode.FriendsUpdate,
                    data = targetFriends.Data.GetInfo(application) });
            }
        }

    }
}

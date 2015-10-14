using Common;
using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Photon.SocketServer;
using SelectCharacter.Characters;
using SelectCharacter.Events;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Notifications {

    public class NotificationService {

        private static ILogger log = LogManager.GetCurrentClassLogger();


        private SelectCharacterApplication mApplication;
        private NotificationCache mCache;

        public NotificationService(SelectCharacterApplication application) {
            mApplication = application;
            mCache = new NotificationCache();
        }

        public CharacterNotifications GetNotifications(string characterId ) {
            CharacterNotifications notifications = null;
            if(mCache.TryGetNotification(characterId, out notifications)) {
                return notifications;
            }

            notifications = mApplication.DB.Notifications.FindOne(Query<CharacterNotifications>.EQ(n => n.characterId, characterId));
            if(notifications != null ) {
                mCache.SetNotification(notifications);
                mCache.ForceSave(characterId, mApplication.DB.Notifications);
                return notifications;
            }

            notifications = new CharacterNotifications { characterId = characterId, notifications = new Dictionary<string, Notification>() };
            mApplication.DB.Notifications.Save(notifications);
            mCache.SetNotification(notifications);
            mCache.ForceSave(characterId, mApplication.DB.Notifications);
            return notifications;
        }

        /// <summary>
        /// Handle notfication for player request
        /// </summary>
        /// <param name="characterId">Character id</param>
        /// <param name="notificationId">Notification id</param>
        /// <param name="response">true - player tap YES(true allowed only on YesDelete notifications), false - player tap delete</param>
        public void HandleNotification(string characterId, string notificationId, bool response ) {
            var notifications = GetNotifications(characterId);
            if(notifications == null ) {
                log.InfoFormat("notifications for character = {0} not founded", characterId);
                return;
            }
            var notification = notifications.Get(notificationId);
            if(notification == null ) {
                log.InfoFormat("notification = {0} for character = {1} not founded", notificationId, characterId);
                return;
            }

            if(response) {
                NotficationRespondAction action = (NotficationRespondAction)notification.respondAction;
                if(action != NotficationRespondAction.YesDelete) {
                    log.InfoFormat("notification = {0} for character = {1} don't have required action type YesDelete", notificationId, characterId);
                    return;
                } else {
                    NotificationSourceServiceType serviceType = (NotificationSourceServiceType)notification.sourceServiceType;
                    if (serviceType == NotificationSourceServiceType.Guild) {
                        //handle notification for guild
                        mApplication.Guilds.HandleNotification(characterId, notification);
                    } else if (serviceType == NotificationSourceServiceType.Friends) {
                        mApplication.friends.HandleNotification(notification);
                    } else if (serviceType == NotificationSourceServiceType.Server) {
                        log.InfoFormat("handle Server notification {0}", notificationId);
                    } else if (serviceType == NotificationSourceServiceType.Group) {
                        SelectCharacterClientPeer peer;
                        if (mApplication.Clients.TryGetPeerForCharacterId(characterId, out peer)) {

                            DbPlayerCharactersObject player;
                            if (mApplication.Players.TryGetPlayer(peer.id, out player)) {
                                mApplication.Groups.HandleNotification(peer.id, characterId, player.Login, notification.data as Hashtable);
                            } else {
                                log.Info("player not found");
                            }
                        } else {
                            log.Info("peer not found");
                        }
                    }

                    notification.handled = true;
                    mCache.SetChanged(characterId, true);
                }
            } else {
                notifications.Remove(notificationId);
                mCache.SetChanged(characterId, true);
            }
        }


        public Notification Create(string uniqueID, string text, Hashtable data, NotficationRespondAction respondAction, NotificationSourceServiceType serviceType, NotificationSubType subType) {
            Notification notification = new Notification {
                uniqueID = uniqueID,
                data = data,
                handled = false,
                id = Guid.NewGuid().ToString(),
                respondAction = (int)respondAction,
                sourceServiceType = (int)serviceType,
                text = text,
                subType = (int)subType
            };
            return notification;
        }

        /// <summary>
        /// Set notification to character and send event to client if connected
        /// </summary>
        /// <param name="characterID">character id</param>
        /// <param name="notification">notification</param>
        public void SetNotificationToCharacter(string characterID, Notification notification ) {

            log.Info("start setting notification to player");

            //get notification from cache or database
            var notifications = GetNotifications(characterID);
            log.InfoFormat("current notifications count = {0}", notifications.notifications.Count);

            //if not found exit
            if(notifications == null) {
                log.InfoFormat("not found notifications for character = {0}", characterID);
                return;
            }

            if(notifications.Contains(notification)) {
                log.InfoFormat("SetNotificationToCharacter: notification with same unique ID already setted on character, duplicates not allowed [green]");
                return;
            }
            //add notfication to cached object
            notifications.Add(notification);
            //mark notfication for this character changed(for later savinf to database)
            mCache.SetChanged(characterID, true);

            log.InfoFormat("after setting notifications count = {0}", notifications.notifications.Count);

            //find peer for client (if such exists) or exit if peer with this character not founded
            SelectCharacterClientPeer peer = null;
            if(!mApplication.Clients.TryGetPeerForCharacterId(characterID, out peer)) {
                log.InfoFormat("event to client about notification not sended, peer not found = {0}", characterID);
                return;
            }

            if (peer != null) {
                //send event to client about new notification 
                EventData eventData = new EventData((byte)SelectCharacterEventCode.NotificationUpdate, new NotificationUpdateEvent { Notifications = notifications.GetInfo() });
                peer.SendEvent(eventData, new SendParameters());
                log.InfoFormat("sended event to peer");
            }
        }

        /// <summary>
        /// when client disconnect call this, save notficications for client and remove this from cache
        /// </summary>
        /// <param name="characterID"></param>
        public void OnDisconnect(string characterID ) {
            if(string.IsNullOrEmpty(characterID)) {
                log.Info("character ID must be not null");
                return;
            }
            mCache.ForceSave(characterID, mApplication.DB.Notifications);
            mCache.RemoveNotification(characterID);
        }

        public void SaveModified(MongoCollection<CharacterNotifications> collection) {
            mCache.DumpChangesToDatabase(collection);
        }
    }
}

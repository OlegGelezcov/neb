// CharacterNotifications.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, November 3, 2015 6:36:31 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using MongoDB.Bson;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace SelectCharacter.Notifications {
    public class CharacterNotifications : IInfoSource {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public Dictionary<string, Notification> notifications { get; set; }

        private readonly object syncRoot = new object();

        /// <summary>
        /// Look for notifiations already contains such notification or not (for comparing used unique ID status and handle state for responded notifications)
        /// </summary>
        /// <param name="n">Notification for comparing with existing notifications</param>
        /// <returns></returns>
        public bool Contains(Notification n) {
            lock(syncRoot) {
                foreach(var pNotification in notifications) {
                    if(pNotification.Value.uniqueID == n.uniqueID) {

                        //Comparing for handled notifications uses handled options
                        if (pNotification.Value.respondAction == (int)NotficationRespondAction.YesDelete && pNotification.Value.handled) {
                            return false;
                        } else {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public void Add(Notification notification) {
            lock(syncRoot) {
                notifications[notification.id] = notification;
            }
        }

        public void Remove(string nID) {
            lock(syncRoot) {
                if(notifications.ContainsKey(nID)) {
                    notifications.Remove(nID);
                }
            }
        }

        public Notification Get(string nID) {
            lock (syncRoot) {
                Notification result = null;
                notifications.TryGetValue(nID, out result);
                return result;
            }
        }

        public Hashtable GetInfo() {
            Hashtable result = new Hashtable {
                {(int)SPC.CharacterId, characterId},
            };
            Hashtable nHash = new Hashtable();
            lock (syncRoot) {
                
                foreach(var pair in notifications) {
                    nHash.Add(pair.Key, pair.Value.GetInfo());
                }
            }
            result.Add((int)SPC.Notifications, nHash);
            return result;
        }

    }
}

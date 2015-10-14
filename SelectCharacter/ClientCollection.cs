using Common;
using ExitGames.Logging;
using SelectCharacter.Chat;
using SelectCharacter.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SelectCharacter {
    public class ClientCollection : Dictionary<string, SelectCharacterClientPeer>{



        private readonly object syncRoot = new object();
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public bool OnConnect(SelectCharacterClientPeer clientPeer) {
            if(string.IsNullOrEmpty(clientPeer.id)) {
                throw new InvalidOperationException("client peer id not setted");
            }
            string key = clientPeer.id;
            lock(syncRoot) {
                SelectCharacterClientPeer peer;
                if(TryGetValue(key, out peer)) {
                    if(clientPeer != peer ) {
                        peer.Disconnect();
                        peer.RemoveClientPeerFromApplication();
                        Remove(key);
                    }
                }
                Log.InfoFormat("add client peer to collection = {0}", key);
                Add(key, clientPeer);
                return true;
            }

        }

        public void OnDisconnect(SelectCharacterClientPeer clientPeer) {
            if(string.IsNullOrEmpty(clientPeer.id)) {
                throw new InvalidOperationException("client peer id invalid");
            }
            string key = clientPeer.id;
            lock(syncRoot) {
                SelectCharacterClientPeer peer;
                if(TryGetValue(key, out peer)) {
                    if(peer == clientPeer) {
                        Log.InfoFormat("remove client peer from collection");
                        Remove(key);
                    }
                }
            }
        }

        public bool TryGetPeerForCharacterId(string characterID, out SelectCharacterClientPeer peer) {
            lock(syncRoot) {
                foreach(var pair in this) {
                    if(pair.Value.characterId == characterID) {
                        peer = pair.Value;
                        return true;
                    }
                }
                peer = null;
                return false;
            }
        }

        public bool TryGetPeerForGameRefId(string gameRedID, out SelectCharacterClientPeer peer) {
            lock(syncRoot) {
                return TryGetValue(gameRedID, out peer);
            }
        }

        public void SendChatMessageToWorld(string worldID, ChatMessage message) {
            lock(syncRoot) {
                foreach(var pair in this ) {
                    if(pair.Value.characterWorldID == worldID) {
                        pair.Value.SendChatMessage(message);
                    }
                }
            }
        }

        public void SendChatMessageToRace(Race race, ChatMessage message) {
            lock(syncRoot) {
                foreach(var pair in this) {
                    var peer = pair.Value;
                    if(peer.selectedCharacter != null && (peer.selectedCharacter.Race == (int)race)) {
                        peer.SendChatMessage(message);
                    }
                }
            }
        }

        public void SendNewCommanderElected(int race, string login) {
            lock(syncRoot) {
                foreach(var pair in this) {
                    pair.Value.SendNewCommaderElected(race, login);
                }
            }
        }

        public void SendChatMessageToGroup(string groupID, ChatMessage message) {
            lock(syncRoot) {
                foreach(var peerPair in this) {
                    if(peerPair.Value.groupID == groupID) {
                        peerPair.Value.SendChatMessage(message);
                    }
                }
            }
        }

        public void SendGenericEventToGameref(string gameRefID, GenericEvent evt) {
            SelectCharacterClientPeer peer;
            if(TryGetPeerForGameRefId(gameRefID, out peer)) {
                peer.SendGenericEvent(evt);
            }
        }
    }
}

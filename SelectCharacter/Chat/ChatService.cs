using Common;
using ExitGames.Logging;
using MongoDB.Driver;

namespace SelectCharacter.Chat {
    public class ChatService {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private SelectCharacterApplication mApplication;
        private ChatCache mCache;

        public ChatService(SelectCharacterApplication application) {
            mApplication = application;
            mCache = new ChatCache();
        }


        public void SendMessage(ChatMessage message) {
            switch((ChatGroup)message.chatGroup) {
                case ChatGroup.zone: {
                        SendChatEventToZone(message);
                        break;
                    }
                case ChatGroup.guild: {
                        SendChatMessageToGuild(message);
                        break;
                    }
                case ChatGroup.whisper: {
                        if (string.IsNullOrEmpty(message.targetCharacterID) && !string.IsNullOrEmpty(message.targetCharacterName)) {
                            SendChatMessageToCharacterName(message);
                        } else if (!string.IsNullOrEmpty(message.targetCharacterID) && string.IsNullOrEmpty(message.targetCharacterName)) {
                            SendChatMessageToPlayer(message);
                        }
                        break;
                    }
                case ChatGroup.group: {
                        SendChatMessageToGroup(message);
                        break;
                    }
                case ChatGroup.race:
                    {
                        SendChatMessageToRace(message);
                        break;
                    }
            }
        }

        private void SendChatMessageToGroup(ChatMessage message) {
            log.InfoFormat("SendChatMessageToGroup() called yellow");

            SelectCharacterClientPeer peer = null;
            if(!mApplication.Clients.TryGetPeerForCharacterId(message.sourceCharacterID, out peer)) {
                log.InfoFormat("Source message peer not founded yellow");
                return;
            }
            string groupID = peer.groupID;
            if(string.IsNullOrEmpty(groupID)) {
                log.InfoFormat("Send message to group = {0}. Group not founded.", groupID);
                return;
            }

            mApplication.Clients.SendChatMessageToGroup(groupID, message);

            mCache.AddMessage(message);

            log.InfoFormat("chat message sended to group = {0} yellow", groupID);
        }

        private void SendChatEventToZone(ChatMessage message) {
            
            SelectCharacterClientPeer sourcePeer = null;
            if(!mApplication.Clients.TryGetPeerForCharacterId(message.sourceCharacterID, out sourcePeer)) {
                log.Info("source peer not founded");
                return;
            }

            var sourcePlayer = mApplication.Players.GetExistingPlayerByLogin(message.sourceLogin);
            if(sourcePlayer == null) {
                log.Info("source player not founded");
                return;
            }

            var sourceCharacter = sourcePlayer.Data.GetCharacter(sourcePlayer.Data.SelectedCharacterId);
            if(sourceCharacter == null ) {
                log.Info("source character not founded");
                return;
            }

            if(string.IsNullOrEmpty(sourceCharacter.WorldId)) {
                log.Info("source character world invalid");
                return;
            }

            log.InfoFormat("send message to world = {0}", sourceCharacter.WorldId);

            mApplication.Clients.SendChatMessageToWorld(sourceCharacter.WorldId, message);
            mCache.AddMessage(message);
        }

        private void SendChatMessageToGuild(ChatMessage message) {
            SelectCharacterClientPeer sourcePeer = null;
            if (!mApplication.Clients.TryGetPeerForCharacterId(message.sourceCharacterID, out sourcePeer)) {
                log.Info("source peer not founded");
                return;
            }

            var sourcePlayer = mApplication.Players.GetExistingPlayerByLogin(message.sourceLogin);
            if (sourcePlayer == null) {
                log.Info("source player not founded");
                return;
            }

            var sourceCharacter = sourcePlayer.Data.GetCharacter(sourcePlayer.Data.SelectedCharacterId);
            if (sourceCharacter == null) {
                log.Info("source character not founded");
                return;
            }

            if(string.IsNullOrEmpty(sourceCharacter.guildID)) {
                log.Info("source character don't have guild");
                return;
            }

            var guild = mApplication.Guilds.GetGuild(sourceCharacter.guildID);

            if(guild == null ) {
                log.Info("guild not founded");
                return;
            }

            var characterIDs = guild.guildCharacters;

            foreach(var character in characterIDs) {
                SelectCharacterClientPeer peer = null;
                if(mApplication.Clients.TryGetPeerForCharacterId(character, out peer)) {
                    peer.SendChatMessage(message);
                }
            }
            mCache.AddMessage(message);
        }

        public void SendBroadcast(ChatMessage message) {
            mApplication.Clients.SendChatMessageToRace(Race.Humans, message);
            mApplication.Clients.SendChatMessageToRace(Race.Criptizoids, message);
            mApplication.Clients.SendChatMessageToRace(Race.Borguzands, message);
            mCache.AddMessage(message);
        }

        private void SendChatMessageToRace(ChatMessage message) {
            SelectCharacterClientPeer sourcePeer = null;
            if (!mApplication.Clients.TryGetPeerForCharacterId(message.sourceCharacterID, out sourcePeer)) {
                log.Info("[SendChatMessageToRace]: source peer not founded [purple]");
                return;
            }

            if(sourcePeer.selectedCharacter == null) {
                log.InfoFormat("[SendChatMessageToRace]: not found selected character at peer [purple]");
                return;
            }

            mApplication.Clients.SendChatMessageToRace((Race)(byte)sourcePeer.selectedCharacter.Race, message);
            mCache.AddMessage(message);
        }

        private void SendChatMessageToCharacterName(ChatMessage message) {
            SelectCharacterClientPeer sourcePeer = null;
            if (!mApplication.Clients.TryGetPeerForCharacterId(message.sourceCharacterID, out sourcePeer)) {
                log.Info("source peer not founded");
                return;
            }

            var sourcePlayer = mApplication.Players.GetExistingPlayerByLogin(message.sourceLogin);
            if (sourcePlayer == null) {
                log.Info("source player not founded");
                return;
            }

            var sourceCharacter = sourcePlayer.Data.GetCharacter(sourcePlayer.Data.SelectedCharacterId);
            if (sourceCharacter == null) {
                log.Info("source character not founded");
                return;
            }

            if(string.IsNullOrEmpty(message.targetCharacterName)) {
                log.InfoFormat("target character name not setted");
                return;
            }

            var targetPlayer = mApplication.Players.GetCachePlayerByCharacterName(message.targetCharacterName);
            if(targetPlayer == null ) {
                log.InfoFormat("player with such character name not cached");
                return;
            }

            SelectCharacterClientPeer targetPeer;
            if(false == mApplication.Clients.TryGetPeerForCharacterId(targetPlayer.Data.SelectedCharacterId, out targetPeer)) {
                log.InfoFormat("target peer not found");
                return;
            }

            message.targetCharacterID = targetPlayer.Data.SelectedCharacterId;
            message.targetLogin = targetPlayer.Data.Login;

            sourcePeer.SendChatMessage(message);
            targetPeer.SendChatMessage(message);
            mCache.AddMessage(message);
        }

        private void SendChatMessageToPlayer(ChatMessage message) {
            SelectCharacterClientPeer sourcePeer = null;
            if (!mApplication.Clients.TryGetPeerForCharacterId(message.sourceCharacterID, out sourcePeer)) {
                log.Info("source peer not founded");
                return;
            }

            var sourcePlayer = mApplication.Players.GetExistingPlayerByLogin(message.sourceLogin);
            if (sourcePlayer == null) {
                log.Info("source player not founded");
                return;
            }

            var sourceCharacter = sourcePlayer.Data.GetCharacter(sourcePlayer.Data.SelectedCharacterId);
            if (sourceCharacter == null) {
                log.Info("source character not founded");
                return;
            }

            SelectCharacterClientPeer targetPeer;
            if(!mApplication.Clients.TryGetPeerForCharacterId(message.targetCharacterID, out targetPeer)) {
                log.Info("target character not founded");
                return;
            }

            sourcePeer.SendChatMessage(message);
            targetPeer.SendChatMessage(message);
            mCache.AddMessage(message);
        }

        public void DumpToDatabase(MongoCollection<ChatMessage> collection ) {
            mCache.DumpCache(collection);
        }
    }
}

using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Chat {
    public class ChatCache {

        private readonly ConcurrentDictionary<string, ChatMessage> mMessages;

        public ChatCache() {
            mMessages = new ConcurrentDictionary<string, ChatMessage>();
        }

        public void AddMessage(ChatMessage message) {
            if(!mMessages.ContainsKey(message.messageID)) {
                mMessages.TryAdd(message.messageID, message);
            }
        }

        public void DumpCache(MongoCollection<ChatMessage> collection) {
            foreach(var pair in mMessages) {
                collection.Save(pair.Value);
            }
            mMessages.Clear();
        }

        public bool Contains(string messageID) { return mMessages.ContainsKey(messageID); }
    }
}

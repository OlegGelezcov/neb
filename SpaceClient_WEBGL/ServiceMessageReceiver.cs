using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ServiceMessageReceiver : IServiceMessageReceiver {
        private readonly int maxMessages;
        private List<IServiceMessage> messages;
        private readonly DateTime mStartTime;
        private List<int> mExpiredMessages = new List<int>();

        public ServiceMessageReceiver(int maxMessages) {
            this.maxMessages = maxMessages;
            messages = new List<IServiceMessage>();
            mStartTime = DateTime.UtcNow;
        }

        public void AddMessage(Hashtable messageTable) {
            if (maxMessages == 0)
                return;

            while (messages.Count >= maxMessages) {
                messages.RemoveAt(0);
            }

            //messages.Insert(0, ParseMessage(messageTable));
            messages.Add(ParseMessage(messageTable));
        }

        public IServiceMessage[] RecentMessages(int count) {
            int validCount = Math.Min(count, messages.Count);
            IServiceMessage[] result = new IServiceMessage[validCount];
            for (int i = 0; i < validCount; i++) {
                result[i] = messages[messages.Count - validCount + i];
            }
            return result;
        }

        public IServiceMessage[] Messages {
            get {
                return messages.ToArray();
            }
        }

        public void Clear() {
            messages.Clear();
        }

        public int Count {
            get {
                return messages.Count;
            }
        }

        public int MaxCount {
            get {
                return maxMessages;
            }
        }

        private IServiceMessage ParseMessage(Hashtable messageTable) {
            ServiceMessageType type = messageTable.GetValueByte((int)SPC.Type, ServiceMessageType.Info.toByte()).toEnum<ServiceMessageType>();
            string message = messageTable.GetValueString((int)SPC.Message);
            return new ServiceMessage(type, message);
        }




        public IServiceMessage[] MessagesAtIndex(int index) {
            if (this.messages.Count == 0)
                return new IServiceMessage[] { };

            if (index < 0)
                index = 0;
            if (index >= this.messages.Count)
                index = this.messages.Count - 1;

            return this.messages.Skip(index).ToArray();
        }

        public void RemoveExpiredMessages() {
            mExpiredMessages.Clear();
            for (int i = 0; i < messages.Count; i++) {
                float dt = (float)(DateTime.UtcNow - ((ServiceMessage)messages[i]).time).TotalSeconds;
                if (dt > 10f) {
                    mExpiredMessages.Add(i);
                }
            }

            foreach (int index in mExpiredMessages) {
                messages.RemoveAt(index);
            }
            mExpiredMessages.Clear();
        }
    }
}

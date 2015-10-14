using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;


namespace Space.Game.Player
{
    public class ChatManager  : IInfo
    {
        private List<ChatMessageData> messages;
        private int maxMessages;

        public ChatManager(int maxMessages)
        {
            this.maxMessages = maxMessages;
            this.messages = new List<ChatMessageData>();
        }

        public bool AddMessage(Hashtable info )
        {
            if(this.messages.Count < maxMessages )
            {
                ChatMessageData msg = new ChatMessageData(info);

                if (this.Contains(msg.Id))
                    return false;

                this.messages.Add(msg);
                return true;
            }
            return false;
        }

        public bool Contains(string id)
        {
            var result = this.messages.Find(m => m.Id == id);
            if (result == null)
                return false;
            else
                return true;
        }


        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            foreach(var msg in this.messages )
            {
                info.Add(msg.Id, msg.GetInfo());
            }
            return info;
        }

        public void ParseInfo(Hashtable info)
        {
        }

        public void Clear()
        {
            this.messages.Clear();
        }
    }
}

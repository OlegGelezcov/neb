using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Space.Game.Player
{
    public class ChatMessageData : IInfo
    {
        public string Id { get; private set; }
        public ChatGroup Group { get; private set; }
        public string Message { get; private set; }
        public string Sender { get; private set; }
        public string SenderName { get; private set; }

        public string Receiver { get; private set; }

        public float Time { get; private set; }

        public ChatMessageData(Hashtable info )
        {
            this.ParseInfo(info);
        }

        public System.Collections.Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.ChatMessageId, this.Id);
            info.Add((int)SPC.ChatMessageGroup, this.Group.toByte());
            info.Add((int)SPC.ChatMessage, this.Message);
            info.Add((int)SPC.ChatSourceLogin, this.Sender);
            info.Add((int)SPC.ChatMessageSourceName, this.SenderName);
            info.Add((int)SPC.ChatReceiverLogin, this.Receiver);
            info.Add((int)SPC.ChatMessageTime, this.Time);
            return info;
        }

        public void ParseInfo(System.Collections.Hashtable info)
        {
            this.Id = info.GetValue<string>((int)SPC.ChatMessageId, string.Empty);
            this.Group = (ChatGroup)info.GetValue<byte>((int)SPC.ChatMessageGroup, (byte)0);
            this.Message = info.GetValue<string>((int)SPC.ChatMessage, string.Empty);
            this.Sender = info.GetValue<string>((int)SPC.ChatSourceLogin, string.Empty);
            this.SenderName = info.GetValue<string>((int)SPC.ChatMessageSourceName, string.Empty);
            this.Receiver = info.GetValue<string>((int)SPC.ChatReceiverLogin, string.Empty);
            this.Time = info.GetValue<float>((int)SPC.ChatMessageTime, 0.0f);
        }
    }
}

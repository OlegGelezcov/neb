using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Mail {
    public class MailMessage : IInfoParser {

        public string id { get; private set; }
        public string title { get; private set; }
        public string body { get; private set; }
        public string senderGameRefId { get; private set; }
        public string receiverGameRefId { get; private set; }
        public string senderLogin { get; private set; }
        public DateTime time { get; private set; }
        public Dictionary<string, MailAttachment> attachments { get; private set; }

        public void ParseInfo(Hashtable info) {
            id = info.Value<string>((int)SPC.Id);
            title = info.Value<string>((int)SPC.Title);
            body = info.Value<string>((int)SPC.Body);
            senderGameRefId = info.Value<string>((int)SPC.SenderId);
            receiverGameRefId = info.Value<string>((int)SPC.ReceiverId);


            time = DateTime.Parse(info.Value<string>((int)SPC.Time));
            senderLogin = info.Value<string>((int)SPC.SenderName);

            attachments = new Dictionary<string, MailAttachment>();
            Hashtable attachmentHash = info.Value<Hashtable>((int)SPC.Attachments);
            foreach(DictionaryEntry attachmentEntry in attachmentHash) {
                Hashtable attachmentInfo = attachmentEntry.Value as Hashtable;
                if(attachmentInfo == null ) {
                    throw new FormatException(string.Format("attachment info must be HashTable"));
                }
                attachments.Add(attachmentEntry.Key.ToString(), new MailAttachment(attachmentInfo));
            }
        }

        public MailMessage(Hashtable mailMessageHash) {
            ParseInfo(mailMessageHash);
        }

    }
}

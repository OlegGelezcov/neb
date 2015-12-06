using Common;
using System;
using System.Collections.Generic;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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

        public void ReplaceTitle(string newTitle) {
            title = newTitle;
        }

        public void ReplaceBody(string newBody) {
            body = newBody;
        }

        public void ParseInfo(Hashtable info) {
            id = info.GetValueString((int)SPC.Id);
            title = info.GetValueString((int)SPC.Title);
            body = info.GetValueString((int)SPC.Body);
            senderGameRefId = info.GetValueString((int)SPC.SenderId);
            receiverGameRefId = info.GetValueString((int)SPC.ReceiverId);


            time = DateTime.Parse(info.GetValueString((int)SPC.Time));
            senderLogin = info.GetValueString((int)SPC.SenderName);

            attachments = new Dictionary<string, MailAttachment>();
            Hashtable attachmentHash = info.GetValueHash((int)SPC.Attachments);
            foreach (System.Collections.DictionaryEntry attachmentEntry in attachmentHash) {
                Hashtable attachmentInfo = attachmentEntry.Value as Hashtable;
                if (attachmentInfo == null) {
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

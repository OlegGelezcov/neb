namespace SelectCharacter.Mail {
    using Common;
    using MongoDB.Bson;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class MailBox : IInfoSource {

        public const int MAIL_EXPIRE_INTERVAL = 30 * 24 * 60 * 60;

        public ObjectId Id { get; set; }

        public string gameRefId { get; set; }

        public Dictionary<string, MailMessage> messages { get; set; }

        public int newMessagesCount { get; set; }

        public void IncrementNewMessages() {
            newMessagesCount++;
        }

        public void ResetNewMessages() {
            newMessagesCount = 0;
        }

        public MailMessage AddNewMessage(MailMessage message) {
            if(!messages.ContainsKey(message.id)) {
                messages.Add(message.id, message);
                IncrementNewMessages();
            }
            return message;
        }

        public bool DeleteMessage(string id) {
            return messages.Remove(id);
        }

        public Hashtable GetInfo() {
            Hashtable mailsHash = new Hashtable();
            if (messages != null) {
                foreach (var kv in messages) {
                    mailsHash.Add(kv.Key, kv.Value.GetInfo());
                }
            }
            return mailsHash;
        }

        public void ClearExpiredMails() {
            List<string> expiredIds = new List<string>();
            foreach(var kMail in messages) {
                var dt = DateTime.Parse(kMail.Value.time, System.Globalization.CultureInfo.InvariantCulture);
                if((DateTime.UtcNow - dt).TotalSeconds > MAIL_EXPIRE_INTERVAL) {
                    expiredIds.Add(kMail.Key);
                }
            }

            foreach(var id in expiredIds) {
                messages.Remove(id);
            }
        }

        public bool TryGetAttachment(string messageId, string attachmentId, out MailAttachment attachment) {
            MailMessage message;
            if(messages.TryGetValue(messageId, out message)) {
                if(message.TryGetAttachment(attachmentId, out attachment)) {
                    return true;
                }
            }
            attachment = null;
            return false;
        }

        public bool RemoveAttachment(string messageId, string attachmentId ) {
            MailMessage message;
            if(messages.TryGetValue(messageId, out message)) {
                return message.RemoveAttachment(attachmentId);
            }
            return false;
        }
    }
}

namespace SelectCharacter.Mail {
    using Common;
    using ServerClientCommon;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class MailMessage  : IInfoSource {
        public string id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string sendefGameRefId { get; set; }
        public string receiverGameRefId { get; set; }
        public string senderLogin { get; set; }
        public string time { get; set; }

        public Dictionary<string, MailAttachment> attachments { get; set; }


        public MailAttachment AddAttachment(Hashtable objectHash, int count ) {
            MailAttachment newAttachment = new MailAttachment { id = Guid.NewGuid().ToString(), objectHash = objectHash, count = count };
            attachments.Add(newAttachment.id, newAttachment);
            return newAttachment;
        }

        public bool RemoveAttachment(string id) {
            return attachments.Remove(id);
        }

        public bool TryGetAttachment(string id, out MailAttachment attach) {
            return attachments.TryGetValue(id, out attach);
        }

        public void ClearAttachments() {
            if (attachments == null)
                attachments = new Dictionary<string, MailAttachment>();

            attachments.Clear();
        }


        public Hashtable GetInfo() {
            Hashtable attachmentsHash = new Hashtable();
            if(attachments != null ) {
                foreach(var kv in attachments) {
                    attachmentsHash.Add(kv.Key, kv.Value.GetInfo());
                }
            }

            return new Hashtable {
                {(int)SPC.Id, id },
                {(int)SPC.Title, title  },
                {(int)SPC.Body, body },
                {(int)SPC.SenderId, sendefGameRefId  },
                {(int)SPC.ReceiverId, receiverGameRefId  },
                {(int)SPC.Time, time },
                {(int)SPC.SenderName, senderLogin },
                {(int)SPC.Attachments, attachmentsHash}
            };
        }
    }
}

namespace SelectCharacter.Mail {
    using Common;
    using System.Collections;
    using System;
    using ServerClientCommon;

    public class MailAttachment : IInfoSource {
        public string id { get; set; }
        public Hashtable objectHash { get; set; }
        public int count { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, id },
                { (int)SPC.AttachedObject, objectHash },
                { (int)SPC.Count, count}
            };
        }
    }
}

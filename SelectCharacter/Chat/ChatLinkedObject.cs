using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Chat {
    /// <summary>
    /// Link in chat
    /// </summary>
    public class ChatLinkedObject : IInfo {
        /// <summary>
        /// Unique id of link, unique in scope of message
        /// </summary>
        public int linkID { get; set; }
        /// <summary>
        /// Display text of link
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// Linked object info
        /// </summary>
        public Hashtable objectInfo { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, linkID },
                { (int)SPC.DisplayName, displayName },
                { (int)SPC.AttachedObject, objectInfo }
            };
        }

        public void ParseInfo(Hashtable info) {
            linkID = info.Value<int>((int)SPC.Id);
            displayName = info.Value<string>((int)SPC.DisplayName);
            objectInfo = info.Value<Hashtable>((int)SPC.AttachedObject);
        }
    }
}

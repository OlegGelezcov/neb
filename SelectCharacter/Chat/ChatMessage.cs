using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Chat {
    public class ChatMessage {
        /// <summary>
        /// Database ID
        /// </summary>
        public ObjectId Id { get; set; }

        public string messageID { get; set; }
        /// <summary>
        /// Login of sender
        /// </summary>
        public string sourceLogin { get; set; }
        /// <summary>
        /// Character ID of sender
        /// </summary>
        public string sourceCharacterID { get; set; }
        /// <summary>
        /// ID of receiver ( only for whisper ) else empty
        /// </summary>
        public string targetLogin { get; set; }
        /// <summary>
        /// Character ID of receiver ( only for whisper ) else empty
        /// </summary>
        public string targetCharacterID { get; set; }
        /// <summary>
        /// Chat group defines destination of message
        /// </summary>
        public int chatGroup { get; set; }
        /// <summary>
        /// Message text
        /// </summary>
        public string message { get; set; }

        public string sourceCharacterName { get; set; }
        /// <summary>
        /// List of chat links
        /// </summary>
        public List<ChatLinkedObject> links { get; set; }
    }
}

using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Events {
    public class ChatMessageEvent {
        [DataMember(Code =(byte)ParameterCode.MessageId)]
        public string MessageID { get; set; }

        [DataMember(Code =(byte)ParameterCode.SourceLogin)]
        public string SourceLogin { get; set; }

        [DataMember(Code =(byte)ParameterCode.SourceCharacterId)]
        public string SourceCharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.TargetLogin)]
        public string TargetLogin { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId)]
        public string CharacterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.Group)]
        public int ChatGroup { get; set; }

        [DataMember(Code =(byte)ParameterCode.Body)]
        public string Message { get; set; }

        [DataMember(Code =(byte)ParameterCode.Attachments)]
        public object[] Links { get; set; }

        [DataMember(Code = (byte)ParameterCode.DisplayName)]
        public string sourceCharacterName { get; set; }
    }
}

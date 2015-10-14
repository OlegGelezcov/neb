//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Common;
//using System.Collections.Generic;
//using System.Collections;
//using ServerClientCommon;

//namespace Space.Game.Groups {
//    public class GroupActionRequest : IInfoSource {
//        private GroupActionRequestType requestType;
//        private string requestId;
//        private bool hasRequest;
//        private Hashtable requestData;

//        public GroupActionRequest() {
//            this.ClearRequest();
//        }

//        public bool SetRequest(GroupActionRequestType reqType, string reqId, Hashtable requestData) {
//            if (HasRequest()) {
//                return false;
//            }

//            this.requestType = reqType;
//            this.requestId = reqId;
//            this.requestData = requestData;

//            if (this.requestType == GroupActionRequestType.None || string.IsNullOrEmpty(requestId) || (this.requestData == null)) {
//                this.hasRequest = false;
//            } else {
//                this.hasRequest = true;
//            }
//            return HasRequest();
//        }

//        public void ClearRequest() {
//            this.requestType = GroupActionRequestType.None;
//            this.requestId = string.Empty;
//            this.requestData = null;
//            this.hasRequest = false;
//        }

//        public bool HasRequest() {
//            return this.hasRequest;
//        }

//        public GroupActionRequestType RequestType() {
//            return this.requestType;
//        }

//        public string RequestId() {
//            return this.requestId;
//        }

//        public Hashtable Data() {
//            return this.requestData;
//        }





//        public Hashtable GetInfo() {
//            return new Hashtable{
//                {(int)SPC.RequestType, (byte)this.requestType },
//                {(int)SPC.Id, this.requestId },
//                {(int)SPC.Data, this.requestData }
//            };
//        }

//        public static Hashtable InviteToNewGroupRequestData(string itemId, string displayName) {
//            return new Hashtable{
//                {(int)SPC.Id, itemId },
//                {(int)SPC.DisplayName, displayName }
//            };
//        }

//        public static Hashtable ExcludeFromGroupRequestData(string fromCharacterId, string fromDisplayName, string toExcludeCharacterId, string toExcludeDisplayName) {
//            return new Hashtable {
//                {(int)SPC.FromCharacterId, fromCharacterId },
//                {(int)SPC.FromDisplayName, fromDisplayName },
//                {(int)SPC.ToExcludeCharacterId, toExcludeCharacterId },
//                {(int)SPC.ToExcludeDisplayName, toExcludeDisplayName }
//            };
//        }
//    }
//}

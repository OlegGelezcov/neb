using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.News {
    public class PostEntry : IInfoParser {

        public string postID { get; private set; }
        public DateTime time { get; private set; }
        public string message { get; private set; }
        public string lang { get; private set; }
        public string postURL { get; private set; }
        public string imageURL { get; private set; }

        public void ParseInfo(Hashtable info) {
            int iTime = info.GetValue<int>((int)SPC.Time, 0);
            time = CommonUtils.START_DATE.AddSeconds(iTime);
            message = info.GetValue<string>((int)SPC.Message, string.Empty);
            postURL = info.GetValue<string>((int)SPC.PostURL, string.Empty);
            lang = info.GetValue<string>((int)SPC.Lang, string.Empty);
            imageURL = info.GetValue<string>((int)SPC.ImageURL, string.Empty);
            postID = info.GetValue<string>((int)SPC.Id, string.Empty);
        }

        public PostEntry(string inPostId, int iTimeIn, string inMessage, string inLang, string inPostUrl, string inImageUrl ) {
            postID = inPostId;
            time = CommonUtils.START_DATE.AddSeconds(iTimeIn);
            message = inMessage;
            lang = inLang;
            postURL = inPostUrl;
            imageURL = inImageUrl;
        }

        public PostEntry(Hashtable info) {
            ParseInfo(info);
        }

        public PostEntry() { }

        public bool hasURL {
            get {
                return (false == string.IsNullOrEmpty(postURL));
            }
        }

        public bool hasImage {
            get {
                return (false == string.IsNullOrEmpty(imageURL));
            }
        }
    }
}

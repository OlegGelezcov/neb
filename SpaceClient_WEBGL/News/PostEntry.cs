using Common;
using System;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.News {
    public class PostEntry : IInfoParser {

        public string postID { get; private set; }
        public DateTime time { get; private set; }
        public string message { get; private set; }
        public string lang { get; private set; }
        public string postURL { get; private set; }
        public string imageURL { get; private set; }

        public void ParseInfo(Hashtable info) {
            int iTime = info.GetValueInt((int)SPC.Time);
            time = CommonUtils.START_DATE.AddSeconds(iTime);
            message = info.GetValueString((int)SPC.Message);
            postURL = info.GetValueString((int)SPC.PostURL);
            lang = info.GetValueString((int)SPC.Lang);
            imageURL = info.GetValueString((int)SPC.ImageURL);
            postID = info.GetValueString((int)SPC.Id);
        }

        public PostEntry(string inPostId, int iTimeIn, string inMessage, string inLang, string inPostUrl, string inImageUrl) {
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

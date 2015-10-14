using Common;
using MongoDB.Bson;
using ServerClientCommon;
using System.Collections;

namespace Master.News {
    public class PostEntry : IInfoSource {
        public ObjectId Id { get; set; }

        public string postID { get; set; }
        public int time { get; set; }
        public string message { get; set; }
        public string lang { get; set; }
        public string postURL { get; set; }
        public string imageURL { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Time, time },
                { (int)SPC.Message, message },
                { (int)SPC.Lang, lang },
                { (int)SPC.PostURL, postURL },
                { (int)SPC.ImageURL, imageURL },
                { (int)SPC.Id, postID }
            };
        }

        public override string ToString() {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine(string.Format("ID: {0}", postID));
            builder.AppendLine(string.Format("TIME: {0}", time));
            builder.AppendLine(string.Format("LANG: {0}", lang));
            builder.AppendLine(string.Format("MESSAGE: {0}", message));
            builder.AppendLine(string.Format("URL: {0}", postURL));
            builder.AppendLine(string.Format("IMAGE URL: {0}", imageURL));
            return builder.ToString();
        }
    }
}

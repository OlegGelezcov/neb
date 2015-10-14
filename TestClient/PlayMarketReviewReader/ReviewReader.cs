using Nebula.Client.MiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.PlayMarketReviewReader {
    public class ReviewReader {
        public List<ReviewEntry> reviews { get; private set; }

        public ReviewReader(string text) {
            reviews = new List<ReviewEntry>();
            List<object> list = Json.Deserialize(text) as List<object>;
            foreach(var obj in list) {
                Dictionary<string, object> dict = obj as Dictionary<string, object>;

                string sRating = dict["rating"].ToString();
                string sTitle = dict["title"].ToString();
                string sText = dict["text"].ToString();
                reviews.Add(new ReviewEntry(int.Parse(sRating), sTitle, sText));
            }
        }

        public static ReviewReader Create(string appID = "com.gameloft.android.ANMP.GloftO2HM") {
            string url = "http://api.viewreview.org/json/app?id=" + appID;
            WebRequest request = WebRequest.Create(url);
            Stream objStrem = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(objStrem);
            string text = reader.ReadToEnd();
            reader.Close();
            objStrem.Close();
            return new ReviewReader(text);

        }
    }

    public class ReviewEntry {
        public int rating { get; private set; }
        public string title { get; private set; }
        public string text { get; private set; }

        public ReviewEntry(int rating, string title, string text) {
            this.rating = rating;
            this.title = title;
            this.text = text;
        }
    }
}

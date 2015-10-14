using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Nebula {
    public static class NetUtils {
        public static string SendToSlack(string server, string message) {
            string msg = string.Format("{2}:{0}:{1}", server, message, DateTime.UtcNow.ToString());

            var request = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");
            string posdata = "payload={\"channel\": \"#nebula\", \"username\": \"Borguzand\", \"text\": \"" + msg + "\"}";
            Console.WriteLine(posdata);

            var data = Encoding.ASCII.GetBytes(posdata);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream()) {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
    }
}

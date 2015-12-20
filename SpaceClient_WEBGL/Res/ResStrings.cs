using Common;
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResStrings {

        public static List<StringData> LoadData(string xml, string language) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            return document.Element("strings").Elements("string").Select(sElement => {
                string key = sElement.GetString("key");
                string value = string.Empty;
                if (language == "ru") {
                    value = sElement.GetString("ru");
                } else {
                    value = sElement.GetString("en");
                }
                return new StringData(key, value);
            }).ToList();
        }
    }

    public class StringData {
        public readonly string key;
        public readonly string value;

        public StringData(string inKey, string inValue) {
            key = inKey;
            value = inValue;
        }
    }
}

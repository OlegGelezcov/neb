namespace Nebula.Resources {
    using System;
    using System.Collections.Generic;
    public static class ObjectIcons {
        private static List<ResObjectIconData> _objectIcons;

        public static List<ResObjectIconData> LoadObjectIcons(string text) {
            sXDocument document = new sXDocument();
            document.ParseXml(text);
            List<ResObjectIconData> result = new List<ResObjectIconData>();
            foreach (var elem in document.GetElement("object_icons").GetElements("icon")) {
                result.Add(new ResObjectIconData {
                    Id = elem.GetString("id"),
                    Path = elem.GetString("path"),
                    Type = (IconType)Enum.Parse(typeof(IconType), elem.GetString("type"))
                });
            }
            return result;
        }

    }
}

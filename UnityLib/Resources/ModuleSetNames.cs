namespace Nebula.Resources {
    using System.Collections.Generic;
    public static class ModulesSetNames {
        public static Dictionary<string, string> LoadModulesSetNames(string text) {
            sXDocument document = new sXDocument();
            document.ParseXml(text);
            Dictionary<string, string> setNames = new Dictionary<string, string>();
            foreach (var elem in document.GetElement("set_names").GetElements("set")) {
                setNames.Add(elem.GetString("id"), elem.GetString("name"));
            }

            return setNames;
        }

    }
}

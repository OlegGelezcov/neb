
namespace Nebula.Client.Res
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;


    public class ResHelp
    {
        private List<ResHelpElement> helpElements;

        public void Load(string xml)
        {
            XDocument document = XDocument.Parse(xml);
            this.helpElements = document.Element("help").Elements("h").Select(e =>
                {
                    string icon = string.Empty;
                    if (e.Attribute("icon") != null)
                    {
                        icon = e.Attribute("icon").Value;
                    }
                    string text = e.Value.Trim();
                    text = text.Replace('{', '<').Replace('}', '>');
                    return new ResHelpElement(text, icon);
                }).ToList();
        }

        public List<ResHelpElement> Elements()
        {
            return this.helpElements;
        }
    }
}

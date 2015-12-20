
namespace Nebula.Client.Res
{
    using System.Collections.Generic;
    using System.Linq;
#if UP
    using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif


    public class ResHelp
    {
        private List<ResHelpElement> helpElements;

        public void Load(string xml)
        {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
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

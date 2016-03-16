namespace Nebula.Client.UI {

    public class TextComposeData : ComposeElementData {

        private string m_Text;
        private ComposeTextSize m_Size;

        public TextComposeData(UniXMLElement element)
            : base(element) {
            m_Text = element.innerValue.Trim();

            if(element.HasAttribute("size")) {
                m_Size = (ComposeTextSize)System.Enum.Parse(typeof(ComposeTextSize), element.GetString("size"));
            } else {
                m_Size = ComposeTextSize.medium;
            }
        }

        public string text {
            get {
                return m_Text;
            }
        }

        public ComposeTextSize size {
            get {
                return m_Size;
            }
        }

        public override ComposeElementType type {
            get {
                return ComposeElementType.text;
            }
        }
    }
}

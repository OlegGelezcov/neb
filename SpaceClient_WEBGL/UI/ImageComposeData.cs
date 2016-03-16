namespace Nebula.Client.UI {
    public class ImageComposeData : ComposeElementData {

        private string m_Path;
        private int m_Width;
        private int m_Height;


        public ImageComposeData(UniXMLElement element)
            : base(element) {

            if(element.HasAttribute("path")) {
                m_Path = element.GetString("path");
            } else {
                m_Path = string.Empty;
            }

            if(element.HasAttribute("width")) {
                m_Width = element.GetInt("width");
            } else {
                m_Width = 0;
            }

            if(element.HasAttribute("height")) {
                m_Height = element.GetInt("height");
            } else {
                m_Height = 0;
            }

        }

        public override ComposeElementType type {
            get {
                return ComposeElementType.image;
            }
        }

        public string path {
            get {
                return m_Path;
            }
        }

        public int width {
            get {
                return m_Width;
            }
        }

        public int height {
            get {
                return m_Height;
            }
        }
    }
}

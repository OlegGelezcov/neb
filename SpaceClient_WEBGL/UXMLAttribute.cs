namespace Nebula.Client {
    public class UXMLAttribute {
        public string name { get; private set; }
        public string value { get; private set; }

        public UXMLAttribute(string name, string value) {
            this.name = name;
            this.value = value;
        }

        public UXMLAttribute(string name, int value)
            : this(name, value.ToString()) { }

        public UXMLAttribute(string name, float value)
            : this(name, value.ToString(System.Globalization.CultureInfo.InvariantCulture)) { }

        public UXMLAttribute(string name, bool value)
            : this(name, value.ToString()) { }

    }
}

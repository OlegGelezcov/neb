namespace Nebula.Client.Manual {
    public class ManStageData {
        public int stage { get; private set; }
        public string textId { get; private set; }
        public string eventName { get; private set; }
        public string hintId { get; private set; }
        public string statName { get; private set; }

        public ManStageData(UniXMLElement element ) {
            stage = element.GetInt("id");
            textId = element.GetString("text");
            eventName = element.GetString("event");
            hintId = element.GetString("hint");
            statName = element.HasAttribute("statname") ? element.GetString("statname") : string.Empty;
        }
    }
}

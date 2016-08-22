namespace Nebula.Client.Dialogs {

    /// <summary>
    /// Represent single topic of dialog
    /// </summary>
    public class DialogTopic {
        /// <summary>
        /// Topic text
        /// </summary>
        public string text { get; private set; }

        /// <summary>
        /// Voice id of topic
        /// </summary>
        public string voice { get; private set; }

        /// <summary>
        /// Character id was topic starter
        /// </summary>
        public string character { get; private set; }

        public DialogTopic() {
            text = string.Empty;
            voice = string.Empty;
            character = string.Empty;
        }

        public DialogTopic(string character, string text, string voice) {
            this.character = character;
            this.text = text;
            this.voice = voice;
        }

        public bool hasCharacter {
            get {
                return !string.IsNullOrEmpty(character);
            }
        }

        public bool hasVoice {
            get {
                return !string.IsNullOrEmpty(voice);
            }
        }
    }
}

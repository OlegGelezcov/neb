namespace Nebula.Client.Res {
    /// <summary>
    /// Data for contract inventory item
    /// </summary>
    public class ContractItemData {
        /// <summary>
        /// Item id
        /// </summary>
        public string id { get; private set; }
        /// <summary>
        /// Item name string key
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// Item description string key
        /// </summary>
        public string description { get; private set; }
        /// <summary>
        /// Item icon path
        /// </summary>
        public string icon { get; private set; }

        public ContractItemData(UniXMLElement element ) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("description");
            icon = element.GetString("icon");
        }
    }
}

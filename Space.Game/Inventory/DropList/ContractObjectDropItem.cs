namespace Nebula.Inventory.DropList {
    public class ContractObjectDropItem : DropItem {
        private string m_TemplateId;
        private string m_ContractId;

        public ContractObjectDropItem(string templateId, string contractId,  int min, int max, float prob)
            : base(min, max, prob, Common.InventoryObjectType.contract_item) {
            m_TemplateId = templateId;
            m_ContractId = contractId;
        }
        public string templateId {
            get {
                return m_TemplateId;
            }
        }

        public string contractId {
            get {
                return m_ContractId;
            }
        }
    }
}

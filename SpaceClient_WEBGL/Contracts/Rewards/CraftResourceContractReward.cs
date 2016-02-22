namespace Nebula.Client.Contracts.Rewards {
    public class CraftResourceContractReward : ContractReward {
        public int count { get; private set; }
        public CraftResourceContractReward(UniXMLElement element)
            : base(element) {
            count = element.GetInt("count");
        }
    }
}

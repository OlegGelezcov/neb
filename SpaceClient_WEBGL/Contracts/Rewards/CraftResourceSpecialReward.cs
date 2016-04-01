namespace Nebula.Client.Contracts.Rewards {
    public class CraftResourceSpecialReward : ContractReward {

        public int count { get; private set; }
        public string id { get; private set; }

        public CraftResourceSpecialReward(UniXMLElement element) 
            : base(element) {
            count = element.GetInt("count");
            id = element.GetString("id");
        }


    }
}

namespace Nebula.Contracts.Inventory {
    /// <summary>
    /// Interface for receiving resource contract items
    /// </summary>
    public interface IContractItemCollection {
        ContractItemDataCollection contractItems { get; }
    }
}

using ExitGames.Client.Photon;


namespace Common {
    public interface IInventoryObjectBase : IInfo, IPlacingType {
        string Id { get; }
        InventoryObjectType Type { get; }
        Hashtable rawHash { get; }
        bool isNew { get; }
    }
}

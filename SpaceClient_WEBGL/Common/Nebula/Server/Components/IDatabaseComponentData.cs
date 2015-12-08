using ExitGames.Client.Photon;

namespace Nebula.Server.Components {
    /// <summary>
    /// Component which saved self to database
    /// </summary>
    public interface IDatabaseComponentData {
        Hashtable AsHash();
    }
}

using ExitGames.Client.Photon;
using ServerClientCommon;
using Nebula.Client.Utils;

namespace Nebula.Client.Store {
    public class IDItemPrice : ItemPrice {

        public string id { get; private set; }

        public IDItemPrice(Hashtable hash) : base(hash) {
            id = hash.GetValueString((int)SPC.Id);
        }
    }
}

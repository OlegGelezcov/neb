using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Store {
    public class ColoredItemPrice : ItemPrice {

        public ObjectColor color { get; private set; }

        public ColoredItemPrice(Hashtable hash) : base(hash) {
            color = (ObjectColor)(byte)hash.GetValueInt((int)SPC.Color, (int)(byte)ObjectColor.white);
        }


    }
}

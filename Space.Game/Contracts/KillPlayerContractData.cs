using Common;
using System.Xml.Linq;

namespace Nebula.Contracts {

    /// <summary>
    /// Contract for killing players
    /// </summary>
    public class KillPlayerContractData : ContractData {

        /// <summary>
        /// Number of players to kill
        /// </summary>
        public int playerCount { get; private set; }

        public KillPlayerContractData(XElement element)
            : base(element) {
            var playerElement = element.Element("player");
            if(playerElement != null ) {
                playerCount = playerElement.GetInt("count");
            } else {
                playerCount = 1;
            }
        }
    }
}

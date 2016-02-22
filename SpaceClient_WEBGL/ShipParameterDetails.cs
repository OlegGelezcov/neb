using ExitGames.Client.Photon;

namespace Nebula.Client {
    public class ShipParameterDetails {
        public readonly SpeedDetail speed = new SpeedDetail();
        public readonly ResistDetail resist = new ResistDetail();

        public void SetSpeed(Hashtable hash) {
            speed.ParseInfo(hash);
        }
        public void SetResist(Hashtable hash) {
            resist.ParseInfo(hash);
        }
    }
}

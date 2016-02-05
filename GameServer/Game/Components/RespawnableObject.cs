using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    public class RespawnableObject : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public float interval { get; private set; }

        public void Init(RespwanableComponentData data) {
            SetRespawnInterval(data.interval);
        }

        public void SetRespawnInterval(float inInterval) {
            interval = inInterval;
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Respawnable;
            }
        }

        public void Death() {
            if(interval > 0 ) {
                if(nebulaObject.Type == (byte)ItemType.Asteroid) {
                    //log.InfoFormat("set respawn data for asteroid = {0} at world = {1} yellow", nebulaObject.Id, nebulaObject.mmoWorld().Name);
                }
                (nebulaObject.world as MmoWorld).nebulaObjectManager.SetRespawnData(new RespawnData { handled = false, ID = nebulaObject.Id, time = Time.curtime() + interval, Type = nebulaObject.Type });
                //log.InfoFormat("set respwan data for object = {0}", nebulaObject.Id);
            }
        }
    }
}

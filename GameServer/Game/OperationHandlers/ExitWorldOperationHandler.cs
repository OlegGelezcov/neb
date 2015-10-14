using Common;
using Nebula.Game.Components;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Space.Game;
using Space.Server;
using Space.Server.Events;

namespace Nebula.Game.OperationHandlers {
    public class ExitWorldOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new Operation();
            operation.OnStart();

            ExitWorld(actor);

            // don't send response
            operation.OnComplete();
            return null;
        }

        private void ExitWorld(MmoActor actor) {
            //reset target when exiting world
            actor.GetComponent<PlayerTarget>().Clear();
            actor.GetComponent<PlayerLoaderObject>().Save(true);

            var worldExited = new WorldExited { WorldName = ((MmoWorld)actor.World).Name };
            actor.nebulaObject.SetInvisibility(false);
            actor.Dispose();

            // set initial handler
            ((MmoPeer)actor.Peer).SetCurrentOperationHandler((MmoPeer)actor.Peer);

            var eventData = new EventData((byte)EventCode.WorldExited, worldExited);

            actor.UpdateCharacterOnMaster();




            // use item channel to ensure that this event arrives in correct order with move/subscribe events
            actor.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

            CL.Out(LogFilter.PLAYER, "ExitWorld() Player {0} from world {1}".f(actor.name, ((MmoWorld)actor.World).Name));
        }
    }
}

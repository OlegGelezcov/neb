using Common;
using ExitGames.Logging;
using Nebula.Game.Components;
using Nebula.Game.Pets;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Space.Game;
using Space.Server;
using Space.Server.Events;

namespace Nebula.Game.OperationHandlers {
    class ExitWorkshopOperationHandler : BasePlayerOperationHandler {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
         
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new Operation();
            operation.OnStart();
            ExitWorkshop(actor);
            operation.OnComplete();
            return null;
        }


        private void ExitWorkshop(MmoActor actor) {

            if (actor.WorkshopSavedInfo != null) {

                if (actor.WorkshopSavedInfo.NowInWorkshop) {

                    var interestArea = new MmoClientInterestArea(actor.Peer, 0, actor.World) {
                        ViewDistanceEnter = actor.WorkshopSavedInfo.ViewDistanceEnter.ToVector(true),
                        ViewDistanceExit = actor.WorkshopSavedInfo.ViewDistanceExit.ToVector(true)
                    };
                    actor.AddInterestArea(interestArea);

                    if(actor.Avatar != null ) {
                        while(!(actor.Avatar.world as MmoWorld).ItemCache.AddItem(actor.Avatar)) {
                            log.Info("error of adding avatar to world");
                        }
                        log.Info("item to world added");
                    } else {
                        log.Error("exit workshop actor avatar is null");
                    }

                    //var avatar = new MmoItem(actor.World, actor.WorkshopSavedInfo.Position, actor.WorkshopSavedInfo.Rotation, actor.WorkshopSavedInfo.Properties,
                    //    actor, actor.WorkshopSavedInfo.ItemId, (byte)ItemType.Avatar);

                    //while (actor.World.ItemCache.AddItem(avatar) == false) {
                    //    //ConsoleLogging.Get.Print(LogFilter.ALL, "error of adding new item");
                    //}
                    //actor.AddItem(avatar);
                    //actor.SetAvatar(avatar);

                    //if (actor.Avatar != null) {
                    //    actor.GetComponent<PlayerLoaderObject>().Save(true);
                    //}

                    var workshopExited = new WorkshopExited { ItemId = actor.nebulaObject.Id, Position = actor.WorkshopSavedInfo.Position };
                    var eventData = new EventData((byte)EventCode.WorkshopExited, workshopExited);
                    actor.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

                    (actor.nebulaObject as MmoItem).Spawn(actor.WorkshopSavedInfo.Position);

                    actor.SetWorkshopStatus(false);
                    actor.SetNewSubZone((actor.World as MmoWorld).ResolvePositionSubzone(actor.WorkshopSavedInfo.Position.ToVector3()));
                    actor.nebulaObject.SetInvisibility(false);
                    actor.GetComponent<PetManager>().Reinitialize();

                } else {
                    //ConsoleLogging.Get.Print(LogFilter.ALL, "EXIT WORKSHOP: NOW IN NOT WORKSHOP");
                }
            } else {
                //ConsoleLogging.Get.Print(LogFilter.ALL, "EXIT: WORKSHOP SAVE INFO NULL");
            }
        }


    }
}

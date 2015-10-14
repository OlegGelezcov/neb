using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Common;
using Space.Server.Events;
using Space.Server;
using Nebula.Game.Components;
using ExitGames.Logging;

namespace Nebula.Game.OperationHandlers {
    public class EnterWorkshopOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {

            var operation = new EnterWorkshop(actor.Peer.Protocol, request);
            operation.OnStart();
            if (operation.IsValid == false) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (int)ReturnCode.Fatal,
                    DebugMessage = "Operation not valid"
                };
            }
            if (operation.ItemId != actor.Avatar.Id) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                    DebugMessage = "operation item id don't match with avatar id"
                };
            }

            EnterWorkshop((byte)request.Parameters[(byte)ParameterCode.Type], actor);

            operation.OnComplete();

            EnterWorkshopResponse responseObject = new EnterWorkshopResponse {
                ItemId = actor.WorkshopSavedInfo.ItemId,
                Info = actor.Station.GetInfo(),
                Type = (byte)request.Parameters[(byte)ParameterCode.Type]
            };
            actor.SetNewSubZone(-1);

            OperationResponse response = new OperationResponse(request.OperationCode, responseObject);
            return response;
        }

        public static void EnterWorkshop(byte workshopType, MmoActor actor) {
            try {

                //fully restore health when enter to station
                var damagable = actor.GetComponent<DamagableObject>();
                if(damagable) {
                    damagable.SetHealth(damagable.maximumHealth);
                }
                actor.GetComponent<PlayerTarget>().Clear();
                SaveWorkshopInfo(actor);
                var workshopEntered = new WorkshopEntered {
                    WorkshopId = actor.GetComponent<PlayerCharacterObject>().workshop,
                    Info = actor.Station.GetInfo(), Type = workshopType };

                

                actor.DeleteItemsFromWorld();
                var eventData = new EventData((byte)EventCode.WorkshopEntered, workshopEntered);
                actor.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel, Unreliable = false });
                actor.SetWorkshopStatus(true);
                //(actor.nebulaObject as Item).ClearChannels();
                //CreateNewItem(actor);
            } catch (Exception ex) {
                //ConsoleLogging.Get.Print(LogFilter.ALL, ex.Message);
                log.InfoFormat("error: " + ex.Message);
                log.InfoFormat(ex.StackTrace);
            }
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static void CreateNewItem(MmoActor player) {
            var oldItem = player.nebulaObject as MmoItem;

            var interestArea = new MmoClientInterestArea(player.Peer, 0, player.World) {
                ViewDistanceEnter = new GameMath.Vector { X = 50000,Y = 50000, Z =  50000},
                ViewDistanceExit = new GameMath.Vector { X = 100000, Y = 100000, Z = 100000 }
            };

            var newITem = new MmoItem(player.Peer,
                interestArea,
                player.World,
                player.application,
                player.transform.position.ToArray(),
                player.transform.rotation.ToArray(),
                new System.Collections.Hashtable(),
                player.nebulaObject.Id,
                player.nebulaObject.tags,
                player.nebulaObject.size,
                player.nebulaObject.subZone,
                oldItem.allBehaviours);

            log.InfoFormat("why why why green");

            player.GetComponent<DamagableObject>().SetIgnoreDamageAtStart(true);
            player.GetComponent<DamagableObject>().SetIgnoreDamageInterval(30);
            oldItem.Dispose();
        }

        private static void SaveWorkshopInfo(MmoActor actor) {
            InterestArea interestArea;
            actor.TryGetInterestArea(0, out interestArea);
            actor.SetWorkshopSaveInfo(new ExitWorkshopSavedInfo {
                Position = actor.Avatar.transform.position.ToArray(),
                Rotation = actor.transform.rotation.ToArray(),
                Properties = actor.props.raw,
                ViewDistanceEnter = (interestArea != null) ? interestArea.ViewDistanceEnter.toArray() : new float[] { 1e6f, 1e6f, 1e6f },
                ViewDistanceExit = (interestArea != null) ? interestArea.ViewDistanceExit.toArray() : new float[] { 2e6f, 2e6f, 2e6f },
                ItemId = actor.Avatar.Id
            });
        }
    }
}

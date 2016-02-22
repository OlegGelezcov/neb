namespace Space.Game {
    using Common;
    using ExitGames.Logging;
    using Nebula.Game.Components;
    using Nebula.Game.Contracts;
    using Nebula.Game.Events;
    using Nebula.Game.Pets;
    using NebulaCommon.SelectCharacter;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using PhotonHostRuntimeInterfaces;
    using Space.Mmo.Server;
    using Space.Server;
    using Space.Server.Operations;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    public class MmoPeer : Peer, IOperationHandler
    {
        private PeerActionExecutor executor = new PeerActionExecutor();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly GameApplication application;

        public MmoPeer(IRpcProtocol rpcProtocol, IPhotonPeer nativePeer, GameApplication application)
            : base(rpcProtocol, nativePeer)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            this.application = application;
            this.SetCurrentOperationHandler(this);
        }

        public IDisposable CounterSubscription { get; set; }
        //public IDisposable MmoRadarSubscription { get; set; }

        public static OperationResponse InvalidOperation(OperationRequest request )
        {
            return new OperationResponse(request.OperationCode)
            {
                ReturnCode = (int)ReturnCode.InvalidOperation,
                DebugMessage = "InvalidOperation: " + (OperationCode)request.OperationCode
            };
        }

        private OperationResponse ExecAction(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            try
            {
                var operation = new ExecAction(peer.Protocol, request);
                if (false == operation.IsValid)
                {
                    return GetErrorResponse(ReturnCode.InvalidOperationParameter, operation.GetErrorMessage());
                }
                var method = this.executor.GetType().GetMethod(operation.Action);
                if (method != null)
                {
                    object methodResult = method.Invoke(this.executor, operation.Parameters);
                    if (methodResult != null && methodResult is Hashtable)
                    {
                        var hashResult = methodResult as Hashtable;
                        ExecActionResponse response = new ExecActionResponse
                        {
                            Result = hashResult,
                            Action = operation.Action,
                            ItemId = string.Empty
                        };
                        return new OperationResponse(OperationCode.ExecAction.toByte(), response)
                        {
                            ReturnCode = (int)ReturnCode.Ok,
                            DebugMessage = "action: {0} completed".f(operation.Action)
                        };
                    }
                    else
                    {
                        return GetErrorResponse(ReturnCode.Fatal, "Invalid return value from method");
                    }
                }
                else
                {
                    return GetErrorResponse(ReturnCode.Fatal, "Method {0} in executor not founded".f(operation.Action));
                }
            }
            catch(Exception ex)
            {
                CL.Out(LogFilter.PLAYER, ex.Message);
                CL.Out(LogFilter.PLAYER, ex.StackTrace);
            }
            return null;
        }

        private OperationResponse GetErrorResponse(ReturnCode code, string message)
        {
            return new OperationResponse { ReturnCode = (short)code, DebugMessage = message };
        }

        public OperationResponse OperationCreateWorld(PeerBase peer, OperationRequest request )
        {
            try
            {
                var operation = new CreateWorld(peer.Protocol, request);
                if (!operation.IsValid)
                {
                    return new OperationResponse(request.OperationCode)
                    {
                        ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                        DebugMessage = operation.GetErrorMessage()
                    };
                }

                MmoWorld world;
                MethodReturnValue result = MethodReturnValue.Ok;


                if(GameApplication.ResourcePool() == null ) {
                    log.Info("Resource pool is null");
                }

                log.InfoFormat("find resource for world = {0}", operation.WorldName);

                Res resource = GameApplication.ResourcePool().Resource(operation.WorldName);

                if (MmoWorldCache.Instance(application).TryCreate(operation.WorldName,
                    Settings.CornerMin,
                    Settings.CornerMax,
                    Settings.TileDimensions, out world, resource))
                {
                    result = MethodReturnValue.Ok;
                    world.Initialize();
                }
                else
                {
                    result = MethodReturnValue.Fail((int)ReturnCode.WorldAlreadyExists, "WorldAlreadyExists");
                }

                return operation.GetOperationResponse(result);
            }
            catch(Exception ex)
            {
                CL.Out(LogFilter.PLAYER, ex.Message);
                CL.Out(LogFilter.PLAYER, ex.StackTrace);
            }
            return null;
        }



        public OperationResponse OperationEnterWorld(PeerBase peer, OperationRequest request, SendParameters sendParameters) {
            try {
                var operation = new EnterWorld(peer.Protocol, request);
                if (!operation.IsValid) {
                    return new OperationResponse(request.OperationCode) {
                        ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                        DebugMessage = operation.GetErrorMessage()
                    };
                }

                MmoWorld world;
                if (MmoWorldCache.Instance(application).TryGet(operation.WorldName, out world) == false) {
                    return operation.GetOperationResponse((int)ReturnCode.WorldNotFound, "WorldNotFound", new System.Collections.Hashtable());
                }
                var interestArea = new MmoClientInterestArea(peer, operation.InterestAreaId, world) {
                    ViewDistanceEnter = operation.ViewDistanceEnter.ToVector(true),
                    ViewDistanceExit = operation.ViewDistanceExit.ToVector(true)
                };

                PlayerCharacter character = new PlayerCharacter();
                character.SetCharacterId(operation.CharacterId);
                character.SetModel(operation.Model);
                character.SetName(operation.Name);
                character.SetRace((Race)operation.Race);
                character.SetWorkshop((Workshop)operation.Workshop);
                character.Login = operation.Login;

                Type[] components = new Type[] {
                    typeof(MmoActor),
                    typeof(AIState),
                    typeof(PlayerCharacterObject),
                    typeof(RaceableObject),
                    typeof(MmoMessageComponent),
                    typeof(PlayerShip),
                    typeof(ShipWeapon),
                    typeof(PlayerSkills),
                    typeof(PlayerBonuses),
                    typeof(PlayerLoaderObject),
                    typeof(ShipBasedDamagableObject),
                    typeof(PlayerTarget),
                    typeof(ShipEnergyBlock),
                    typeof(PlayerShipMovable),
                    typeof(PassiveBonusesComponent),
                    typeof(PlayerTimedEffects),
                    typeof(PetManager),
                    typeof(PlayerEventSubscriber),
                    typeof(ContractManager),
                    typeof(AchievmentComponent)
                };

                Dictionary<byte, object> tags = new Dictionary<byte, object> {
                    {(byte)PlayerTags.GameRefId, operation.GameRefId },
                    {(byte)PlayerTags.CharacterId, character.CharacterId },
                    {(byte)PlayerTags.Race, character.Race },
                    {(byte)PlayerTags.Workshop, character.Workshop },
                    {(byte)PlayerTags.Name, ( character.Name != null ) ? character.Name : string.Empty },
                    {(byte)PlayerTags.Model, character.Model},
                    {(byte)PlayerTags.Login, operation.Login }
                };
                //var actor = new MmoActor(peer, world, interestArea, operation.GameRefId, character, application);
                log.InfoFormat("before creating player item = {0}", operation.GameRefId);
                var avatar = new MmoItem(peer, interestArea, world, application, operation.Position, operation.Rotation, operation.Properties, operation.GameRefId, tags, 1f, 0,  components);
                avatar.GetComponent<MmoActor>().SetApplication(application);
                log.InfoFormat("init player item at position = {0}", operation.Position.ToVector3());
                log.InfoFormat("Before Start() player");
                avatar.GetComponent<DamagableObject>().SetIgnoreDamageAtStart(true);
                avatar.GetComponent<DamagableObject>().SetIgnoreDamageInterval(30);
                avatar.Update(0);
                log.InfoFormat("Before Load() player");


                log.InfoFormat("calling load on avatar [dy]");
                avatar.GetComponent<PlayerLoaderObject>().Load();

                while (world.ItemCache.AddItem(avatar) == false) {
                    Item otherAvatarItem;
                    if (world.ItemCache.TryGetItem(avatar.Type, avatar.Id, out otherAvatarItem)) {
                        avatar.Dispose();
                        avatar.GetComponent<MmoActor>().Dispose();
                        interestArea.Dispose();
                        ((Peer)((MmoItem)otherAvatarItem).Owner.Peer).DisconnectByOtherPeer(this, request, sendParameters);
                        return null;
                    }
                }

                log.Info("After adding player to world");


                //actor.AddItem(avatar);
                //actor.Avatar = avatar;
                //actor.Initialize();
                //actor.Target.ResetTarget();

                ((Peer)peer).SetCurrentOperationHandler(avatar.GetComponent<MmoActor>());
                log.Info("Set player item operation handler to Actor");

                var responseObject = new EnterWorldResponse {
                    BottomRightCorner = world.Area.Max.ToFloatArray(3),
                    TopLeftCorner = world.Area.Min.ToFloatArray(3),
                    TileDimensions = world.TileDimensions.ToFloatArray(3),
                    WorldName = world.Name,
                    Content = world.GetInfo(),
                    ItemId = avatar.Id,
                    components = avatar.componentIds,
                    size = avatar.size,
                    subZone = avatar.subZone
                    //startAtStation = operation.EnterAtStation
                };
                log.InfoFormat("Components = {0} sended to player back", avatar.componentIds.Length);


                var response = new OperationResponse(request.OperationCode, responseObject);
                sendParameters.ChannelId = Settings.ItemEventChannel;
                peer.SendOperationResponse(response, sendParameters);
                log.Info("World entered response sended back");

                lock (interestArea.SyncRoot) {
                    interestArea.AttachToItem(avatar);
                    interestArea.UpdateInterestManagement();
                }

                avatar.Spawn(operation.Position);
                log.Info("Avatar Spawn fire....");

                return null;
            } catch (Exception ex) {
                CL.Out(LogFilter.PLAYER, ex.Message);
                CL.Out(LogFilter.PLAYER, ex.StackTrace);
            }
            return null;
        }

        public void OnDisconnect(PeerBase peer)
        {
            log.Info("player peer disconnect");
            this.SetCurrentOperationHandler(null);
            this.Dispose();
        }

        public void OnDisconnectByOtherPeer(PeerBase peer)
        {
            log.Info("player peer disconnect by other peer");
            // disconnect after any queued events are sent
            peer.RequestFiber.Enqueue(() => peer.RequestFiber.Enqueue(peer.Disconnect));
        }

        public OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters) {
            switch ((OperationCode)operationRequest.OperationCode) {
                case OperationCode.CreateWorld:
                    {
                        return this.OperationCreateWorld(peer, operationRequest);
                    }

                case OperationCode.EnterWorld:
                    {
                        return this.OperationEnterWorld(peer, operationRequest, sendParameters);
                    }

                case OperationCode.SubscribeCounter:
                    {
                        return CounterOperations.SubscribeCounter(peer, operationRequest);
                    }

                case OperationCode.UnsubscribeCounter:
                    {
                        return CounterOperations.UnsubscribeCounter(peer, operationRequest);
                    }
                case OperationCode.ExecAction:
                    {
                        return this.ExecAction(peer, operationRequest, sendParameters);
                    }
                default:
                    {
                        return InvalidOperation(operationRequest);
                    }
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {

                if (this.CounterSubscription != null) {
                    this.CounterSubscription.Dispose();
                    this.CounterSubscription = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}

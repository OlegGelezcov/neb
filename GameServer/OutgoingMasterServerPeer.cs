using Common;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Nebula;
using Nebula.Engine;
using Nebula.Game;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using Nebula.Inventory;
using NebulaCommon;
using NebulaCommon.Group;
using NebulaCommon.ServerToServer.Events;
using NebulaCommon.ServerToServer.Operations;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;
using ServerClientCommon;
using Space.Database;
using Space.Game;
using Space.Game.Inventory;
using Space.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

public class OutgoingMasterServerPeer : ServerPeerBase {

    private static readonly ILogger log = LogManager.GetCurrentClassLogger();
    //private readonly GameApplication application;

    private bool redirected = false;
    private IDisposable updateLoop;
    private GameApplication m_App;
    private readonly S2SMethodInvoker m_Invoker;

    public OutgoingMasterServerPeer(IRpcProtocol protocol, IPhotonPeer nativePeer, GameApplication application)
        : base(protocol, nativePeer) {
        m_App = application;
        m_Invoker = new S2SMethodInvoker(m_App);
        //this.application = application;
        log.InfoFormat("connection to master at {0}:{1} established (id={2}), serverId={3}", this.RemoteIP, this.RemotePort, this.ConnectionId, GameApplication.ServerId);
        this.RequestFiber.Enqueue(this.Register);
    }

    private void Register() {
        var contract = new RegisterGameServer {
            GameServerAddress = m_App.PublicIpAddress.ToString(),
            UdpPort = m_App.GamingUdpPort,
            TcpPort = m_App.GamingTcpPort,
            WebSocketPort = m_App.GamingWebSocketPort,
            ServerId = GameApplication.ServerId.ToString(),
            ServerState = (int)ServerState.Normal,
            ServerType = (byte)NebulaCommon.ServerType.Game
        };
        if (log.IsInfoEnabled) {
            log.InfoFormat(
                "Registering game server with address {0}, TCP {1}, UDP {2}, WebSocket {3}, ServerID {4}",
                contract.GameServerAddress,
                contract.TcpPort,
                contract.UdpPort,
                contract.WebSocketPort,
                contract.ServerId
                );
        }
        var request = new OperationRequest((byte)ServerToServerOperationCode.RegisterGameServer, contract);
        this.SendOperationRequest(request, new SendParameters());
    }

    protected bool IsRegistered { get; set; }


    protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
        this.IsRegistered = false;
        this.StopUpdateLoop();
        MmoWorldCache.Instance(m_App).Clear();

        if (!this.redirected) {
            log.InfoFormat("connection to master closed (id={0}, reason={1}, detail={2}), serverId={3}",
                this.ConnectionId, reasonCode, reasonDetail, GameApplication.ServerId);
            m_App.ReconnectToMaster();
        } else {
            if (log.IsDebugEnabled) {
                log.DebugFormat("{0} disconnected from master server: reason={1}, detail={2}, serverId={3}",
                    this.ConnectionId, reasonCode, reasonDetail, GameApplication.ServerId);
            }
        }
    }



    protected override void OnEvent(IEventData eventData, SendParameters sendParameters) {
        switch((S2SEventCode)eventData.Code) {
            case S2SEventCode.GroupUpdate:
                HandleGroupUpdateEvent(eventData);
                break;
            case S2SEventCode.GroupRemoved:
                HandleGroupRemovedEvent(eventData);
                break;
            case S2SEventCode.GETInventoryItemStart:
                HandleGETInventoryItemStart(eventData, sendParameters);
                break;
            case S2SEventCode.GETInventoryItemsStart:
                HandleGETInventoryItemsStart(eventData, sendParameters);
                break;
            case S2SEventCode.PUTInventoryItemStart:
                HandlePUTInventoryItemStart(eventData, sendParameters);
                break;
            case S2SEventCode.InvokeMethodEnd:
                HandleInvokeMethodEnd(eventData, sendParameters);
                break;
            case S2SEventCode.RaceStatusChanged:
                HandleRaceStatusChanged(eventData, sendParameters);
                break;
            case S2SEventCode.WorldRaceChanged:
                HandleWorldRaceChanged(eventData, sendParameters);
                break;
            case S2SEventCode.InvokeMethodStart:
                HandleInvokeMethodStart(eventData, sendParameters);
                break;
        }
    }

    #region Game Methods

    private void HandleInvokeMethodStart(IEventData eventData, SendParameters sendParameters) {
        string method = (string)eventData.Parameters[(byte)ServerToServerParameterCode.Method];
        object[] arguments = eventData.Parameters[(byte)ServerToServerParameterCode.Arguments] as object[];
        string sourceServerID = eventData.Parameters[(byte)ServerToServerParameterCode.SourceServer] as string;
        byte targetServetType = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.TargetServer];

        var mtd = m_Invoker.GetType().GetMethod(method);
        S2SInvokeMethodEnd end = new S2SInvokeMethodEnd {
            method = method,
            sourceServerID = sourceServerID,
            targetServerType = targetServetType,
        };

        if(mtd != null ) {
            object result = mtd.Invoke(m_Invoker, arguments);
            end.callSuccess = true;
            end.result = result;
        } else {
            end.callSuccess = false;
            end.result = null;
        }
        EventData retEvent = new EventData((byte)S2SEventCode.InvokeMethodEnd, end);
        SendEvent(retEvent, new SendParameters());
    }
    /// <summary>
    /// Handle race status update from Select Character Server
    /// </summary>
    private void HandleRaceStatusChanged(IEventData eventData, SendParameters sendParameters) {
        string gameRefID = (string)eventData[(byte)ServerToServerParameterCode.GameRefId];
        string characterID = (string)eventData[(byte)ServerToServerParameterCode.CharacterId];
        int raceStatus = (int)eventData[(byte)ServerToServerParameterCode.RaceStatus];

        foreach (string locationID in m_App.CurrentRole().Locations) {
            MmoWorld world;
            if (MmoWorldCache.Instance(m_App).TryGet(locationID, out world)) {
                NebulaObject playerObj;
                if (world.TryGetObject((byte)ItemType.Avatar, gameRefID, out playerObj)) {
                    if (playerObj.GetComponent<PlayerCharacterObject>()) {
                        playerObj.GetComponent<PlayerCharacterObject>().SetRaceStatus(raceStatus);
                        log.InfoFormat("send player {0} race status {1}", gameRefID, (RaceStatus)raceStatus);
                        break;
                    }
                }
            }
        }
    }

    private void HandleInvokeMethodEnd(IEventData eventData, SendParameters sendParameters) {
        bool success = (bool)eventData.Parameters[(byte)ServerToServerParameterCode.Success];
        object result = eventData.Parameters[(byte)ServerToServerParameterCode.Result] as object;
        string method = eventData.Parameters[(byte)ServerToServerParameterCode.Method] as string;
        NebulaCommon.ServerType serverType = (NebulaCommon.ServerType)(byte)eventData.Parameters[(byte)ServerToServerParameterCode.TargetServer];

        if( success ) {
            log.InfoFormat("method {0} successfully called on server {1}", method, serverType);
            if(result != null ) {
                log.InfoFormat("method {0} call result = {1}".Color(LogColor.orange), method, result.ToString());

                switch(method) {
                    case "RequestGuildInfo":
                        {
                            HandleRequestGuildInfo(result);
                        }
                        break;
                    case "RequestIcon": {
                            HandleRequestIcon(result);
                        }
                        break;
                    case "SendChatBroadcast": {
                            Hashtable hash = result as Hashtable;
                            if(hash != null ) {
                                RPCErrorCode code = (RPCErrorCode)hash.GetValue<int>((int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError);
                                log.InfoFormat("send chat broadcast response: {0}", code);
                            }
                        }
                        break;
                    case "PlanetObjectUnderAttackNotification": {
                            log.Info("PlanetObjectUnderAttackNotification(): end response...");
                        }
                        break;
                    case "AddNebulaCredits": {
                            log.Info($"S2SMETHOD: {method} ended with code {result}");
                        }
                        break;
                }
            }
        } else {
            log.InfoFormat("fail call method {0} on server {1}", method, serverType);
        }
    }


    private void HandleRequestGuildInfo(object result) {
        Hashtable guildInfo = result as Hashtable;
        if(guildInfo != null ) {
            string gameRef = guildInfo.GetValue<string>((int)SPC.GameRefId, string.Empty);
            if(!string.IsNullOrEmpty(gameRef)) {
                MmoActor player;
                if(m_App.serverActors.TryGetValue(gameRef, out player)) {
                    var character = player.GetComponent<PlayerCharacterObject>();
                    if(character != null ) {
                        character.SetGuildInfo(guildInfo);
                    }
                }
            }
        }
    }

    private void HandleRequestIcon(object result) {
        Hashtable hash = result as Hashtable;
        if(hash != null ) {
            RPCErrorCode code = (RPCErrorCode)hash.GetValue<int>((int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError);

            if (code == RPCErrorCode.Ok) {
                string gameRef = hash.GetValue<string>((int)SPC.GameRefId, string.Empty);
                int icon = hash.GetValue<int>((int)SPC.Icon, 0);
                if (!string.IsNullOrEmpty(gameRef)) {
                    MmoActor player;
                    if (m_App.serverActors.TryGetValue(gameRef, out player)) {
                        if (player) {
                            var character = player.GetComponent<PlayerCharacterObject>();
                            if (character) {
                                character.SetIcon(icon);
                            }
                        }
                    }
                }
            } else {
                log.InfoFormat("RPC:RequestIcon error: {0}", code);
            }

        }
    }

    private void HandlePUTInventoryItemStart(IEventData eventData, SendParameters sendParameters) {
        //log.Info("HandlePUTInventoryItemStart: PUT inventory item event received");
        PUTInventoryItemTransactionStart start = new PUTInventoryItemTransactionStart(eventData);

        try {
            MmoActor player;
            if (!m_App.serverActors.TryGetValue(start.gameRefID, out player)) {
                log.InfoFormat("HandlePUTInventoryItemStart: player = {0} not founded on server", start.gameRefID);
                return;
            }

            PUTInventoryItemTransactionEnd end = new PUTInventoryItemTransactionEnd {
                characterID = start.characterID,
                gameRefID = start.gameRefID,
                inventoryType = start.inventoryType,
                itemID = start.itemID,
                count = start.count,
                transactionID = start.transactionID,
                transactionSource = start.transactionSource,
                transactionStartServer = start.transactionStartServer,
                transactionEndServer = start.transactionEndServer
            };

            InventoryType invType = (InventoryType)start.inventoryType;
            if (invType == InventoryType.ship || invType == InventoryType.station) {
                var inventory = (invType == InventoryType.ship) ? player.Inventory : player.Station.StationInventory;
                int count = 0;
                var itemObject = InventoryUtils.Create(start.targetObject as Hashtable, out count);
                count = start.count;

                if (!inventory.EnoughSpace(new Dictionary<string, InventoryObjectType> { { itemObject.Id, itemObject.Type } })) {
                    end.success = false; end.result = 1; end.returnCode = (short)ReturnCode.NotEnoughInventorySpace;
                } else {
                    if (!inventory.Add(itemObject, count)) {
                        end.success = false; end.result = 1; end.returnCode = (short)ReturnCode.ErrorAddingToInventory;
                    } else {
                        end.success = true; end.result = 0; end.returnCode = (short)ReturnCode.Ok;
                    }
                    player.EventOnInventoryUpdated();
                    player.EventOnStationHoldUpdated();
                }

            }

            EventData evt = new EventData((byte)S2SEventCode.PUTInventoryItemEnd, end);
            SendEvent(evt, sendParameters);
            log.InfoFormat("HandlePUTInventoryItemStart: transaction end sended with success = {0}", end.success);
        } catch (Exception exception) {
            log.Info("exception");
            log.Info(exception.Message);
            log.Info(exception.StackTrace);
        }
    }

    private void HandleGETInventoryItemsStart(IEventData eventData, SendParameters sendParameters) {
        try {
            log.InfoFormat("OutgoinfMasterServerPeer.HandleGETInventoryItemsStart()");
            GETInventoryItemsTransactionStart start = new GETInventoryItemsTransactionStart(eventData);

            MmoActor player;
            if (m_App.serverActors.TryGetValue(start.gameRefID, out player)) {

                GETInventoryItemsTransactionEnd end = new GETInventoryItemsTransactionEnd {
                    characterID = start.characterID,
                    gameRefID = start.gameRefID,
                    inventoryType = start.inventoryType,
                    transactionID = start.transactionID,
                    transactionSource = start.transactionSource,
                    items = start.items,
                    transactionStartServer = start.transactionStartServer,
                    transactionEndServer = start.transactionEndServer
                };

                List<IDCountPair> itemPairs = new List<IDCountPair>();
                foreach (DictionaryEntry entry in start.items) {
                    itemPairs.Add(new IDCountPair { ID = (string)entry.Key, count = (int)entry.Value });
                }

                ServerInventory inventory;
                if (start.inventoryType == (byte)InventoryType.ship) {
                    inventory = player.Inventory;
                } else {
                    inventory = player.Station.StationInventory;
                }

                bool checkItems = true;
                foreach (var pair in itemPairs) {
                    if (inventory.ItemCount(pair.ID) < pair.count) {
                        checkItems = false;
                        break;
                    }
                }

                if (!checkItems) {
                    end.result = new Hashtable();
                    end.success = false;
                    end.returnCode = (short)ReturnCode.InventoryItemNotFound;
                    log.InfoFormat("OutgoinfMasterServerPeer.HandleGETInventoryItemsStart(): item check invalid");
                } else {
                    Hashtable result = new Hashtable();

                    foreach (var itemPair in itemPairs) {
                        ServerInventoryItem it;
                        if (inventory.TryGetItem(itemPair.ID, out it)) {
                            Hashtable itInfo = new Hashtable {
                                { (int)SPC.Id, it.Object.Id },
                                { (int)SPC.Count, itemPair.count },
                                { (int)SPC.Info, it.Object.GetInfo() }
                            };
                            result.Add(it.Object.Id, itInfo);
                            inventory.Remove(it.Object.Type, it.Object.Id, itemPair.count);
                        }
                    }

                    end.result = result;
                    end.success = true;
                    end.returnCode = (short)ReturnCode.Ok;
                    player.EventOnInventoryUpdated();
                    player.EventOnStationHoldUpdated();
                    log.InfoFormat("OutgoingMasterServerPeer.HandleGETInventoryItemsStart()-SUCCESS");
                }

                EventData evt = new EventData((byte)S2SEventCode.GETInventoryItemsEnd, end);
                SendEvent(evt, sendParameters);
            } else {
                log.InfoFormat("OutgoinfMasterServerPeer.HandleGETInventoryItemsStart(): player not found on server [dy]");
            }
        } catch(Exception exception) {
            log.ErrorFormat(exception.Message);
            log.ErrorFormat(exception.StackTrace);
        }
    }

    private void HandleGETInventoryItemStart(IEventData eventData, SendParameters sendParameters ) {



        try {
            log.InfoFormat("OutgoinfMasterServerPeer.HandleGETInventoryItemStart()");

            GETInventoryItemTransactionStart start = new GETInventoryItemTransactionStart(eventData);

            MmoActor player;
            if (m_App.serverActors.TryGetValue(start.gameRefID, out player)) {

                GETInventoryItemTransactionEnd end = new GETInventoryItemTransactionEnd {
                    characterID = start.characterID,
                    count = start.count,
                    gameRefID = start.gameRefID,
                    inventoryType = start.inventoryType,
                    itemID = start.itemID,
                    transactionID = start.transactionID,
                    transactionSource = start.transactionSource,
                    transactionStartServer = start.transactionStartServer,
                    transactionEndServer = start.transactionEndServer
                };

                if (start.inventoryType == (byte)InventoryType.ship || start.inventoryType == (byte)InventoryType.station) {
                    var inventory = ((InventoryType)start.inventoryType == InventoryType.ship) ? player.Inventory : player.Station.StationInventory;
                    ServerInventoryItem item;
                    if (inventory.TryGetItem(start.itemID, out item)) {
                        if (item.Count >= start.count) {
                            inventory.Remove(item.Object.Type, item.Object.Id, start.count);
                            end.result = item.Object.GetInfo();
                            end.success = true;
                            end.returnCode = (short)ReturnCode.Ok;
                            player.EventOnInventoryUpdated();
                            player.EventOnStationHoldUpdated();

                        } else {
                            end.result = new Hashtable();
                            end.success = false;
                            end.returnCode = (short)ReturnCode.DontEnoughItemsInInventory;
                        }
                    } else {
                        end.result = new Hashtable();
                        end.success = false;
                        end.returnCode = (short)ReturnCode.InventoryItemNotFound;
                    }
                }

                EventData evt = new EventData((byte)S2SEventCode.GETInventoryItemEnd, end);

                SendEvent(evt, sendParameters);
                log.InfoFormat("send event back {0} with status {1}", (S2SEventCode)evt.Code, end.success);
            } else {
                log.InfoFormat("not found such player");
            }
        } catch (Exception ex) {
            log.Info("exception");
            log.Info(ex.Message);
            log.Info(ex.StackTrace);
        }

    }

    private void HandleGroupUpdateEvent(IEventData eventData) {
        try {
            log.InfoFormat("OutgoinfMasterServerPeer.HandleGroupUpdateEvent()");
            m_App.updater.EnqueueFiberAction(() => {
                try {
                    Hashtable groupHash = (Hashtable)eventData.Parameters[(byte)ServerToServerParameterCode.Group];
                    Group group = new Group();
                    group.ParseInfo(groupHash);
                    foreach (var member in group.members) {
                        if (string.IsNullOrEmpty(member.Value.worldID)) { continue; }
                        MmoWorld world;
                        if (MmoWorldCache.Instance(m_App).TryGet(member.Value.worldID, out world)) {
                            Item item;
                            if (world.ItemCache.TryGetItem((byte)ItemType.Avatar, member.Value.gameRefID, out item)) {
                                log.InfoFormat("set group at player = {0}", item.Id);
                                item.GetComponent<PlayerCharacterObject>().SetGroup(group);
                            }
                        }
                    }
                } catch(Exception ex) {
                    log.Info("exception");
                    log.Info(ex.Message);
                    log.Info(ex.StackTrace);
                }
            });
        } catch (Exception ex) {
            log.Info("exception");
            log.Info(ex.Message);
            log.Info(ex.StackTrace);
        }
    }

    private void HandleGroupRemovedEvent(IEventData eventData) {

        log.InfoFormat("OutgoinfMasterServerPeer.HandleGroupRemovedEvent()");
        try {
            string groupID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.Group];
            foreach (var location in m_App.CurrentRole().Locations) {
                MmoWorld world;
                if (MmoWorldCache.Instance(m_App).TryGet(location, out world)) {
                    var items = world.GetItems((it) => it.Type == (byte)ItemType.Avatar);
                    foreach (var item in items) {
                        item.Value.SendMessage(ComponentMessages.OnGroupRemoved, groupID);
                    }
                }
            }
        } catch (Exception ex) {
            log.Info("exception");
            log.Info(ex.Message);
            log.Info(ex.StackTrace);
        }
    }

    private void HandleWorldRaceChanged(IEventData eventData, SendParameters sendParameters) {
        log.InfoFormat("MasterPeer: received world race changed event... [red]");

        m_App.updater.EnqueueFiberAction(() => {
            try {

                string worldID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.WorldId];
                byte previousRace = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.PreviousRace];
                byte currentRace = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.CurrentRace];

                Hashtable info = new Hashtable {
                    { (int)SPC.WorldId, worldID },
                    { (int)SPC.PreviousRace, previousRace },
                    { (int)SPC.CurrentRace, currentRace }
                };

                MmoWorldCache.Instance(m_App).SendWorldRaceChanged(info);

            }catch(Exception exception) {
                log.InfoFormat(exception.Message + " [red]");
                log.InfoFormat(exception.StackTrace + " [red]");
            }
        });
    }
    #endregion

    protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
        if (log.IsDebugEnabled) {
            log.DebugFormat("Received unknown operation code {0}", operationRequest.OperationCode);
        }
        var response = new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = -1, DebugMessage = "Unknown operation code" };
        this.SendOperationResponse(response, sendParameters);
    }

    protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters) {
        switch ((ServerToServerOperationCode)operationResponse.OperationCode) {
            default:
                {
                    if (log.IsDebugEnabled) {
                        log.DebugFormat("Received unknown operation code {0}", operationResponse.OperationCode);
                    }
                    break;
                }
            case ServerToServerOperationCode.RegisterGameServer:
                {
                    this.HandleRegisterGameServerResponse(operationResponse);
                    break;
                }
        }
    }

    private void HandleRegisterGameServerResponse(OperationResponse operationResponse) {
        var contract = new RegisterGameServerResponse(this.Protocol, operationResponse);
        if (!contract.IsValid) {
            if (operationResponse.ReturnCode != (short)ReturnCode.Ok) {
                log.ErrorFormat("RegisterGameServer returned with err {0}: {1}", operationResponse.ReturnCode, operationResponse.DebugMessage);
            }
            log.Error("RegisterGameServerResponse contract invalid: " + contract.GetErrorMessage());
            this.Disconnect();
            return;
        }

        switch (operationResponse.ReturnCode) {
            case (short)ReturnCode.Ok:
                {
                    log.InfoFormat("Successfully registered at master server: serverId={0}", GameApplication.ServerId);
                    this.IsRegistered = true;
                    this.StartUpdateLoop();
                    m_App.updater.Start();
                    break;
                }
            case (short)ReturnCode.RedirectRepeat:
                {
                    var address = new IPAddress(contract.InternalAddress);
                    log.InfoFormat("Connected master server is not the leader; Reconnecting to master at IP {0}...", address);
                    this.Reconnect(address);
                    break;
                }
            default:
                {
                    log.WarnFormat("Failed to register at master: err={0}, msg={1}, serverid={2}", operationResponse.ReturnCode, operationResponse.DebugMessage, GameApplication.ServerId);
                    this.Disconnect();
                    break;
                }
        }
    }



    protected void StartUpdateLoop() {
        if (this.updateLoop != null) {
            log.Error("Update loop already started! Duplicate RegisterGameServer response?");
            this.updateLoop.Dispose();
        }

        this.updateLoop = this.RequestFiber.ScheduleOnInterval(this.UpdateServerState, 1000, 1000);

    }

    protected void StopUpdateLoop() {
        if (this.updateLoop != null) {
            this.updateLoop.Dispose();
            this.updateLoop = null;
        }
    }

    public void UpdateServerState() {
        if (this.Connected == false) {
            return;
        }
        //log.Info("outgoing master server peer update");
        //here do something
    }

    protected void Reconnect(IPAddress address) {
        this.redirected = true;
        log.InfoFormat("Reconnecting to master: serverId={0}", GameApplication.ServerId);
        m_App.ConnectToMaster(new IPEndPoint(address, this.RemotePort));
        this.Disconnect();
        this.Dispose();
    }


    



}

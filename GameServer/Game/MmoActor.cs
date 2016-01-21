
namespace Space.Game {
    using Common;
    using ExitGames.Logging;
    using GameMath;
    using Nebula.Database;
    using Nebula.Engine;
    using Nebula.Game;
    using Nebula.Game.Components;
    using Nebula.Game.OperationHandlers;
    using Nebula.Game.Pets;
    using Nebula.Inventory.Objects;
    using NebulaCommon.SelectCharacter;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using ServerClientCommon;
    using Space.Database;
    using Space.Game.Drop;
    using Space.Game.Inventory;
    using Space.Game.Inventory.Objects;
    using Space.Game.Player;
    using Space.Server;
    using Space.Server.Events;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    public enum PlayerTags : byte { CharacterId, GameRefId, Race, Workshop, Name, Model, Login }


    [REQUIRE_COMPONENT(typeof(PlayerShip))]
    [REQUIRE_COMPONENT(typeof(PlayerTarget))]
    [REQUIRE_COMPONENT(typeof(NebulaTransform))]
    [REQUIRE_COMPONENT(typeof(NebulaObjectProperties))]
    [REQUIRE_COMPONENT(typeof(PlayerCharacterObject))]
    [REQUIRE_COMPONENT(typeof(AIState))]
    [REQUIRE_COMPONENT(typeof(RaceableObject))]
    [REQUIRE_COMPONENT(typeof(MmoMessageComponent))]
    [REQUIRE_COMPONENT(typeof(ShipBasedDamagableObject))]
    [REQUIRE_COMPONENT(typeof(ShipWeapon))]
    [REQUIRE_COMPONENT(typeof(PlayerSkills))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    public sealed class MmoActor : Actor, IOperationHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private PlayerShip mShip;
        private PlayerTarget mTarget;
        private PlayerCharacterObject mCharacter;
        private AIState mAI;
        private RaceableObject mRace;
        private MmoMessageComponent mMessage;
        private ShipBasedDamagableObject mDamagable;
        private ShipWeapon mWeapon;
        private PlayerSkills mSkills;
        private PlayerBonuses mBonuses;
        private PassiveBonusesComponent mPassiveBonuses;


        //public GameApplication application { get; private set; }

        private ActionExecutor                              actionExecutor;
        private ServerInventory                             inventory;
        private WorkhouseStation                            station;
        private ExitWorkshopSavedInfo                       workshopSaveInfo;

        
        //private EventConnectionManager                      eventConnections;
        private ChatManager                                 chat;

        //private PlayerCooperativeGroupController            groupController;
        private bool                                        chestCreated = false;
        private float                                       chestLife;
        
        private Dictionary<OperationCode, BasePlayerOperationHandler> operationHandlers;
        private MoveOperationHandler moveHandler;
        private ExecActionOperationHandler execActionHandler;

        private float printPropertiesInterval = 25;
        private float printPropertiesTimer = 0f;

        public override void Awake() {
            log.Info("player Awake()");
            operationHandlers = new Dictionary<OperationCode, BasePlayerOperationHandler>();
            operationHandlers.Add(OperationCode.AddInterestArea, new AddInterestAreaOperationHandler());
            operationHandlers.Add(OperationCode.AttachInterestArea, new AttachInterestAreaOperationHandler());
            operationHandlers.Add(OperationCode.DestroyItem, new DestroyItemOperationHandler());
            operationHandlers.Add(OperationCode.DetachInterestArea, new DetachInterestAreaOperationHandler());
            operationHandlers.Add(OperationCode.ExitWorld, new ExitWorldOperationHandler());
            operationHandlers.Add(OperationCode.EnterWorkshop, new EnterWorkshopOperationHandler());
            operationHandlers.Add(OperationCode.ExitWorkshop, new ExitWorkshopOperationHandler());
            operationHandlers.Add(OperationCode.GetProperties, new GetPropertiesOperationHandler());
            operationHandlers.Add(OperationCode.GetWorlds, new GetWorldsOperationHandler());
            operationHandlers.Add(OperationCode.RequestServerId, new RequestServerIDOperationHandler());
            operationHandlers.Add(OperationCode.GetShipModel, new GetShipModelOperationHandler());


            moveHandler = new MoveOperationHandler();
            operationHandlers.Add(OperationCode.Move, moveHandler);

            operationHandlers.Add(OperationCode.MoveInterestArea, new MoveInterestAreaOperationHandler());
            operationHandlers.Add(OperationCode.RaiseGenericEvent, new RaiseGenericEventOperationHandler());
            operationHandlers.Add(OperationCode.RemoveInterestArea, new RemoveInterestAreaOperationHandler());
            operationHandlers.Add(OperationCode.SetProperties, new SetPropertiesOperationHandler());
            operationHandlers.Add(OperationCode.SetViewDistance, new SetViewDistanceOperationHandler());
            operationHandlers.Add(OperationCode.SpawnItem, new SpawnItemOperationHandler());
            operationHandlers.Add(OperationCode.SubscribeItem, new SubscribeItemOperationHandler());
            operationHandlers.Add(OperationCode.UnsubscribeItem, new UnsubscribeItemOperationHandler());

            execActionHandler = new ExecActionOperationHandler();
            operationHandlers.Add(OperationCode.ExecAction, execActionHandler);
            actionExecutor = new ActionExecutor(this);
            inventory = new Inventory.ServerInventory(30);
            station = new WorkhouseStation();
            //eventConnections = new EventConnectionManager();
            chat = new ChatManager(resource.ServerInputs.GetValue<int>("max_chat_cache_size"));
            //groupController = new PlayerCooperativeGroupController(this);
            chestLife = resource.ServerInputs.GetValue<float>("chest_life");

            
        }

        public override void Start() {
            mShip = RequireComponent<PlayerShip>();
            mTarget = RequireComponent<PlayerTarget>();
            mCharacter = RequireComponent<PlayerCharacterObject>();
            mAI = RequireComponent<AIState>();
            mRace = RequireComponent<RaceableObject>();
            mMessage = RequireComponent<MmoMessageComponent>();
            mDamagable = RequireComponent<ShipBasedDamagableObject>();
            mWeapon = RequireComponent<ShipWeapon>();
            mSkills = RequireComponent<PlayerSkills>();
            mBonuses = RequireComponent<PlayerBonuses>();
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();

            mCharacter.SetCharacterId((string)nebulaObject.Tag((byte)PlayerTags.CharacterId));
            mCharacter.SetCharacterName((string)nebulaObject.Tag((byte)PlayerTags.Name));
            printPropertiesTimer = printPropertiesInterval;

            if(GameApplication.Instance.serverActors.ContainsKey(nebulaObject.Id)) {
                MmoActor old;
                if( GameApplication.Instance.serverActors.TryRemove(nebulaObject.Id, out old)) {
                    log.Info("successfully remove actor before replacing with new [red]");
                }
            }
            if( GameApplication.Instance.serverActors.TryAdd(nebulaObject.Id, this) ) {
                log.Info("successfully added actor to server actors [red]");
            }

            //create chest on killing when player die
            mDamagable.SetCreateChestOnKilling(true);
        }



        public void Load() {
            log.InfoFormat("MmoActor Load() [dy]");

            bool isNew = false;
            PlayerCharacter dbCharacter = CharacterDatabase.instance.LoadCharacter(mCharacter.characterId, resource as Res, out isNew);
            if (!isNew) {
                SetPlayerCharacter(dbCharacter);
            } else {
                this.name = (string)nebulaObject.Tag((byte)PlayerTags.Name);
                GetComponent<PlayerCharacterObject>().SetExp(0);
                GetComponent<PlayerShip>().SetStartModel((Hashtable)nebulaObject.Tag((byte)PlayerTags.Model));
                GetComponent<PlayerCharacterObject>().SetWorkshop((byte)(int)nebulaObject.Tag((byte)PlayerTags.Workshop));
                GetComponent<RaceableObject>().SetRace((byte)(int)nebulaObject.Tag((byte)PlayerTags.Race));
                GetComponent<PlayerCharacterObject>().SetLogin((string)nebulaObject.Tag((byte)PlayerTags.Login));

                CharacterDatabase.instance.SaveCharacter(mCharacter.characterId, GetPlayerCharacter());
            }

            switch ((Race)(byte)(int)nebulaObject.Tag((byte)PlayerTags.Race)) {
                case Race.Humans:
                    GetComponent<PlayerCharacterObject>().SetFraction(FractionType.PlayerHumans);
                    break;
                case Race.Borguzands:
                    GetComponent<PlayerCharacterObject>().SetFraction(FractionType.PlayerBorguzands);
                    break;
                case Race.Criptizoids:
                    GetComponent<PlayerCharacterObject>().SetFraction(FractionType.PlayerCriptizids);
                    break;
            }


        }

        
        private void AddStartItemToInventory() {
            Workshop workshop = (Workshop)(byte)(int)nebulaObject.Tag((byte)PlayerTags.Workshop);
            SchemeDropper schemeDropper = new SchemeDropper(workshop, 1, resource, ObjectColor.white);
            MaterialObject material = new MaterialObject("CraftOre0001", workshop, 1, resource.Materials.Ore("CraftOre0001"));
            SchemeObject scheme = schemeDropper.Drop() as SchemeObject;
            scheme.ReplaceCraftingMaterials(new Dictionary<string, int> { { "CraftOre0001", 1 } });
            Inventory.Add(scheme, 1);
            Inventory.Add(material, 5);
            log.InfoFormat("adding tutorial crafting materials at start yellow");
        }

        public void LoadOther() {

            try {
                log.InfoFormat("mmoActor LoadOther() [dy]");
                var dbInventory = InventoryDatabase.instance.LoadInventory(mCharacter.characterId, resource as Res);
                Inventory.Replace(dbInventory);
                Inventory.ChangeMaxSlots(mShip.holdCapacity);

                bool stationIsNew = false;
                var dbStation = StationDatabase.instance.LoadStation(mCharacter.characterId, resource as Res, out stationIsNew);
                Station.SetInventory(dbStation.StationInventory);
                Station.SetPetSchemeAdded(dbStation.petSchemeAdded);

                if (false == Station.petSchemeAdded) {
                    AddPetSchemeToNewPlayer();
                    Station.SetPetSchemeAdded(true);
                }
                UpdateCharacterOnMaster();
                log.InfoFormat("player = {0} started at position = {1}", GetComponent<PlayerCharacterObject>().login, transform.position);
            } catch(Exception e) {
                log.InfoFormat(e.Message);
                log.ErrorFormat(e.StackTrace);
            }
        }

        private void AddPetSchemeToNewPlayer() {
            if(mRace == null ) {
                mRace = GetComponent<RaceableObject>();
            }
            Race race = (Race)mRace.race;
            string petModel = resource.petParameters.defaultModels[race];
            var petScheme = new PetSchemeObject(petModel, PetColor.gray);
            Station.StationInventory.Add(petScheme, 1);
            EventOnStationHoldUpdated();
        }

        public void UpdateCharacterOnMaster() {
            GameApplication.Instance.MasterUpdateCharacter(nebulaObject.Id,
                mCharacter.characterId, GetPlayerCharacter(),
                mShip.shipModel.ModelHash(),
                (nebulaObject.world as MmoWorld).Name,
                mCharacter.exp);
        }


        public void SetApplication(GameApplication app) {
            //application = app;
        }


        public void SetPlayerCharacter(PlayerCharacter character) {
            this.name = character.Name;
            GetComponent<PlayerCharacterObject>().SetCharacterId(character.CharacterId);
            GetComponent<PlayerCharacterObject>().SetExp(character.Exp);
            GetComponent<PlayerShip>().SetStartModel(character.Model);
            GetComponent<PlayerCharacterObject>().SetWorkshop((byte)character.Workshop);
            GetComponent<RaceableObject>().SetRace((byte)character.Race);

            if (string.IsNullOrEmpty(character.Login)) {
                character.Login = (string)nebulaObject.Tag((byte)PlayerTags.Login);
            }
            GetComponent<PlayerCharacterObject>().SetLogin(character.Login);
            GetComponent<PlayerCharacterObject>().SetCharacterName(character.Name);
        }

        public PlayerCharacter GetPlayerCharacter() {
            return new PlayerCharacter {
                CharacterId = mCharacter.characterId,
                Exp = mCharacter.exp,
                Model = mShip.shipModel.ModelHash(),
                Name = name,
                Race = mRace.race,
                Workshop = mCharacter.workshop
            };
        }

        public WorkhouseStation Station {
            get {
                return this.station;
            }
        }

        public ChatManager Chat {
            get {
                return this.chat;
            }
        }

        [Obsolete("Use instead application")]
        public GameApplication Application {
            get {
                return GameApplication.Instance;
            }
        }

        public GameApplication application {
            get {
                return GameApplication.Instance;
            }
        }

        public override void OnQuit() {
            CL.Out(LogFilter.PLAYER, "Player {0} OnQuit()".f(name));
            //remove from group if in group
            //this.GroupController().OnExitGame(GameApplication.PlayerGroups);
            mTarget.Clear();
            mAI.SetControlState(PlayerState.Idle);
            base.OnQuit();
            

            //PlayerCharacter ch = new PlayerCharacter { CharacterId = mCharacter.characterId, Exp = mCharacter.exp, Model = mShip.shipModel.ModelHash(), Name = name, Race = mRace.race, Workshop = mCharacter.workshop };
            //application.MasterUpdateCharacter(nebulaObject.Id, mCharacter.characterId, ch, ch.Model, (nebulaObject.world as MmoWorld).Name, ch.Exp);
            UpdateCharacterOnMaster();



        }

        public void Respawn()
        {
            mDamagable.damagers.Clear();
            mDamagable.Respawn();
            mBonuses.Respawn();
            mBonuses.Respawn();
            mDamagable.SetIgnoreDamageInterval(30);
            mDamagable.SetIgnoreDamageAtStart(true);

            this.chestCreated = false;
            log.InfoFormat("PLAYER: health {0}", mDamagable.health);
            log.InfoFormat("PLAYER: destroyed: {0}", (bool)(nebulaObject));
        }

        public ServerInventory Inventory
        {
            get
            {
                return this.inventory;
            }
        }

        public ActionExecutor ActionExecutor {
            get {
                return actionExecutor;
            }
        }

        #region Public Methods
        
        /// <summary>
        ///   Handles operations <see cref = "CreateWorld" /> and <see cref = "EnterWorld" />.
        /// </summary>
        /// <param name = "request">
        ///   The request.
        /// </param>
        /// <returns>
        ///   An <see cref = "OperationResponse" /> with error code <see cref = "ReturnCode.InvalidOperation" />.
        /// </returns>
        public static OperationResponse InvalidOperation(OperationRequest request)
        {
            string debugMessage = "InvalidOperation: " + (OperationCode)request.OperationCode;
            return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperation, DebugMessage = debugMessage };
        }



        /// <summary>
        ///   Adds the <paramref name = "item" /> to the owned items.
        /// </summary>
        /// <param name = "item">
        ///   The mmo item.
        /// </param>
        /// <exception cref = "ArgumentException">
        ///   The item <see cref = "MmoItem.Owner" /> must be this actor.
        /// </exception>
        public void AddItem(MmoItem item)
        {
            if (item.Owner != this)
            {
                throw new ArgumentException("foreign owner forbidden");
            }

            SetAvatar(item);
        }

        #endregion

        private const float UNDER_ATTACK_EXP_INTERVAL = 10 * 60;
        private const int EXP_FOR_ATTACK_WORLD_STAY = 50;

        private bool needDispose = false;
        private float needDisposeTimer = 0;
        private Thread disposeThread = null;
        private float mUnderAttackTimer = UNDER_ATTACK_EXP_INTERVAL;

        public void OnDisconnect(PeerBase peer )
        {
            log.InfoFormat("Player.OnDisconnect():{0} object state = {1}", GetComponent<PlayerCharacterObject>().login, (bool)nebulaObject);
            //return this if any bugs
            //this.Dispose();

            MmoActor old;
            if (GameApplication.Instance.serverActors.TryRemove(nebulaObject.Id, out old)) {
                log.InfoFormat("successfully removed actor from server actors");
            }

            var playerLoader = GetComponent<PlayerLoaderObject>();
            playerLoader.Save(true);

            needDispose = true;
            if(disposeThread == null ) {
                disposeThread = new Thread(MakeDispose);
                disposeThread.Start();
            }
            ((Peer)peer).SetCurrentOperationHandler(null);
            peer.Dispose();
            
        }

        private float mSubZoneTimer = 5;

        public override void Update(float deltaTime) {
            printPropertiesTimer -= deltaTime;
            if (printPropertiesTimer < 0f) {
                printPropertiesTimer = printPropertiesInterval;
            }

            var world = nebulaObject.world as MmoWorld;
            if(world != null ) {
                if (!world.underAttack) {
                    mUnderAttackTimer = UNDER_ATTACK_EXP_INTERVAL;
                } else {
                    mUnderAttackTimer -= deltaTime;
                    if(mUnderAttackTimer <= 0f) {
                        mUnderAttackTimer = UNDER_ATTACK_EXP_INTERVAL;
                        mCharacter.AddExp(EXP_FOR_ATTACK_WORLD_STAY);
                        log.InfoFormat("added exp = {0} to {1} for under attack world state", EXP_FOR_ATTACK_WORLD_STAY, mCharacter.login);
                    }
                }
            }

            if(nebulaObject.subZone != -1) {
                mSubZoneTimer -= deltaTime;
                if(mSubZoneTimer < 0f ) {
                    mSubZoneTimer = 5;
                    int subZone = world.ResolvePositionSubzone(transform.position);
                    if(subZone != nebulaObject.subZone) {
                        SetNewSubZone(subZone);
                    }
                }
            }
        }

        public void SetNewSubZone(int subZone) {
            nebulaObject.SetSubZone(subZone);
            mMessage.SendSubZoneUpdate(EventReceiver.OwnerAndSubscriber, nebulaObject.subZone);
        }

        private void MakeDispose() {
            Thread.Sleep(10 * 1000);
            //remove this if any bugs and return this to OnDisconnect()
            if (needDispose) {
                needDispose = false;
                needDispose = false;
                log.InfoFormat("Player.Dispose():{0} object state = {1}", GetComponent<PlayerCharacterObject>().login, (bool)nebulaObject);
                this.Dispose();
            }
        }


        public void OnDisconnectByOtherPeer(PeerBase peer )
        {

            this.ExitWorld();

            peer.RequestFiber.Enqueue(() => peer.RequestFiber.Enqueue(peer.Disconnect));
            CL.Out(LogFilter.PLAYER, "Player {0} disconnected by other peer".f(name));
        }

        public OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {

            OperationCode opCode = (OperationCode)operationRequest.OperationCode;
            if(opCode == OperationCode.Move) {
                return moveHandler.Handle(this, operationRequest, sendParameters);
            } else if(opCode == OperationCode.ExecAction) {
                return execActionHandler.Handle(this, operationRequest, sendParameters);
            } else if(operationHandlers.ContainsKey(opCode)) {
                return operationHandlers[opCode].Handle(this, operationRequest, sendParameters);
            }

            return new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (int)ReturnCode.OperationNotSupported,
                DebugMessage = "OperationNotSupported: " + operationRequest.OperationCode
            };
        }



        public void EventOnInventoryItemAdded(string containerId, byte containerType, List<IInventoryObject> addedObjects ) { 
            //GetInventory().TryGetItem()

            List<Hashtable> items = new List<Hashtable>();
            foreach(var addedObj in addedObjects ) {
                ServerInventoryItem resultItem = null;
                if(this.Inventory.TryGetItem(addedObj.Type, addedObj.Id, out resultItem)){
                    items.Add(resultItem.GetInfo());
                }
            }
            Hashtable eventData = new Hashtable();
            eventData.Add((int)SPC.Target, containerId);
            eventData.Add((int)SPC.TargetType, containerType );
            eventData.Add((int)SPC.Items, items.ToArray());

            var eventInstance = new ItemGeneric { 
                ItemId = Avatar.Id,
                ItemType = Avatar.Type,
                CustomEventCode = (byte)CustomEventCode.InventoryItemsAdded,
                EventData = eventData,
                GameReferenceId = nebulaObject.Id,
                CharacterId = mCharacter.characterId
            };

            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters{ Unreliable = true, ChannelId = Settings.ItemEventChannel };
            ((IMmoItem)Avatar).ReceiveEvent(eData, sendParameters);
        }



        /// <summary>
        /// sends when player ship model updated
        /// </summary>
        public void EventOnShipModelUpdated() {

            Hashtable eventData = new Hashtable();

            //get array of all model slots state
            eventData.Add((int)SPC.Info, mShip.shipModel.GetInfo());

            var eventInstance = new ItemGeneric { 
                ItemId = string.Empty,
                ItemType = ItemType.Avatar.toByte(),
                CustomEventCode = (byte)CustomEventCode.ShipModelUpdated,
                EventData = eventData,
                GameReferenceId =nebulaObject.Id,
                CharacterId = mCharacter.characterId
            };

            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            this.Peer.SendEvent(eData, sendParameters);
        }

        public void EventOnWeaponUpdated()
        {
            Hashtable eventData = new Hashtable();

            //get array of all model slots state
            eventData.Add((int)SPC.Info, mWeapon.hasWeapon ? mWeapon.GetInfo() : new Hashtable());

            var eventInstance = new ItemGeneric
            {
                ItemId = string.Empty,
                ItemType = ItemType.Avatar.toByte(),
                CustomEventCode = (byte)CustomEventCode.PlayerWeaponUpdated,
                EventData = eventData,
                GameReferenceId = nebulaObject.Id,
                CharacterId = mCharacter.characterId
            };

            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            this.Peer.SendEvent(eData, sendParameters);
        }


        /// <summary>
        /// Send info to client about updated station hold
        /// </summary>
        public void EventOnStationHoldUpdated()
        {
            //Hashtable eventData = new Hashtable();
            //eventData.Add(GenericEventProps.info, this.station.Hold.GetInfo());
            var eventInstance = new ItemGeneric { 
               ItemId = string.Empty,
               ItemType = ItemType.Avatar.toByte(),
               CustomEventCode = (byte)CustomEventCode.StationHoldUpdated,
               EventData = this.station.GetInfo(), //eventData
                GameReferenceId = nebulaObject.Id,
                CharacterId = mCharacter.characterId
            };

            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            this.Peer.SendEvent(eData, sendParameters);
            //application.Save(nebulaObject.Id, mCharacter.characterId, Station, false);
        }

        public void EventOnInventoryUpdated()
        {
            //GetInventory().TryGetItem()
            var eventInstance = new ItemGeneric
            {
                ItemId = string.Empty,
                ItemType = ItemType.Avatar.toByte(),
                CustomEventCode = (byte)CustomEventCode.InventoryUpdated,
                EventData = ((IInfo)this.inventory).GetInfo(),
                GameReferenceId = nebulaObject.Id,
                CharacterId = mCharacter.characterId
            };

            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            this.Peer.SendEvent(eData, sendParameters);
        }

        /// <summary>
        /// send event on client when player info updated
        /// </summary>
        public void EventOnPlayerInfoUpdated()
        {
            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = ItemType.Avatar.toByte(),
                CustomEventCode = (byte)CustomEventCode.PlayerInfoUpdated,
                EventData = GetPlayerCharacter().GetInfo(),
                GameReferenceId = nebulaObject.Id,
                CharacterId = mCharacter.characterId
            };

            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            this.Peer.SendEvent(eData, sendParameters);
        }

        public void EventOnSkillsUpdated()
        {

            var eventInstance = new ItemGeneric {
                ItemId = nebulaObject.Id,
                ItemType = (byte)ItemType.Avatar,
                CustomEventCode = (byte)CustomEventCode.SkillsUpdated,
                EventData = mSkills.GetInfo(),
                GameReferenceId = nebulaObject.Id,
                CharacterId = mCharacter.characterId

            };
            var eData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            SendParameters sendParameters = new SendParameters { Unreliable = false, ChannelId = Settings.ItemEventChannel };
            this.Peer.SendEvent(eData, sendParameters);
        }


        private MethodReturnValue CheckAccess(Item item)
        {
            if (item.Disposed)
            {
                return MethodReturnValue.Fail((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            if (((IMmoItem)item).GrantWriteAccess(this))
            {
                return MethodReturnValue.Ok;
            }

            return MethodReturnValue.Fail((int)ReturnCode.ItemAccessDenied, "ItemAccessDenied");
        }

        public void ExecItemOperation(Func<OperationResponse> operation, SendParameters sendParameters)
        {
            OperationResponse result = operation();
            if (result != null)
            {
                this.Peer.SendOperationResponse(result, sendParameters);
            }
        }

        private void ExitWorld()
        {
            log.InfoFormat("Player.ExitWorld(): {0}  object state = {1}", GetComponent<PlayerCharacterObject>().login, (bool)nebulaObject);
            //reset target when exiting world
            mTarget.Clear();

            var worldExited = new WorldExited { WorldName = ((MmoWorld)this.World).Name };
            this.Dispose();

            // set initial handler
            ((MmoPeer)this.Peer).SetCurrentOperationHandler((MmoPeer)this.Peer);

            var eventData = new EventData((byte)EventCode.WorldExited, worldExited);

            UpdateCharacterOnMaster();


            //var loader = GetComponent<PlayerLoaderObject>();
            //if(loader != null) {
            //    loader.Save(true);
            //}
            // use item channel to ensure that this event arrives in correct order with move/subscribe events
            this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

            //CL.Out(LogFilter.PLAYER, "ExitWorld() Player {0} from world {1}".f(name, ((MmoWorld)this.World).Name));
        }

        public void SetWorkshopSaveInfo(ExitWorkshopSavedInfo info) {
            workshopSaveInfo = info;
        }

        public void SetWorkshopStatus(bool inWorkshop) {
            if(workshopSaveInfo != null) {
                workshopSaveInfo.NowInWorkshop = inWorkshop;
            }
        }

        public ExitWorkshopSavedInfo WorkshopSavedInfo {
            get {
                return workshopSaveInfo;
            }
        }

        public bool atStation {
            get {
                if(workshopSaveInfo != null ) {
                    return workshopSaveInfo.NowInWorkshop;
                }
                return false;
            }
        }

        public bool atSpace {
            get {
                return (false == atStation);
            }
        }

        #region ICombatActor


        #endregion

        public Vector3 Position
        {
            get
            {
                if (Avatar != null)
                {
                    return Avatar.transform.position;
                }
                return Vector3.Zero;
            }
        }




        public void Death() {

            log.InfoFormat("MmoActor.Death()...destroying pets");

            

            Avatar.SendEventShipDestroyed(true);
            mTarget.Clear();
            if(!chestCreated) {
                var damagable = GetComponent<DamagableObject>();
                if (damagable.killed && damagable.createChestOnKilling) {
                    float chestTime = resource.ServerInputs.playerLootChestLifeTime;

                    if (Rand.Float01() > mPassiveBonuses.chanceDontDropLootBonus) {
                        var allInventoryItems = GetAllInventoryItems();
                        ObjectCreate.SharedChest(nebulaObject.mmoWorld(), transform.position, chestTime, allInventoryItems).AddToWorld();
                        ClearInventory();
                        log.InfoFormat("create shared chest when player die, items count = {0}  on time = {1} [purple]", allInventoryItems.Count, chestTime);
                        GetComponent<PlayerLoaderObject>().SaveInventory();

                    } else {
                        log.InfoFormat("you dont drop loot via passive bonus [red]");
                    }
                }
                chestCreated = true;
            }
        }



        private ConcurrentBag<ServerInventoryItem> GetAllInventoryItems() {
            ConcurrentBag<ServerInventoryItem> items = new ConcurrentBag<ServerInventoryItem>();
            foreach(var pFiltered in Inventory.Items) {
                foreach(var pItem in pFiltered.Value) {
                    items.Add(pItem.Value);
                }
            }
            return items;
        }

        private void ClearInventory() {
            Inventory.Clear();
            EventOnInventoryUpdated();
        }

        public Hashtable Buffs() {
            float time = Time.curtime();

            var buffs = new Hashtable();
            foreach(var b in mBonuses.bonuses) {
                buffs.Add((byte)b.Key, mBonuses.Value(b.Key));
            }
            return buffs;
        }


        public override int behaviourId {
            get {
                return (int)ComponentID.Player;
            }
        }
    }
}

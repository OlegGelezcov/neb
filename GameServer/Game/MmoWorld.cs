namespace Space.Game {
    using Common;
    using ExitGames.Logging;
    using GameMath;
    using Nebula.Database;
    using Nebula.DSL;
    using Nebula.Engine;
    using Nebula.Game;
    using Nebula.Game.Components;
    using ServerClientCommon;
    using Space.Game.Resources;
    using Space.Game.Resources.Zones;
    using Space.Server;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class MmoWorld : GridWorld, IInfoSource, INpcOwner, IConditionContext
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public const int MAX_UNDER_ATTACK_INTERVAL = 3600;
        private const int INVULENRABLE_INTERVAL = 3600;

        private readonly string name;
        private readonly ZoneData zone;
        public Race ownedRace { get; private set; }
        private Dictionary<string, NpcGroup> npcGroups;
        private Res resource;
        public  WorldAsteroidManager asteroidManager { get; private set; } 
        public WorldNpcManager npcManager { get; private set; }
        public MmoWorldNebulaObjectManager nebulaObjectManager { get; private set; }
        private readonly object lockObject = new object();
        public bool underAttack { get; private set; }

        //after gour under attack world in invulnerable state
        public bool invulnerable { get; private set; }

        private float mUnderAttackTimer;
        private const float UNDER_ATTACK_INTERVAL = 5 * 60;

        //count interval the world under attack
        private float mUnderAttackDuration = 0f;

        private float mInvulnerableTimer = 0f;
        private int mAttackedRace;

        private WorldState mWorldState;
        private bool mStateRestored = false;


        public MmoWorld(string name, Vector minCorner, Vector maxCorner, Vector tileDimensions, Res resource)
            : base(minCorner, maxCorner, tileDimensions, new MmoItemCache() )
        {
            try
            {
                this.resource = resource;
                this.name = name;
                this.zone = (Resource().Zones.ExistZone(this.name) ? Resource().Zones.Zone(this.name) : Resource().Zones.Default(this.name));
                this.ownedRace = (this.zone != null) ? this.zone.InitiallyOwnedRace : Race.None;
                underAttack = false;

                this.InitializeNpcGroups(Resource());
                asteroidManager = new WorldAsteroidManager(this);
                npcManager = new WorldNpcManager(this);
                nebulaObjectManager = new MmoWorldNebulaObjectManager(this);

                //load world info from database
                LoadWorldInfo();
                LoadWorldState();

            }
            catch (System.Exception eee)
            {
                CL.Out(LogFilter.WORLD, "Exception in world constructor");
                CL.Out(eee.Message);
                CL.Out(eee.StackTrace);
            }
        }

        //load world data from database
        private void LoadWorldInfo() {
            //get document
            var worldDocument = GameApplication.Instance.DatabaseManager.GetWorld(Zone.Id);
            
            if(worldDocument == null ) {

                //if save not found 
                GameApplication.Instance.DatabaseManager.SetWorld(this);
            }  else {
                if(worldDocument.info == null ) {
                    log.ErrorFormat("database world = {0} document info is null", Zone.Id);
                    GameApplication.Instance.DatabaseManager.SetWorld(this);
                    return;
                }
                ownedRace = (Race)(byte)worldDocument.info.currentRace;
                underAttack = worldDocument.info.underAttack;
                mAttackedRace = worldDocument.info.attackRace;
            }
        }

        private void LoadWorldState() {
            mWorldState = GameApplication.Instance.DatabaseManager.GetWorldState(Zone.Id);
            if(mWorldState == null ) {
                log.InfoFormat("world state (null) for world = {0} create new [dy]", Zone.Id);
                mWorldState = new WorldState();
                mWorldState.Init(Zone.Id);
            } else {
                mWorldState.CheckMembers();
            }


        }

        public void AddDestroyedObjectToSave(string databaseID ) {
            if(mWorldState != null ) {
                mWorldState.AddDestroyedObject(databaseID);
            }
        }

        public bool HasDestroyedObject(string databaseID) {
            if(mWorldState == null) {
                return false;
            }
            return mWorldState.HasDestroyedObject(databaseID);
        }

        public void SaveWorldState() {
            if(mWorldState != null ) {
                mWorldState.FillSaves(this);
                GameApplication.Instance.DatabaseManager.worldStates.Save(mWorldState);
                log.InfoFormat("saving world state = {0} [dy]", Zone.Id);
            }
        }


        private void InitializeNpcGroups(IRes resource)
        {
            this.npcGroups = new Dictionary<string, NpcGroup>();

            foreach(string npcGroupId in this.Zone.NpcGroups)
            {
                NpcGroupData npcGroupData = resource.NpcGroups.GroupData(npcGroupId);
                if (npcGroupData == null)
                    continue;
                NpcGroup npcGroup = new NpcGroup(this, npcGroupData);
                this.npcGroups.Add(npcGroupData.Id, npcGroup);
            }
        }

        public Hashtable GetStats()
        {
            Hashtable stats = new Hashtable();
            stats.Add("Total count", this.TotalCount());
            return stats;
        }

        public int TotalCount()
        {
            return this.ItemCache.TotalCount();
        }

        public int playerCountOnStartFrame {
            get;
            private set;
        } = 0;

        public int playerCount {
            get {
                return ItemCache.GetCount((byte)ItemType.Avatar);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        //public int playerCount { get; private set; } = 0;


        public void Initialize()
        {
            foreach(var eventData in Zone.Events) {
                var eventObject = ObjectCreate.Event(this, eventData);
                eventObject.AddToWorld();
                log.InfoFormat("event {0} added to world", eventObject.Id);
            }
        }

        private void CreatePlanets()
        {
            if(this.Zone == null )
            {
                log.ErrorFormat("error: world {0} don't have ZoneData object", this.Name);
                return;
            }

            var planets = this.Zone.Planets;

            foreach(var planetInfo in planets )
            {
                //this.fiber.Enqueue(() =>
                //    {
                //        //var planetObject = new PlanetObject(planetInfo.Id, this, planetInfo.Position.ToArray().ToVector(false), 
                //        //    GameObjectEventInfo.Default, planetInfo.Name, planetInfo);
                //        //planetObject.AddToWorld();
                //    });
            }
        }

        private void UpdateNpcGroups(float time)
        {
            lock(((ICollection)this.npcGroups).SyncRoot)
            {
                foreach(var pGroup in this.npcGroups)
                {
                    pGroup.Value.Update(time);
                }
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        private float[] GetArray(string str)
        {
            string[] strArr = str.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<float> result = new List<float>();
            foreach (string s in strArr) {
                float f;
                if (float.TryParse(s, out f))
                    result.Add(f);
            }
            return result.ToArray();
        }


        public ZoneData Zone
        {
            get
            {
                return this.zone;
            }
        }

        public void SetCurrentRace(Race race) {
            Race previousRace = ownedRace;
            ownedRace = race;
            //save changes to database
            GameApplication.Instance.DatabaseManager.SetWorld(this);

            log.InfoFormat("world = {0} set race owned to = {1} [red]", Zone.Id, ownedRace);
            GameApplication.Instance.updater.SendS2SWorldRaceChanged(Zone.Id, (byte)previousRace, (byte)ownedRace);
        }

        public void SetAttackRace(Race race) {
            mAttackedRace = (byte)race;
            GameApplication.Instance.DatabaseManager.SetWorld(this);
            log.InfoFormat("world = {0} set attacked race = {1} [purple]", Zone.Id, ownedRace);
        }

        public Race attackedRace {
            get {
                return (Race)(byte)mAttackedRace;
            }
        }
        public void SetUnderAttack(bool inUnderAttack) {

            //if not under attack reset under attack duration
            if(!inUnderAttack) {
                mUnderAttackDuration = 0f;
                SetAttackRace(Race.None);
            } else {
                //if we were not attacked and and make attacked than reset duration
                if(!underAttack && inUnderAttack) {
                    mUnderAttackDuration = 0f;
                }
            }

            //when invulnerable active, then reset attack duration
            if(invulnerable) {
                mUnderAttackDuration = 0f;
            }

            underAttack = inUnderAttack;
            //save changes to database
            GameApplication.Instance.DatabaseManager.SetWorld(this);

            if(underAttack) {
                mUnderAttackTimer = UNDER_ATTACK_INTERVAL;
            }

            log.InfoFormat("set world = {0} under attack = {1} [purple]", Zone.Id, underAttack);
        }



        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.name);
            info.Add((int)SPC.OwnedRace, this.ownedRace.toByte());
            info.Add((int)SPC.ZoneInfo, (this.zone != null ) ? this.zone.GetInfo(): new Hashtable());
            return info;
        }



        //private ReaderWriterLockSlim lockSlim;

        /// <summary>
        /// Get bot items of selected sub type and and filter
        /// </summary>
        /// <param name="subType"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Dictionary<string, Item> GetBotItems(BotItemSubType subType, Func<Item, bool> filter) {
            var concurrentDict = GetBotItemsConcurrent(subType, filter);
            Dictionary<string, Item> result = new Dictionary<string, Item>();
            foreach (var pair in concurrentDict) {
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }

        public ConcurrentDictionary<string, Item> GetBotItemsConcurrent(BotItemSubType subType, Func<Item, bool> filter) {
            var items = ItemCache.GetItems((byte)ItemType.Bot);

            ConcurrentDictionary<string, Item> result = new ConcurrentDictionary<string, Item>();
            foreach(var p in items) {
                var bot = p.Value.GetComponent<BotObject>();
                if(bot.botSubType == (byte)subType) {
                    if (filter(p.Value)) {
                        result.TryAdd(p.Value.Id, p.Value);
                    }
                }
            }
            return result;
        }

        public Dictionary<string, MmoActor> GetMmoActors(Func<MmoActor, bool> filter)
        {
            lock(lockObject)
            {
                return this.ItemCache.GetItems(ItemType.Avatar.toByte()).
                    Where(p => filter(p.Value.GetComponent<MmoActor>())).
                    ToDictionary(p => p.Key, p => p.Value.GetComponent<MmoActor>());
            }
        }

        public ConcurrentDictionary<string, MmoActor> GetMmoActorsConcurrent(Func<MmoActor, bool> filter) {
            ConcurrentDictionary<string, MmoActor> result = new ConcurrentDictionary<string, MmoActor>();
            foreach(var pair in ItemCache.GetItems((byte)ItemType.Avatar)) {
                var a = pair.Value.GetComponent<MmoActor>();
                if (filter(a)) {
                    result.TryAdd(pair.Key, a);
                }
            }
            return result;
        }

        public ConcurrentDictionary<string, Item> GetItems(Func<Item, bool> filter) {
            return ItemCache.GetItems(filter);
        }

        public ConcurrentDictionary<string, Item> GetItems(ItemType type, Func<Item, bool> filter) {
            return ItemCache.GetItemsConcurrent((byte)type, filter);
        }

        public Item GetItem(Func<Item, bool> filter) {
            return ItemCache.GetItem(filter);
        }

        public bool Contains(byte type, string id) {
            return ItemCache.Contains(type, id);
        }

        public ConcurrentDictionary<string, T> FindObjectsOfType<T>() where T : NebulaBehaviour {
            var foundedDict =  GetItems((it) => {
                if (it.GetComponent<T>()) {
                    return true;
                }
                return false;
            });
            ConcurrentDictionary<string, T> result = new ConcurrentDictionary<string, T>();
            foreach(var p in foundedDict) {
                result.TryAdd(p.Key, p.Value.GetComponent<T>());
            }
            return result;
        }

        public int ResolvePositionSubzone(Vector3 position) {
            var subZones = FindObjectsOfType<SubZoneComponent>();
            foreach(var subZone in subZones) {
                if(Vector3.Distance(subZone.Value.transform.position, position) <= subZone.Value.innerRadius) {
                    return subZone.Value.subZoneID;
                }
            }
            return 0;
        }

        public T FindObjectOfType<T>() where T : NebulaBehaviour {
            var result = GetItem(it => {
                if (it.GetComponent<T>()) {
                    return true;
                }
                return false;
            });
            if(!result) {
                return default(T);
            }
            return result.GetComponent<T>();
        }



        public string GetID()
        {
            return this.zone.Id;
        }


        public override bool TryGetObject(byte objectType, string objectId, out NebulaObject obj) {
            Item item;
            bool result = ItemCache.TryGetItem(objectType, objectId, out item);
            obj = item;
            return result;
        }

        public override void RemoveObject(byte objectType, string objectId) {
            ItemCache.RemoveItem(objectType, objectId);
        }

        public override bool AddObject(NebulaObject obj) {
            return ItemCache.AddItem(obj as Item);
        }

        public override List<NebulaObject> Filter(Func<NebulaObject, bool> filter) {
            return ItemCache.Filter(filter);
        }

        public override IRes Resource() {
            return resource;
        }

        public void Tick(float deltaTime) {
            playerCountOnStartFrame = playerCount;
            ItemCache.Tick(deltaTime);

            asteroidManager.Update(deltaTime);
            npcManager.Update(deltaTime);
            nebulaObjectManager.Update(deltaTime);

            //restore saved objects
            if(!mStateRestored) {
                if(mWorldState != null ) {
                    mStateRestored = true;
                    mWorldState.RestoreObjectsFromSave(this);
                }
            }

            if(underAttack) {
                if(mUnderAttackTimer > 0f) {
                    mUnderAttackTimer -= deltaTime;
                    if(mUnderAttackTimer <=0f ) {
                        SetUnderAttack(false);
                    }
                }

                
                mUnderAttackDuration += deltaTime;
                if(mUnderAttackDuration >= MAX_UNDER_ATTACK_INTERVAL ) {
                    if(!invulnerable) {
                        invulnerable = true;
                        mInvulnerableTimer = INVULENRABLE_INTERVAL;
                        BroadcastMessage("OnInvulnerableChanged");
                        return;
                    }
                    mUnderAttackDuration = 0f;
                }
            }

            if(invulnerable) {
                mInvulnerableTimer -= deltaTime;
                if(mInvulnerableTimer <= 0f ) {
                    invulnerable = false;
                    BroadcastMessage("OnInvulnerableChanged");
                }
            }
        }

        public void BroadcastMessage(string message, object arg = null ) {
            ItemCache.SendMessage(message, arg);
        }


        public void HandleNpcDeath(string npcID) {

        }


        public object GetVariable(string name) {
            switch (name.ToLower()) {
                case "world_race":
                    return ownedRace;
                default:
                    return null;
            }
        }

        public bool HasItem(string ID, byte type) {
            Item it;
            if(ItemCache.TryGetItem(type, ID, out it)) {
                return (it != null);
            }
            return false;
        }

        public void ClearResources() {
            ItemCache.DeleteItems();
        }

        public void SendWorldChangedToPlayers(Hashtable info ) {
            var players = GetItems((item) => item.IsPlayer());
            foreach(var pPlayer in players ) {
                log.InfoFormat("send world race changed event to player at = {0} [red]", Zone.Id);
                pPlayer.Value.MmoMessage().ReceiveWorldRaceChanged(info);
            }
        }
    }
         


}

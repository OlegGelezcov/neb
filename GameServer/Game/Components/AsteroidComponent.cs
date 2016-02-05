using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;
using Photon.SocketServer;
using Space.Game;
using Space.Game.Inventory.Objects;
using Space.Game.Objects;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using Space.Server;
using Space.Server.Events;
using Space.Server.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Game.Components {

    public class AsteroidComponent : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger(); 

        private List<AsteroidContent> mContent = new List<AsteroidContent>();
        private const float UPDATE_MANAGEMENT_INTERVAL = 60;
        private float mUpdateManagementTimer = UPDATE_MANAGEMENT_INTERVAL;

        public List<AsteroidContent> content {
            get {
                return mContent;
            }
            private set {
                mContent = value;
                if(mContent == null) {
                    mContent = new List<AsteroidContent>();
                }
            } }

        public AsteroidData data { get; private set; }

        public ZoneAsteroidInfo zoneAsteroidInfo { get; private set; } 

        public void Init(AsteroidComponentData data) {
            SetData(nebulaObject.resource.Asteroids.Data(data.dataID));
            Generate();
        }

        public override void Start() {
            //log.Info("asteroid Start()");
            //log.InfoFormat("asteroid start = {0} at world = {1} yellow", nebulaObject.Id, nebulaObject.mmoWorld().Name);
        }

        public override void Update(float deltaTime) {
            //if(nebulaObject.IAmBotAndNoPlayers()) {
            //    return;
            //}

            nebulaObject.properties.SetProperty((byte)PS.AsteroidContent, contentRaw);
            nebulaObject.properties.SetProperty((byte)PS.Name, name);

            mUpdateManagementTimer -= deltaTime;
            if(mUpdateManagementTimer <= 0f) {
                mUpdateManagementTimer = UPDATE_MANAGEMENT_INTERVAL;
                var position = transform.position;
                (nebulaObject as Item).Move(new Vector { X = position.X, Y = position.Y, Z = position.Z });
            }
        }

        public void SetContent(List<AsteroidContent> inputContent) {
            content = inputContent;
        }

        public void SetData(AsteroidData inData) {
            data = inData;
        } 

        public void SetZoneAsteroidInfo( ZoneAsteroidInfo info) {
            zoneAsteroidInfo = info;
        }

        public void Generate() {
            content.Clear();
            var materials = (nebulaObject.world as MmoWorld).asteroidManager.asteroidDropper.DropMaterials(data);
            if(materials.Count == 0) {
                log.ErrorFormat("Error: number of materials asteroid at world = {0} is ZERO (AsteroidData is null?)", (nebulaObject.world as MmoWorld).Name);
            }
            foreach(var matPair in materials) {
                if(matPair.Value > 0) {
                    var matData = nebulaObject.world.Resource().Materials.Ore(matPair.Key);
                    if(matData == null ) {
                        continue;
                    }
                    var matObject = new MaterialObject(matData.Id, Workshop.DarthTribe, 1, matData);
                    content.Add(new AsteroidContent { Material = matObject, Count = matPair.Value });
                }
            }
            if(data != null ) {
                nebulaObject.properties.SetProperty((byte)PS.DataId, data.Id);
            }
        }

        public object[] contentRaw {
            get {
                object[] rawArray = new object[content.Count];
                for(int i = 0; i < content.Count; i++) {
                    rawArray[i] = content[i].GetInfo();
                }
                return rawArray;
            }
        }

        public List<string> contentIds {
            get {
                return content.Select(c => c.Material.Id).ToList();
            }
        }

        public AsteroidContent ContentObject(string id) {
            return content.Where(c => c.Material.Id == id).FirstOrDefault();
        }

        public Dictionary<string, InventoryObjectType> contentDictionary {
            get {
                var idType = new Dictionary<string, InventoryObjectType>();
                foreach(var c in content) { idType.Add(c.Material.Id, InventoryObjectType.Material); }
                return idType;
            }
        }

        public void ClearContent() {
            if (content.Count > 0) {
                var collected = new Hashtable();
                foreach (var cont in content) {
                    collected.Add(cont.Material.Id, cont.Count);
                }
            }
            content.Clear();
            nebulaObject.properties.SetProperty((byte)PS.AsteroidContent, contentRaw);
            SendPropertiesEvent();
        }

        public void DestroyIfEmpty() {
            if(content.Count == 0) {
                (nebulaObject as Item).Destroy();
            }
        }

        public void RemoveContentObject(string objId) {
            int index = -1;
            for(int i = 0; i < content.Count; i++) {
                if(content[i].Material.Id == objId) {
                    index = i;
                    break;
                }
            }
            if(index >= 0 ) {
                content.RemoveAt(index);
                nebulaObject.properties.SetProperty((byte)PS.AsteroidContent, contentRaw);
                SendPropertiesEvent();
            }

            
        }

        public bool Contains(string contentID, InventoryObjectType objType ) {
            foreach(var contentObj in content) {
                if(contentObj.Material.Id == contentID && contentObj.Material.Type == objType) {
                    return true;
                }
            }
            return false;
        }

        private void SendPropertiesEvent() {
            var inst = new ItemProperties {
                ItemId = nebulaObject.Id,
                ItemType = nebulaObject.Type,
                PropertiesRevision = nebulaObject.properties.propertiesRevision,
                PropertiesSet = nebulaObject.properties.raw
            };
            var evtData = new EventData((byte)EventCode.ItemProperties, inst);
            SendParameters sendParameters = new SendParameters { ChannelId = Settings.ItemEventChannel };
            var message = new ItemEventMessage(nebulaObject as Item, evtData, sendParameters);
            (nebulaObject as Item).EventChannel.Publish(message);
        }



        public void Death() {
            //log.InfoFormat("Death on asteroid {0} called", nebulaObject.Id);

            if (zoneAsteroidInfo != null) {
                (nebulaObject.world as MmoWorld).asteroidManager.SetDestructionTime(zoneAsteroidInfo.Index, Time.curtime());
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Asteroid;
            }
        }
    }
}

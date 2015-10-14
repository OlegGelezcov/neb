//// Item.cs
//// Nebula
////
//// Created by Oleg Zheleztsov on Thursday, September 10, 2015 10:13:43 PM
//// Copyright (c) 2015 KomarGames. All rights reserved.
////   
//namespace Nebula.Mmo.Items {
//    using System;
//    using System.Collections.Generic;
//    using UnityEngine;
//    using System.Collections;
//    using Common;
//    using Nebula.Mmo.Games;
//    using Nebula.Mmo.Items.Components;
//    using Nebula.Resources;
//    using Nebula.UI;
//    using Nebula.Mmo.Objects;
//    using Nebula.Client;
//    using Unity;

//    public abstract class Item : IObjectInfo {
//        private readonly NetworkGame game;
//        private readonly string id;
//        private List<byte> subscribedInterestAreas;
//        private readonly byte type;
//        private readonly List<byte> visibleInterestAreas;

//        private Dictionary<byte, object> properties;
//        protected GameObject _view;
//        protected NetworkTransformInterpolation _transformInterpolation;
//        protected bool _subscribed;
//        private bool _shipDestroyed;
//        private float movedSpeed = 0f;
//        private ShipModel mShipModel;
//        public int subZoneID { get; private set; }
//        private BaseSpaceObject mComponent;

//        public float[] Position { get; private set; }
//        public float[] Rotation { get; private set; }
//        public float[] PreviousPosition { get; private set; }
//        public float[] PreviousRotation { get; private set; }
//        public int PropertyRevision { get; set; }
//        public event Action<Item> Moved;
//        public ComponentID[] componentIDS { get; private set; }
//        public MmoBonusesComponent bonuses { get; private set; }
//        public MmoBotComponent bot { get; private set; }
//        public MmoCharacterComponent character { get; private set; }
//        public MmoDamagableComponent damagable { get; private set; }
//        public MmoEnergyComponent energy { get; private set; }
//        public MmoModelComponent model { get; private set; }
//        public MmoPlayerAIComponent playerAI { get; private set; }
//        public MmoRaceableComponent raceable { get; private set; }
//        public MmoShipComponent ship { get; private set; }
//        public MmoTargetComponent target { get; private set; }
//        public MmoMovableComponent movable { get; private set; }
//        public MmoWeaponComponent weapon { get; private set; }
//        public bool InterestAreaAttached { get; private set; }
//        public bool IsDestroyed { get; set; }
//        public abstract bool IsMine { get; }
//        public Dictionary<ComponentID, MmoBaseComponent> components { get; set; }
//        public abstract ObjectInfoType InfoType { get; }
//        public abstract string Description { get; }
//        public bool invisible { get; private set; }

//        public virtual BaseSpaceObject Component {
//            get {
//                if (!mComponent) {
//                    if (View) {
//                        mComponent = View.GetComponent<BaseSpaceObject>();
//                    }
//                }
//                return mComponent;
//            }
//        }
//        public int previousSubZone { get; private set; }

//        public void SetPosition(Vector3 p) {
//            Position = new float[] { p.x, p.y, p.z };
//        }

//        public NetworkGame Game {
//            get {
//                return this.game;
//            }
//        }

//        public Race Race {
//            get {

//                if (raceable != null) {

//                    return raceable.race;
//                }
//                if (IsMine) {
//                    Debug.Log("you don't have raceable component!!".Color("blue"));
//                }

//                return Race.None;
//            }
//        }

//        public string Id {
//            get {
//                return this.id;
//            }
//        }

//        public byte Type {
//            get {
//                return this.type;
//            }
//        }


//        public Hashtable RawProperties {
//            get {
//                Hashtable result = new Hashtable();
//                foreach (var pair in properties) {
//                    result.Add(pair.Key, pair.Value);
//                }
//                return result;
//            }
//        }

//        public Dictionary<byte, object> props {
//            get {
//                return properties;
//            }
//        }

//        public bool ExistsView {
//            get {
//                return this._view;
//            }
//        }

//        public GameObject View {
//            get {
//                return _view;
//            }
//        }

//        public bool Subscribed {
//            get {
//                return _subscribed;
//            }
//        }

//        public string Name {
//            get {
//                return StringCache.nameResolver.Resolve(this);
//            }
//        }

//        public Sprite uiIcon {
//            get {
//                return SpriteCache.uiIconResolver.Resolve(this);
//            }
//        }

//        public Sprite descriptionIcon {
//            get {
//                return SpriteCache.descriptionIconResolver.Resolve(this);
//            }
//        }

//        public bool ShipDestroyed {
//            get {
//                return _shipDestroyed;
//            }
//        }

//        public float size { get; private set; }

//        public void SetSubscribed(bool subscribed) {
//            //"item of [{0}] subscribed [{1}]".f(this.type.toItemType(), subscribed).Bold().Color( subscribed ? Color.green : Color.yellow).Print();
//            _subscribed = subscribed;
//        }


//        public void SetProperty(byte propName, object value) {
//            object oldValue = null;
//            if (properties.ContainsKey(propName)) {
//                oldValue = properties[propName];
//            }
//            properties[propName] = value;
//            if (!IsMine) {
//                if (propName == (byte)PS.InterestAreaAttached) {
//                    SetInterestAreaAttached((bool)value);
//                    MakeVisibleToSubscribedInterestAreas();
//                }
//            }
//            OnPropertySetted(propName, oldValue, value);

//            if (propName == (byte)PS.Invisibility) {
//                invisible = (bool)value;
//            }
//        }

//        public void SetProperties(Hashtable inProperties) {
//            foreach (DictionaryEntry entry in inProperties) {
//                SetProperty((byte)entry.Key, entry.Value);
//            }
//        }

//        public virtual void OnPropertySetted(byte key, object oldValue, object newValue) {

//        }

//        public bool ContainsProperty(byte name) {
//            return properties.ContainsKey(name);
//        }

//        public T GetProperty<T>(byte name) {

//            if (properties.ContainsKey(name)) {
//                return (T)properties[name];
//            }
//            return default(T);
//        }

//        public bool TryGetProperty<T>(byte name, out T value) {
//            if (properties.ContainsKey(name)) {
//                value = (T)properties[name];
//                return true;
//            }
//            value = default(T);
//            return false;
//        }


//        public void ResetSubZone() {
//            previousSubZone = 0;
//        }


//        /*
//        public float[] ViewDistanceEnter { get; private set; }
//        public float[] ViewDistanceExit { get; private set; }
//        */

//        public void SetSubZone(int subZone) {
//            if (subZone != subZoneID) {
//                previousSubZone = subZoneID;
//            }

//            this.subZoneID = subZone;
//        }

//        protected Item(string id, byte type, NetworkGame game, string name, object[] inComponents, float size, int subZoneID) {
//            if (Position == null) {
//                Position = new float[] { 0, 0, 0 };
//            }
//            if (Rotation == null) {
//                Rotation = new float[] { 0, 0, 0 };
//            }
//            this.id = id;
//            this.game = game;
//            this.type = type;
//            this.visibleInterestAreas = new List<byte>();
//            this.subscribedInterestAreas = new List<byte>();
//            properties = new Dictionary<byte, object>();
//            this.size = size;
//            this.subZoneID = subZoneID;

//            if (!IsMine) {
//                ReplaceComponents(inComponents);
//            } else {
//                ReplaceMyComponentsFromGameData(inComponents);
//            }
//        }

//        private void ReplaceMyComponentsFromGameData(object[] inComponents) {
//            if (GameData.instance != null) {
//                if (GameData.instance.cachedComponents != null && GameData.instance.cachedComponents.Length > 0) {
//                    if (inComponents == null || inComponents.Length == 0) {
//                        ReplaceComponents(GameData.instance.cachedComponents);
//                    } else {
//                        ReplaceComponents(inComponents);
//                    }
//                } else {
//                    ReplaceComponents(inComponents);
//                }
//            } else {
//                ReplaceComponents(inComponents);
//            }
//        }

//        private void CacheMyComponentsInGameData(object[] inComponents) {
//            if (IsMine) {
//                if (inComponents != null && inComponents.Length > 0) {
//                    if (GameData.instance != null) {
//                        GameData.instance.CacheComponents(inComponents);
//                    }
//                }
//            }
//        }

//        public void ReplaceComponents(object[] inComponents) {
//            CacheMyComponentsInGameData(inComponents);

//            if (inComponents != null) {
//                componentIDS = new ComponentID[inComponents.Length];
//                for (int i = 0; i < inComponents.Length; i++) {
//                    componentIDS[i] = (ComponentID)(int)inComponents[i];
//                }
//            } else {
//                componentIDS = new ComponentID[] { };
//            }

//            components = new Dictionary<ComponentID, MmoBaseComponent>();
//            foreach (var cID in componentIDS) {
//                var component = MmoBaseComponent.CreateNew(cID);
//                if (component != null) {
//                    components.Add(cID, component);
//                    component.SetItem(this);
//                    switch (cID) {
//                        case ComponentID.Bonuses: bonuses = component as MmoBonusesComponent; break;
//                        case ComponentID.Bot: bot = component as MmoBotComponent; break;
//                        case ComponentID.Character: character = component as MmoCharacterComponent; break;
//                        case ComponentID.Damagable: damagable = component as MmoDamagableComponent; break;
//                        case ComponentID.Energy: energy = component as MmoEnergyComponent; break;
//                        case ComponentID.Model: model = component as MmoModelComponent; break;
//                        case ComponentID.PlayerAI: playerAI = component as MmoPlayerAIComponent; break;
//                        case ComponentID.Raceable: raceable = component as MmoRaceableComponent; break;
//                        case ComponentID.Ship: ship = component as MmoShipComponent; break;
//                        case ComponentID.Target: target = component as MmoTargetComponent; break;
//                        case ComponentID.Movable: movable = component as MmoMovableComponent; break;
//                        case ComponentID.Weapon: weapon = component as MmoWeaponComponent; break;
//                    }
//                }
//            }
//        }

//        public MmoBaseComponent GetMmoComponent(ComponentID cID) {
//            if (components.ContainsKey(cID)) {
//                return components[cID];
//            }
//            return null;
//        }

//        public T GetMmoComponent<T>() where T : MmoBaseComponent {
//            foreach (var cpair in components) {
//                if (cpair.Value is T) {
//                    return (cpair.Value as T);
//                }
//            }
//            return default(T);
//        }



//        public bool AddSubscribedInterestArea(byte cameraId) {
//            if (this.subscribedInterestAreas.Contains(cameraId)) {
//                return false;
//            }
//            this.subscribedInterestAreas.Add(cameraId);
//            return true;
//        }

//        public bool AddVisibleInterestArea(byte cameraId) {
//            if (this.visibleInterestAreas.Contains(cameraId)) {
//                return false;
//            }
//            this.visibleInterestAreas.Add(cameraId);
//            return true;
//        }

//        public void GetInitialProperties() {
//            Operations.GetProperties(this.game, this.id, this.type, null);
//        }

//        public void GetProperties() {
//            Operations.GetProperties(this.game, this.id, this.type, this.PropertyRevision);
//        }

//        public void MakeVisibleToSubscribedInterestAreas() {
//            this.subscribedInterestAreas.ForEach(b => this.AddVisibleInterestArea(b));
//        }
//        public bool RemoveSubscribedInterestArea(byte cameraId) {
//            return this.subscribedInterestAreas.Remove(cameraId);
//        }
//        public bool RemoveVisibleInterestArea(byte cameraId) {
//            return this.visibleInterestAreas.Remove(cameraId);
//        }
//        public void ResetPreviousPosition() {
//            this.PreviousPosition = null;
//        }
//        public virtual void SetInterestAreaAttached(bool attached) {
//            this.InterestAreaAttached = attached;
//        }

//        /*
//        public virtual void SetInterestAreaViewDistance(float[] viewDistanceEnter, float[] viewDistanceExit)
//        {
//            this.ViewDistanceEnter = viewDistanceEnter;
//            this.ViewDistanceExit = viewDistanceExit;
//        }

//        public virtual void SetViewDistanceEnter(float[] viewDistanceEnter) { 
//            this.ViewDistanceEnter = viewDistanceEnter; 
//        }

//        public virtual void SetViewDistanceExit(float[] viewDistanceExit) { 
//            this.ViewDistanceExit = viewDistanceExit; 
//        }*/


//        public void SetPositions(float[] position, float[] previousPosition, float[] rotation, float[] previousRotation, float speed) {
//            this.Position = position;
//            this.PreviousPosition = previousPosition;
//            this.Rotation = rotation;
//            this.PreviousRotation = previousRotation;
//            this.movedSpeed = speed;
//            this.OnMoved();
//        }

//        private void OnMoved() {
//            if (IsMine == false) {
//                if (_transformInterpolation) {
//                    _transformInterpolation.ReceivedData(new ExtrapolationData { Position = this.Position.toVector(), Rotation = this.Rotation.toVector(), Time = Time.time, Speed = this.movedSpeed });
//                }
//            }
//            if (this.Moved != null) {
//                this.Moved(this);
//            }
//        }

//        public virtual void Create(GameObject modelObject) {
//            if (false == this.ExistsView) {
//                GameObject obj = new GameObject(modelObject.name);
//                obj.transform.position = modelObject.transform.position;
//                obj.transform.rotation = modelObject.transform.rotation;
//                modelObject.transform.SetParent(obj.transform);
//                modelObject.transform.localPosition = Vector3.zero;
//                modelObject.transform.localRotation = Quaternion.identity;

//                obj.transform.position = (Position != null) ? Position.toVector() : Vector3.zero;
//                obj.transform.rotation = (Rotation != null ? Quaternion.Euler(Rotation.toVector()) : Quaternion.identity);
//                _view = obj;
//                _view.name = "A_player" + (this.IsMine ? "MY" : this.Id.Substring(0, 3)) + "(" + this.Type.toItemType().ToString() + ")";
//                _transformInterpolation = _view.AddComponent<NetworkTransformInterpolation>();
//                _view.AddComponent<MmoComponentUpdater>().SetItem(this);
//                mShipModel = _view.GetComponentInChildren<ShipModel>();
//            }
//        }

//        /// <summary>
//        /// Change model of view to new model
//        /// </summary>
//        /// <param name="model"></param>
//        public void ChangeModel(string model) {
//            if (View) {
//                string prefabPath;
//                if (DataResources.Instance.prefabsDb.TryGetPath(model, out prefabPath)) {
//                    //destroy old view model
//                    foreach (Transform childTransoform in View.transform) {
//                        GameObject.Destroy(childTransoform.gameObject);
//                    }
//                    //create and setup new model
//                    GameObject newModelObject = GameObject.Instantiate<GameObject>(PrefabCache.Get(prefabPath));
//                    newModelObject.transform.SetParent(View.transform);
//                    newModelObject.transform.localPosition = Vector3.zero;
//                    newModelObject.transform.localRotation = Quaternion.identity;
//                }
//            }
//        }

//        public void ChangeUnderConstruct(bool underConstruct) {
//            if (View) {
//                var construct = View.GetComponentInChildren<Construct>();
//                if (construct) {
//                    if (underConstruct) {
//                        construct.MakeConstruct();
//                    } else {
//                        construct.MakeUnconstruct();
//                    }
//                }
//            }
//        }

//        public virtual void DestroyView() {
//            if (true == this.ExistsView) {
//                if ((!ShipDestroyed) && (!IsMine)) {
//                    SetShipDestroyed(true);
//                }

//                if (IsMine) {
//                    Debug.Log("Destroy mine view");
//                }
//                GameObject.Destroy(_view);
//                _view = null;
//                // Debug.Log(string.Format("Item.DestroyView(): success {0}:{1}", (ItemType)Type, Id).Color("green"));
//            } else {
//                Debug.Log("Item.DestroyView(): fail not exist view".Color("green"));
//            }
//        }

//        public virtual void SkillUsed(SkillUseEventInfo skillProperties, Hashtable info) {
//            if (View) {
//                View.GetComponent<BaseSpaceObject>().UseSkill(skillProperties, info);
//            }
//        }

//        public Vector3 GetPosition() {
//            if (Position == null) {
//                return Vector3.zero;
//            }
//            return new Vector3(Position[0], Position[1], Position[2]);
//        }



//        public virtual void SetShipDestroyed(bool shipDetroyed) {
//            bool oldDestroyed = this._shipDestroyed;
//            _shipDestroyed = shipDetroyed;

//            if ((false == oldDestroyed) && (true == this._shipDestroyed)) {
//                if (this.Component) {
//                    this.Component.OnShipWasDestroyed();
//                }
//            }
//        }

//        public abstract void AdditionalUpdate();

//        public ShipModel GetShipModel() {
//            return mShipModel;
//        }

//        /// <summary>
//        /// Called in screen icon update to check show or hide icon temporary
//        /// </summary>
//        /// <returns></returns>
//        public virtual bool CheckIconVisibility() {
//            return (false == invisible);
//        }

//        public T GetViewComponent<T>() where T : BaseSpaceObject {
//            if (!Component) {
//                return default(T);
//            }
//            if (Component is T) {
//                return (Component as T);
//            }
//            return default(T);
//        }

//        public bool IsEqual(string itemID, byte itemType) {
//            return ((Id == itemID) && (Type == itemType));
//        }

//        public bool isOutpost {
//            get {
//                if (bot != null) {
//                    if (bot.botSubType.HasValue) {
//                        return (bot.botSubType.Value == BotItemSubType.MainOutpost);
//                    }
//                }
//                return false;
//            }
//        }




//        public bool isFortification {
//            get {
//                if (bot != null) {
//                    if (bot.botSubType.HasValue) {
//                        return (bot.botSubType.Value == BotItemSubType.Outpost);
//                    }
//                }
//                return false;
//            }
//        }
//    }
//}

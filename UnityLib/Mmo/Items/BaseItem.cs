namespace Nebula.Mmo.Items {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Common;
    using Components;

    public abstract class BaseItem : IMmoComponentContainer {

        public string id { get; private set; }
        public byte type { get; private set; }
        public Dictionary<byte, object> properties { get; private set; }
        public GameObject view { get; private set; }

        public float[] Position { get; private set; }
        public float[] Rotation { get; private set; }
        public float[] PreviousPosition { get; private set; }
        public float[] PreviousRotation { get; private set; }
        public int PropertyRevision { get; set; }

        public ComponentID[] componentIDS { get; private set; }
        public MmoBonusesComponent bonuses { get; private set; }
        public MmoBotComponent bot { get; private set; }
        public MmoCharacterComponent character { get; private set; }
        public MmoDamagableComponent damagable { get; private set; }
        public MmoEnergyComponent energy { get; private set; }
        public MmoModelComponent model { get; private set; }
        public MmoPlayerAIComponent playerAI { get; private set; }
        public MmoRaceableComponent raceable { get; private set; }
        public MmoShipComponent ship { get; private set; }
        public MmoTargetComponent target { get; private set; }
        public MmoMovableComponent movable { get; private set; }
        public MmoWeaponComponent weapon { get; private set; }


        public bool IsDestroyed { get; private set; }
        public abstract bool IsMine { get; }
        public Dictionary<ComponentID, MmoBaseComponent> components { get; private set; }
        public bool invisible { get; private set; }
        public int previousSubZone { get; private set; }
        public float size { get; private set; }
        public int subZoneID { get; private set; }

        public bool InterestAreaAttached { get; private set; }

        protected NetworkTransformInterpolation transformInterpolation { get; private set; }
        private bool subscribed { get; set; }
        private bool mShipDestroyed;
        private float mMoveSpeed;

        public void SetPosition(Vector3 p) {
            Position = new float[] { p.x, p.y, p.z };
        }

        protected BaseItem(string inID, byte inType, object[] inComponents, float inSize, int inSubZoneID) {
            if (Position == null) {
                SetPosition(Vector3.zero);
            }
            if (Rotation == null) {
                Rotation = new float[] { 0, 0, 0 };
            }
            id = inID;
            type = inType;
            properties = new Dictionary<byte, object>();
            size = inSize;
            subZoneID = inSubZoneID;
            if (!IsMine) {
                ReplaceComponents(inComponents);
            } else {
                ReplaceMyComponentsFromGameData(inComponents);
            }
        }



        public void ReplaceComponents(object[] inComponents) {
            CacheMyComponentsInGameData(inComponents);

            if (inComponents != null) {
                componentIDS = new ComponentID[inComponents.Length];
                for (int i = 0; i < inComponents.Length; i++) {
                    componentIDS[i] = (ComponentID)(int)inComponents[i];
                }
            } else {
                componentIDS = new ComponentID[] { };
            }

            components = new Dictionary<ComponentID, MmoBaseComponent>();
            foreach (var cID in componentIDS) {
                var component = MmoBaseComponent.CreateNew(cID);
                if (component != null) {
                    components.Add(cID, component);
                    component.SetItem(this);
                    switch (cID) {
                        case ComponentID.Bonuses: bonuses = component as MmoBonusesComponent; break;
                        case ComponentID.Bot: bot = component as MmoBotComponent; break;
                        case ComponentID.Character: character = component as MmoCharacterComponent; break;
                        case ComponentID.Damagable: damagable = component as MmoDamagableComponent; break;
                        case ComponentID.Energy: energy = component as MmoEnergyComponent; break;
                        case ComponentID.Model: model = component as MmoModelComponent; break;
                        case ComponentID.PlayerAI: playerAI = component as MmoPlayerAIComponent; break;
                        case ComponentID.Raceable: raceable = component as MmoRaceableComponent; break;
                        case ComponentID.Ship: ship = component as MmoShipComponent; break;
                        case ComponentID.Target: target = component as MmoTargetComponent; break;
                        case ComponentID.Movable: movable = component as MmoMovableComponent; break;
                        case ComponentID.Weapon: weapon = component as MmoWeaponComponent; break;
                    }
                }
            }
        }

        public void SetSubscribed(bool insubscribed) {
            subscribed = insubscribed;
        }

        public void SetProperty(byte propName, object value) {
            object oldValue = null;
            if (properties.ContainsKey(propName)) {
                oldValue = properties[propName];
            }
            properties[propName] = value;
            OnPropertySetted(propName, oldValue, value);

            if (propName == (byte)PS.Invisibility) {
                invisible = (bool)value;
            }
        }

        public virtual void SetPositions(float[] position, float[] previousPosition, float[] rotation, float[] previousRotation, float speed) {
            Position = position;
            PreviousPosition = previousPosition;
            Rotation = rotation;
            PreviousRotation = previousRotation;
            mMoveSpeed = speed;
        }

        public void SetSubZone(int subZone) {
            if (subZone != subZoneID) {
                previousSubZone = subZoneID;
            }

            this.subZoneID = subZone;
        }

        public void ResetSubZone() {
            previousSubZone = 0;
        }
        public void SetProperties(Hashtable inProperties) {
            foreach (DictionaryEntry entry in inProperties) {
                SetProperty((byte)entry.Key, entry.Value);
            }
        }
        public bool ContainsProperty(byte name) {
            return properties.ContainsKey(name);
        }

        public T GetProperty<T>(byte name) {

            if (properties.ContainsKey(name)) {
                return (T)properties[name];
            }
            return default(T);
        }

        public bool TryGetProperty<T>(byte name, out T value) {
            if (properties.ContainsKey(name)) {
                value = (T)properties[name];
                return true;
            }
            value = default(T);
            return false;
        }

        public MmoBaseComponent GetMmoComponent(ComponentID cID) {
            if (components.ContainsKey(cID)) {
                return components[cID];
            }
            return null;
        }
        public T GetMmoComponent<T>() where T : MmoBaseComponent {
            foreach (var cpair in components) {
                if (cpair.Value is T) {
                    return (cpair.Value as T);
                }
            }
            return default(T);
        }
        public virtual void DestroyView() {
            if (true == this.ExistsView) {
                if ((!ShipDestroyed) && (!IsMine)) {
                    SetShipDestroyed(true);
                }

                if (IsMine) {
                    Debug.Log("Destroy mine view");
                }
                GameObject.Destroy(view);
                SetView(null);
                // Debug.Log(string.Format("Item.DestroyView(): success {0}:{1}", (ItemType)Type, Id).Color("green"));
            } else {
                Debug.Log("Item.DestroyView(): fail not exist view".Color("green"));
            }
        }
        public virtual bool CheckIconVisibility() {
            return (false == invisible);
        }
        public Vector3 GetPosition() {
            if (Position == null) {
                return Vector3.zero;
            }
            return new Vector3(Position[0], Position[1], Position[2]);
        }
        public bool IsEqual(string itemID, byte itemType) {
            return ((Id == itemID) && (Type == itemType));
        }

        public virtual void SetInterestAreaAttached(bool attached) {
            InterestAreaAttached = attached;
        }
        protected abstract void CacheMyComponentsInGameData(object[] inComponents);
        protected abstract void ReplaceMyComponentsFromGameData(object[] inComponents);
        public virtual void OnPropertySetted(byte key, object oldValue, object newValue) { }
        public abstract void Create(GameObject modelObject);
        public abstract void AdditionalUpdate();
        public abstract void UpdateMmoComponent(MmoBaseComponent component);

        public virtual void SetShipDestroyed(bool inshipDetroyed) {
            mShipDestroyed = inshipDetroyed;
        }

        public void SetDestroyed(bool inDestroyed) {
            IsDestroyed = inDestroyed;
        }

        protected void SetView(GameObject inView) {
            view = inView;
        }

        protected void SetTransformInterpolation(NetworkTransformInterpolation inTransformInterpolation) {
            transformInterpolation = inTransformInterpolation;
        }

        public GameObject View {
            get {
                return view;
            }
        }

        public bool ExistsView {
            get {
                return view;
            }
        }

        public bool Subscribed {
            get {
                return subscribed;
            }
        }

        public bool ShipDestroyed {
            get {
                return mShipDestroyed;
            }
        }

        public float movedSpeed {
            get {
                return mMoveSpeed;
            }
        }

        public string Id {
            get {
                return this.id;
            }
        }

        public byte Type {
            get {
                return this.type;
            }
        }
        public Race Race {
            get {
                if (raceable != null) {
                    return raceable.race;
                }
                if (IsMine) {
                    Debug.Log("you don't have raceable component!!".Color("blue"));
                }
                return Race.None;
            }
        }
        public bool isOutpost {
            get {
                if (bot != null) {
                    if (bot.botSubType.HasValue) {
                        return (bot.botSubType.Value == BotItemSubType.MainOutpost);
                    }
                }
                return false;
            }
        }
        public bool isFortification {
            get {
                if (bot != null) {
                    if (bot.botSubType.HasValue) {
                        return (bot.botSubType.Value == BotItemSubType.Outpost);
                    }
                }
                return false;
            }
        }

        public bool isPlanet {
            get {
                if(bot != null ) {
                    if(bot.botSubType.HasValue) {
                        return (bot.botSubType.Value == BotItemSubType.Planet);
                    }
                }
                return false;
            }
        }

        public bool isMiningStation {
            get {
                if(bot != null ) {
                    if(bot.botSubType.HasValue) {
                        return (bot.botSubType.Value == BotItemSubType.Drill);
                    }
                }
                return false;
            }
        }

        public bool isBot {
            get {
                return (type == (byte)ItemType.Bot);
            }
        }
    }
}

// NebulaObject.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:58:25 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nebula.Engine {

    /// <summary>
    /// Base object in world run loop, contains components
    /// </summary>
    public class NebulaObject {

        private string mName = "";


        private BehaviourCollection behaviours;

        public string Id { get; private set; }

        public byte Type { get; private set; }

        public ItemType getItemType() {
            return (ItemType)Type;
        }

        public string badge { get; private set; } = string.Empty;

        public int subZone { get; private set; }

        public string name { get {
                return mName;
            } set {
                mName = value;
                if(mName == null ) {
                    mName = string.Empty;
                }
            } }

        public NebulaTransform transform { get; private set; }

        public NebulaObjectProperties properties { get; private set; }

        public IBaseWorld world { get; private set; }

        public Dictionary<byte, object> tags { get; private set; }

        public float size { get; private set; } = 1f;

        public bool invisible { get; private set; } = false;

        public bool databaseSaveable { get; private set; } = false;

        public void SetInvisibility(bool value) {
            if(invisible != value) {
                invisible = value;
                properties.SetProperty((byte)PS.Invisibility, invisible);
                SendMessage(ComponentMessages.OnInvisibilityChanged, invisible);
            }
        }

        public void SetDatabaseSaveable(bool value) {
            databaseSaveable = value;
        }

        public void SetBadge(string inBadge) {
            badge = inBadge;
            properties.SetProperty((byte)PS.Badge, (badge == null) ? string.Empty : badge);
        }

        public IRes resource {
            get {
                return world.Resource();
            }
        }

        public NebulaObject(IBaseWorld iWorld, Dictionary<byte, object> inTags, float size, int subZone, params Type[] components) 
            : this(iWorld, Guid.NewGuid().ToString(), inTags, size, subZone, components) {

        }

        public NebulaObject(IBaseWorld iWorld, string id, Dictionary<byte, object> inTags, float size, int subZone, params Type[] components) 
            : this(iWorld, id, (byte)ItemType.Avatar, inTags, size, subZone,  components) {

        }

        public NebulaObject(IBaseWorld iWorld, string id, byte type, 
            Dictionary<byte, object> inTags, float size, int subZone, params Type[] components) {
            Id = id;
            Type = type;
            world = iWorld;
            tags = inTags;
            this.size = size;
            this.subZone = subZone;
            if(tags == null ) {
                tags = new Dictionary<byte, object>();
            }

            behaviours = new BehaviourCollection();

            transform = AddComponent<NebulaTransform>();
            properties = AddComponent<NebulaObjectProperties>();
            properties.SetProperty((byte)PS.Invisibility, invisible);
            //properties.SetProperty((byte)PS.SubZoneID, subZone);
            properties.SetProperty((byte)PS.Badge, string.Empty);

            if (components != null && components.Length > 0) {
                foreach (var component in components) {
                    AddComponent(component);
                }
            }
        }

        public NebulaObject (IBaseWorld iWorld, string id, byte type, Dictionary<byte, object> inTags,
            float size, int subZone, BehaviourCollection behaviours) {
            Id = id;
            Type = type;
            world = iWorld;
            tags = inTags;
            this.size = size;
            this.subZone = subZone;
            if (tags == null) {
                tags = new Dictionary<byte, object>();
            }
            this.behaviours = behaviours;
            transform = GetComponent<NebulaTransform>();
            properties = GetComponent<NebulaObjectProperties>();
            properties.SetProperty((byte)PS.Invisibility, invisible);
            UpdateNebulaObject();
        }

        public void SetSubZone(int subZoneID) {
            subZone = subZoneID;
        }

      

        private void UpdateNebulaObject() {
            foreach (var b in behaviours) {
                MethodInfo method = typeof(NebulaBehaviour).GetMethod("SetNebulaObject", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(b.Value, new object[] { this });
            }
        }

        /// <summary>
        /// Add component to object
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>Created component instance</returns>
        public T AddComponent<T>() where T : NebulaBehaviour {
            if (!behaviours.ContainsBehaviour(typeof(T))) {
                T behaviour = Activator.CreateInstance<T>();
                MethodInfo method = typeof(NebulaBehaviour).GetMethod("SetNebulaObject", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(behaviour, new object[] { this });
                behaviours.AddBehaviour(typeof(T), behaviour);
                behaviour.Awake();
                return behaviour;
            } else {
                return GetComponent<T>();
            }
            
        }

        public T GetInterface<T>() where T : class {
            return behaviours.GetInterface<T>();
        }

        /// <summary>
        /// Not templated version of adding component to object
        /// </summary>
        /// <param name="type">Type of component to added</param>
        /// <returns>Newly created component</returns>
        public NebulaBehaviour AddComponent(Type type) {
            NebulaBehaviour behaviour = Activator.CreateInstance(type) as NebulaBehaviour;
            MethodInfo method = typeof(NebulaBehaviour).GetMethod("SetNebulaObject", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(behaviour, new object[] { this });
            behaviours.AddBehaviour(type, behaviour);
            behaviour.Awake();
            return behaviour;
        }

        public NebulaBehaviour AddComponentNoAwake(Type type, NebulaBehaviour behaviour) {
            MethodInfo method = typeof(NebulaBehaviour).GetMethod("SetNebulaObject", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(behaviour, new object[] { this });
            behaviours.AddBehaviour(type, behaviour);
            return behaviour;
        }

        /// <summary>
        /// Get component of parameter type from object
        /// </summary>
        /// <typeparam name="T">Type of requested component</typeparam>
        /// <returns>If found component such type return it else return null</returns>
        public T GetComponent<T>() where T : NebulaBehaviour {
            if (behaviours.ContainsBehaviour(typeof(T))) {
                return behaviours.GetBehaviour(typeof(T)) as T;
            }
            return default(T);
        }

        public NebulaBehaviour GetComponent(Type type) {
            if(behaviours.ContainsBehaviour(type)) {
                return behaviours.GetBehaviour(type) as NebulaBehaviour;
            }
            return null;
        }

        public NebulaBehaviour GetComponent(int componentID) {
            return behaviours.GetBehaviour(componentID);
        }

        /// <summary>
        /// Set component of parameter type to object. Start() on component don't called. Don't call this method in usual manner, call instead AddComponent()
        /// </summary>
        /// <typeparam name="T">Type of component to set</typeparam>
        /// <param name="component">Componet to setted to object </param>
        /// <returns>Return old component of such type on object or null if such component don't exists before</returns>
        public T SetComponent<T>(T component) where T : NebulaBehaviour {
            T oldComponent = default(T);
            //find old component
            if(behaviours.ContainsBehaviour(typeof(T))) {
                oldComponent = behaviours.GetBehaviour(typeof(T)) as T;
            }    
            //attacj new component to NebulaObject       
            MethodInfo method = typeof(NebulaBehaviour).GetMethod("SetNebulaObject", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            method.Invoke(component, new object[] { this });
            //replace to collection 
            behaviours.SetBehaviour(typeof(T), component);
            return oldComponent;
        }

        public object SetComponentAsNew(Type type, NebulaBehaviour component) {
            object oldComponent = null;
            if (behaviours.ContainsBehaviour(type)) {
                oldComponent = behaviours.GetBehaviour(type);
            }
            MethodInfo method = typeof(NebulaBehaviour).GetMethod("SetNebulaObject", BindingFlags.NonPublic | BindingFlags.Instance );
            method.Invoke(component, new object[] { this });
            //replace to collection 
            behaviours.SetBehaviour(type, component);
            if(type == typeof(NebulaTransform)) {
                transform = component as NebulaTransform;
            }
            if(type == typeof(NebulaObjectProperties)) {
                properties = component as NebulaObjectProperties;
            }
            return oldComponent;
        }
        /// <summary>
        /// Remove component from object
        /// </summary>
        /// <typeparam name="T">Type of removed component</typeparam>
        /// <returns>return removed component</returns>
        public T RemoveComponent<T>() where T : NebulaBehaviour {
            T behaviour = default(T);
            if(behaviours.ContainsBehaviour(typeof(T))) {
                behaviour = behaviours.GetBehaviour(typeof(T)) as T;
            }
            behaviours.RemoveBehaviour(typeof(T));
            return behaviour;
        }

        /// <summary>
        /// Find component with parameter name in collection of components
        /// </summary>
        /// <param name="name">Name of component</param>
        /// <returns>Founded component</returns>
        public NebulaBehaviour FindComponent(string name) {
            return behaviours.FindBehaviour(name);
        }

        /// <summary>
        /// Update function. Called from game loop in call update functions on all components
        /// </summary>
        /// <param name="deltaTime">Time interval from previous call</param>
        public void Update(float deltaTime) {
            behaviours.Update(deltaTime);
        }

        protected virtual bool Valid() {
            return true;
        }

        public void SendMessage(string message, object arg = null) {
            behaviours.SendMessage(message, arg);
        }

        public static implicit operator bool(NebulaObject obj) {
            if(obj == null) {
                return false;
            }
            return obj.Valid();
        }

        public bool HasTag(byte key) {
            return tags.ContainsKey(key);
        }

        public object Tag(byte key) {
            return tags[key];
        }

        public void SetTag(byte key, object value) {
            tags[key] = value;
        }

        public object[] componentIds {
            get {
                return behaviours.behaviourIds;
            }
        }

        public BehaviourCollection allBehaviours {
            get {
                return behaviours;
            }
        }
    }
}

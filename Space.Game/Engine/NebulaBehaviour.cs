using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using Space.Game;
using Common;

namespace Nebula.Engine {
    /// <summary>
    /// Base component class. Components creates only by attaching to nebula object
    /// </summary>
    public abstract class NebulaBehaviour {

        private int mStarted = 0;
        /// <summary>
        /// Game object to component attached
        /// </summary>
        public NebulaObject nebulaObject { get; private set; }

        public NebulaTransform transform {
            get {
                return nebulaObject.transform;
            }
        }

        public NebulaObjectProperties props {
            get {
                return nebulaObject.properties;
            }
        }

        public string badge {
            get {
                return nebulaObject.badge;
            }
        }

        public IRes resource {
            get {
                return nebulaObject.resource;
            }
        }
        
        /// <summary>
        /// name of component
        /// </summary>
        public string name {
            get {
                return nebulaObject.name;
            }
            set {
                if(value == null) {
                    nebulaObject.name = string.Empty;
                }
                nebulaObject.name = value;
            }
        }

        //attach to gameobject, called from Nebula Object by reflection when component attached
        private void SetNebulaObject(NebulaObject obj) {
            nebulaObject = obj;
        }

        public virtual void Awake() { }
        /// <summary>
        /// Virtual function overrides by subclasses, called when component attached to object
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Update function overrides by subclasses, called one per frame
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(float deltaTime) { }



        public void Tick(float deltaTime) {
            if(mStarted == 0) {
                CheckComponents();
                Start();
                Interlocked.Exchange(ref mStarted, 1);
                return;
            } else {
                Update(deltaTime);
            }
        }

        private void CheckComponents() {
            foreach(Attribute attr in Attribute.GetCustomAttributes(GetType(), true)) {
                if(attr is REQUIRE_COMPONENT) {
                    
                    REQUIRE_COMPONENT requireComponentAttribute = attr as REQUIRE_COMPONENT;
                    if (nebulaObject.GetComponent(requireComponentAttribute.Type) == null ) {
                        throw new BehaviourMissedException(((ItemType)nebulaObject.Type) + ":" + nebulaObject.Id + ":" + name, requireComponentAttribute.Type);
                    }
                }
            }
        }

        public T GetComponent<T>() where T : NebulaBehaviour {
            return nebulaObject.GetComponent<T>();
        }

        public T RequireComponent<T>() where T : NebulaBehaviour {
            var component = GetComponent<T>();
            if(component == default(T)) {
                throw new BehaviourMissedException(name, typeof(T));
            }
            return component;
        }

        public static implicit operator bool(NebulaBehaviour behaviour) {
            if(behaviour == null) {
                return false;
            }
            return ((bool)behaviour.nebulaObject);
        }

        public void SendMessage(string message, object arg = null) {
            foreach(var method in GetType().GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if(method.Name == message) {
                    if(arg != null ) {
                        method.Invoke(this, new object[] { arg });
                    } else {
                        method.Invoke(this, null);
                    }
                }
            }
        }

        public object RequireProperty(byte key) {
            if(props.Contains(key)) {
                return props.GetProperty(key);
            }
            throw new PropertyMissedException(key, name);
        }

        public abstract int behaviourId { get; }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {

    /// <summary>
    /// Component collection based on Dictionary. Used by NebulaObject. Thread safe
    /// </summary>
    public class BehaviourCollection : ConcurrentDictionary<Type, NebulaBehaviour> {

        //public readonly object syncRoot = new object();

        /// <summary>
        /// Add component
        /// </summary>
        /// <param name="type">Component type</param>
        /// <param name="behaviour">Component instance</param>
        public void AddBehaviour(Type type, NebulaBehaviour behaviour) {
            TryAdd(type, behaviour);
        }

        /// <summary>
        /// Contains or not component of type
        /// </summary>
        /// <param name="key">Type for which test</param>
        /// <returns>True or false</returns>
        public bool ContainsBehaviour(Type key) {
            bool result = ContainsKey(key);
            if (!result) {
                foreach (var kv in this) {
                    if (key.IsAssignableFrom(kv.Key)) {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get component of type
        /// </summary>
        /// <param name="type">Type of component</param>
        /// <returns>Founded component</returns>
        public NebulaBehaviour GetBehaviour(Type type) {
            if (ContainsKey(type)) {
                return this[type];
            } else {
                Type foundedType = null;
                foreach (var kv in this) {
                    if (type.IsAssignableFrom(kv.Key)) {
                        foundedType = kv.Key;
                        break;
                    }
                }
                if (foundedType != null)
                    return this[foundedType];
                else
                    return null;
            }
        }

        public NebulaBehaviour GetBehaviour(int componentID) {
            foreach (var pBeh in this) {
                if (pBeh.Value.behaviourId == componentID) {
                    return pBeh.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove component from collection
        /// </summary>
        /// <param name="type"></param>
        public void RemoveBehaviour(Type type) {
            if (ContainsKey(type)) {
                NebulaBehaviour oldB;
                TryRemove(type, out oldB);
            } else {
                Type foundedType = null;
                foreach (var pair in this) {
                    if (type.IsAssignableFrom(pair.Key)) {
                        foundedType = pair.Key;
                        break;
                    }
                }
                if (foundedType != null) {
                    NebulaBehaviour oldB;
                    TryRemove(foundedType, out oldB);
                }
            }
        }

        public T GetInterface<T>() where T : class {
            foreach (var pair in this) {
                if (pair.Value is T) {
                    return (pair.Value as T);
                }
            }
            return default(T);
        }

        /// <summary>
        /// Set component
        /// </summary>
        /// <param name="type">type of component</param>
        /// <param name="behaviour">Component instance</param>
        public void SetBehaviour(Type type, NebulaBehaviour behaviour) {
            this[type] = behaviour;
        }



        /// <summary>
        /// Find component with parameter name
        /// </summary>
        /// <param name="name">Name of component to find</param>
        /// <returns>Founded component or null</returns>
        public NebulaBehaviour FindBehaviour(string name) {
            foreach (var behaviourPair in this) {
                if (behaviourPair.Value.name == name) {
                    return behaviourPair.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Update component collection/ Don't call this, used NebulaObject for updating in game loop
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime) {
            foreach (var behaviourPair in this) {
                behaviourPair.Value.Tick(deltaTime);
            }
        }

        public void SendMessage(string message, object arg = null) {
            foreach (var behaviourPair in this) {
                behaviourPair.Value.SendMessage(message, arg);
            }
        }

        public object[] behaviourIds {
            get {
                object[] ids = new object[Count];
                int index = 0;

                foreach (var behaviourPair in this) {
                    if (index < ids.Length) {
                        ids[index++] = behaviourPair.Value.behaviourId;
                    }
                }
                return ids;
            }
        }
    }
}

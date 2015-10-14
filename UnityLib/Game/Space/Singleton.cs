using UnityEngine;

namespace Game.Space {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        public static T Get {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<T>();
                return _instance;
            }
        }
    }
}
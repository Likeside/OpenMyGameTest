using UnityEngine;

namespace Utils {
    public class LocalSingleton<T> : MonoBehaviour where T: Component {
        public static T Instance => _instance;
		
        static T _instance;
		
        // Do NOT use Awake method in the derived classes or it will not be called here
        // Use OnSingletonAwake() method instead
        void Awake() {
            if (_instance == null) {
                _instance = this as T;
                OnSingletonAwake();
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// A substitute for the Awake() method. Will be called after all of the singleton stuff is done
        /// </summary>
        protected virtual void OnSingletonAwake() { }
    }
}
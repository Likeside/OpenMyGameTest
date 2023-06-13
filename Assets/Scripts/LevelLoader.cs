using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts {
    public class LevelLoader: MonoBehaviour {
        
        int[,] _fieldArray;
        public event Action<int[,]> OnLevelLoadedEvent;
        static int s_level;
        
        void Start() {
            _fieldArray = JsonConvert.DeserializeObject<int[,]>(LoadTextAsset().text);
            OnLevelLoadedEvent?.Invoke(_fieldArray);
        }
        
        public void LoadNextLevel() {
            s_level++;
            SceneManager.LoadScene(0);
        }
        
        TextAsset LoadTextAsset() {
            string path = $"Levels/{s_level}";
            var jsonText = Resources.Load<TextAsset>(path);
            if (jsonText == null) {
                s_level = 1; //предполагается, что хотя бы 1 уровень в игре есть
                path = $"Levels/{s_level}";
                jsonText = Resources.Load<TextAsset>(path);
            }
            return jsonText;
        }
        
        
    }
}
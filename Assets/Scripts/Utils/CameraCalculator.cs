using System;
using UnityEngine;

namespace Utils
{
    public class CameraCalculator: LocalSingleton<CameraCalculator> {
        //Screen size
        public float ScreenWidthHalved { get; private set; }
        public float ScreenHeightHalved { get; private set; }
        
        public float ScreenWidth { get; private set; }
        public float ScreenHeight { get; private set; }
        
        //SafeAreaSize
        public float SafeAreaWidthHalved { get; private set; }
        public float SafeAreaHeightHalved { get; private set; }
        public float SafeAreaWidth { get; private set; }
        public float SafeAreaHeight { get; private set; }
        
        public event Action OnCalculated;


        protected override void OnSingletonAwake() {
            Initialize();
        }
        

      public void Initialize() {
            var mainCamera = Camera.main;
            ScreenHeightHalved = mainCamera.orthographicSize;
            ScreenWidthHalved = mainCamera.aspect * mainCamera.orthographicSize;

            ScreenHeight = ScreenHeightHalved * 2;
            ScreenWidth = ScreenWidthHalved * 2;
            
            SafeAreaHeightHalved = ConvertPixelsToWorldUnits(Screen.safeArea.height / 2);
            SafeAreaWidthHalved = ConvertPixelsToWorldUnits(Screen.safeArea.width / 2);
            
            SafeAreaHeight = SafeAreaHeightHalved * 2;
            SafeAreaWidth = SafeAreaWidthHalved * 2;
            OnCalculated?.Invoke();
        }


        float ConvertPixelsToWorldUnits(float pixels) { 
            float camOrthoSize = Camera.main.orthographicSize;
            float pixelHeight = Camera.main.scaledPixelHeight;
            
            return (pixels * camOrthoSize * 2) / pixelHeight;
        }
        
        
    }
}
using UnityEngine;

namespace Scripts {
    public class FieldPresenter: MonoBehaviour {
        [SerializeField] FieldView _view;
        [SerializeField] LevelLoader _loader;
        
        FieldModel _model;
        
        void Awake() {
            _loader.OnLevelLoadedEvent += Initialize;
        }
        void Initialize(int[,] fieldArray) {
            _model = new FieldModel();
            _model.Initialize(fieldArray);
            _view.Initialize(fieldArray);
            _model.OnFirstSwipeEvent += _view.SwapFirst;
            _model.OnFieldNormalizedEvent += _view.Normalize;
            _view.OnTryingToSwapFirstEvent += _model.FirstSwipe;
            _view.OnAllClearedEvent += _loader.LoadNextLevel;
        }
        
        void OnDestroy() {
            _loader.OnLevelLoadedEvent -= Initialize;
            _model.OnFirstSwipeEvent -= _view.SwapFirst;
            _model.OnFieldNormalizedEvent -= _view.Normalize;
            _view.OnTryingToSwapFirstEvent -= _model.FirstSwipe;
            _view.OnAllClearedEvent -= _loader.LoadNextLevel; 
        }
    }
}
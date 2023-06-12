using UnityEngine;
using UnityEngine.UI;

namespace Scripts {
    public class FieldPresenter: MonoBehaviour {
        [SerializeField] FieldView _view;

        //test
        [SerializeField] Vector2Int _start;
        [SerializeField] Vector2Int _target;
        [SerializeField] Button _testBtn;

        FieldModel _model;
        void Start() {
            _model = new FieldModel();
            _model.Initialize();
            
            _model.OnFirstSwipeEvent += _view.SwapFirst;
            _model.OnFieldNormalizedEvent += _view.Normalize;
            _view.OnTryingToSwapFirstEvent += _model.FirstSwipe;
            
            _testBtn.onClick.AddListener( (() => _view.TryToSwapFirst(_start, _target)));
        }
    }
}
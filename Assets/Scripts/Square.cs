using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts {
    public class Square: MonoBehaviour, IBeginDragHandler, IDragHandler {

        [SerializeField] SpriteRenderer _sr;
        [SerializeField] Animator _animator;
        [SerializeField] Collider2D _collider;

        public event Action<Vector2Int, Vector2Int> OnTryingToSwapEvent;
        public Vector2Int Coords { get; set; }

        AnimationConfigSO _animationConfigSo;
        readonly Vector2Int _deletedSquareCoords = new(Int32.MaxValue, -Int32.MaxValue);
        float _speed = 0.5f; //todo: задавать при спавне
        int _maxSquares;
        int _representation;
        
        public void Set(Vector2Int coords, int representation, int maxSquares, AnimationConfigSO animationConfigSo) {
            _representation = representation;
            _animationConfigSo = animationConfigSo;
            Coords = coords;
            _animator.SetTrigger(_animationConfigSo.animationConfigs.FirstOrDefault(_ => _.representation == _representation).idleTrigger);
            _maxSquares = maxSquares;
            SetSortingOrder();
        }
        
        public void Move(int stepsVertical, int stepsHorizontal) {
            //т.к. позиция квадрата не равна координате, учитываем скейл, перемещаем относительно текущей позиции
            var targetPos = transform.localPosition + new Vector3(stepsHorizontal * transform.localScale.x,
                -stepsVertical * transform.localScale.y);
            float speedModifier = Math.Abs(stepsVertical) > Math.Abs(stepsHorizontal) ? stepsVertical : stepsHorizontal;
            transform.DOLocalMove(targetPos, _speed * Math.Abs(speedModifier)).SetEase(Ease.InCubic).onComplete = SetSortingOrder;
            Coords += new Vector2Int(stepsVertical, stepsHorizontal);
        }
        
        public void Delete() {
            Coords = _deletedSquareCoords;
            _animator.SetTrigger(_animationConfigSo.animationConfigs.FirstOrDefault(_ => _.representation == _representation).destroyTrigger);
        }

        public void SetInteraction(bool active) {
            _collider.enabled = active;
        }

        void SetSortingOrder() {
            _sr.sortingOrder = _maxSquares - Coords.x * 2 + Coords.y;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            Debug.Log(eventData.delta);
            Vector2Int target = Coords;
            if (MathF.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y)) {
                if (eventData.delta.x > 0) {
                    target += Vector2Int.up;
                }
                else {
                    target += Vector2Int.down;
                }
            }
            else {
                if (eventData.delta.y > 0) {
                    target += Vector2Int.right;
                }
                else {
                    target += Vector2Int.left;
                }
            }
            OnTryingToSwapEvent?.Invoke(Coords, target);
        }

        public void OnDrag(PointerEventData eventData) {
            
        }
    }
}
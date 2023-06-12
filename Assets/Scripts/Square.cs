using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Scripts {
    public class Square: MonoBehaviour {

        [SerializeField] Vector2Int _coords;

        public Vector2Int Coords {
            get => _coords;
            set => _coords = value;
        }  // test { get; set; }


        readonly Vector2Int _deletedSquareCoords = new Vector2Int(Int32.MaxValue, -Int32.MaxValue);
        float _speed = 0.5f; //todo: задавать при спавне
        
        public void Move(int stepsVertical, int stepsHorizontal) {
            var targetPos = transform.localPosition + new Vector3(stepsHorizontal * transform.localScale.x,
                -stepsVertical * transform.localScale.y);
            float speedModifier = Math.Abs(stepsVertical) > Math.Abs(stepsHorizontal) ? stepsVertical : stepsHorizontal;
            transform.DOLocalMove(targetPos, _speed * Math.Abs(speedModifier)).onComplete = (() => StartCoroutine(SetCoords(new Vector2Int(stepsVertical, stepsHorizontal))));

          //  Coords += new Vector2Int(stepsVertical, stepsHorizontal);//тут возможно икс и игреком перепутаны, проверить
        }

        IEnumerator SetCoords(Vector2Int modifier) {
            Coords += modifier;
            yield return new WaitForFixedUpdate();
        }

        public void Delete() {
            transform.DOScale(Vector3.zero, _speed);//TODO: добавить анимацию 
            Coords = _deletedSquareCoords;
        }

        public void SetInteraction(bool active) {
            
        }
    }
}
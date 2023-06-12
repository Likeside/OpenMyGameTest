using System;
using DG.Tweening;
using UnityEngine;

namespace Scripts {
    public class Square: MonoBehaviour {
        public Vector2Int Coords { get; set; }


        readonly Vector2Int _deletedSquareCoords = new Vector2Int(Int32.MaxValue, -Int32.MaxValue);
        float _speed = 1f; //todo: задавать при спавне
        
        public void Move(int stepsVertical, int stepsHorizontal) {
            var targetPos = transform.localPosition + new Vector3(stepsHorizontal * transform.localScale.x,
                stepsVertical * transform.localScale.y);
            float speedModifier = stepsVertical < stepsHorizontal ? stepsVertical : stepsHorizontal;
            transform.DOLocalMove(targetPos, _speed * speedModifier);

            Coords += new Vector2Int(stepsVertical, stepsHorizontal);//тут возможно икс и игреком перепутаны, проверить
        }

        public void Delete() {
            transform.DOScale(Vector3.zero, _speed);//TODO: добавить анимацию 
            Coords = _deletedSquareCoords;
        }

        public void SetInteraction(bool active) {
            
        }
    }
}
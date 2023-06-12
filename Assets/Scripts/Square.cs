using DG.Tweening;
using UnityEngine;

namespace Scripts {
    public class Square: MonoBehaviour {
        public Vector2Int Coords { get; set; }


        float _speed; //todo: задавать при спавне



        public void Move(int stepsVertical, int stepsHorizontal) {
            var targetPos = transform.localPosition + new Vector3(stepsHorizontal * transform.localScale.x,
                stepsVertical * transform.localScale.y);
            float speedModifier = stepsVertical < stepsHorizontal ? stepsVertical : stepsHorizontal;
            transform.DOLocalMove(targetPos, _speed * speedModifier);
        }

        public void Delete() {
        }
    }
}
using UnityEngine;

namespace Scripts {
    public class Balloon: MonoBehaviour {
        [SerializeField] SpriteRenderer _sr;
        public bool MovingLeft => _movingLeft;

        float _time;
        float _speedY;
        float _speedX;
        Vector2 _moveVector;
        Vector2 _startPos;
        bool _movingLeft;
        
        void Update() {
            Move();
        }

        public void SetPositionAndSpeed(Vector3 pos, float speedX, float speedY, bool movingLeft, Sprite sprite) {
            transform.position = pos;
            _startPos = pos;
            _time = 0;
            _speedX = speedX;
            _speedY = speedY;
            _movingLeft = movingLeft;
            _sr.sprite = sprite;
        }
        
        void Move() {
            _time += Time.deltaTime;
            _moveVector.x = _movingLeft ? -_time * _speedX : _time * _speedX;
            _moveVector.y = Mathf.Sin(_time)*_speedY;
            transform.position = _startPos + _moveVector;
        }
    }
}
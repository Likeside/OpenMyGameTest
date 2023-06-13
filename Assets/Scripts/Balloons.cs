using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Scripts {
    public class Balloons: MonoBehaviour {
        [SerializeField] int _maxBalloons;
        [SerializeField] float _maxSpeedX;
        [SerializeField] float _minSpeedX;
        [SerializeField] float _maxSpeedY;
        [SerializeField] float _minSpeedY;
        [SerializeField] float _screenBoundsPaddingMin;
        [SerializeField] float _screenBoundsPaddingMax;
        [SerializeField] float _minYPos;
        [SerializeField] GameObject _baloonPrefab;
        [SerializeField] List<Sprite> _sprites;
        
        List<Balloon> _balloons;
        float _leftBound;
        float _rightBound;
        void Start() {
            Spawn();
            _leftBound = -CameraCalculator.Instance.ScreenWidthHalved - _screenBoundsPaddingMin;
            _rightBound = -_leftBound;
        }

        void Update() {
            foreach (var balloon in _balloons) {
                if (balloon.MovingLeft && balloon.transform.position.x < _leftBound) {
                    SendBack(balloon);
                }
                if (!balloon.MovingLeft && balloon.transform.position.x > _rightBound) {
                    SendBack(balloon);
                }
            }
        }

        void Spawn() {
            _balloons = new List<Balloon>();
            for (int i = 0; i < _maxBalloons; i++) {
                var balloon = Instantiate(_baloonPrefab).GetComponent<Balloon>();
                _balloons.Add(balloon);
                SendBack(balloon);
            }
        }
        
        void SendBack(Balloon balloon) {
            float speedX = Random.Range(_minSpeedX, _maxSpeedX);
            float speedY = Random.Range(_minSpeedY, _maxSpeedY);
            int spriteIndex = Random.Range(0, _sprites.Count);
            balloon.SetPositionAndSpeed(GetRandomPosition(out bool direction), speedX, speedY, direction, _sprites[spriteIndex]);
        }
        
        Vector3 GetRandomPosition(out bool direction) {
            bool left = GetRandomDirection();
            direction = left;
            float xPos = Random.Range(_screenBoundsPaddingMin, _screenBoundsPaddingMax) + CameraCalculator.Instance.ScreenWidthHalved;
            float yPos = Random.Range(_minYPos, CameraCalculator.Instance.ScreenHeightHalved);
            if (!left) xPos = -xPos;
            return new Vector3(xPos, yPos, 0);
        }
        
        bool GetRandomDirection() {
            int random = Random.Range(0, 2);
            if (random == 0) {
                return false;
            }
            return true;
        }
    }
}
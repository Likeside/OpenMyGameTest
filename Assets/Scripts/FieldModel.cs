using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts {
    public class FieldModel {
        //чтобы у игрока не было задержки на инпут, первый свайп обрабатываем сразу
        public event Action<Vector2Int, Vector2Int> OnFirstSwipeEvent;

        //вызываем после того, как нормализовали поле, чтобы отобразить все действия по нормализации во вьюшке
        public event Action<List<Vector2Int>, List<(Vector2Int, Vector2Int)>, bool> OnFieldNormalizedEvent;
        
        //эти два поля хранят порядок действий по нормализации для вьюшки 
        List<Vector2Int> _squaresToDelete;
        List<(Vector2Int, Vector2Int)> _squaresToSwap;
        
        int[,] _fieldArray;
        int _emptyRepresentation = 0;
        int _orangeSquareRepresentation = 1; 
        int _blueSquareRepresentation = 2; //задал тут, но вообще для этого нужен конфиг с указанием репрезентации, цвета спрайта и тд
        
        List<(Vector2Int, Vector2Int)> _crossToCheckForMatch; //используем ниже в методе, чтобы не создавать каждый раз новый
        readonly Vector2Int _separator = new(-Int32.MaxValue, -Int32.MaxValue); //используем как сепаратор, чтобы не спавнить лишние списки 
        
        public void Initialize(int[,] fieldArray) {
              _fieldArray = fieldArray;
            _squaresToDelete = new List<Vector2Int>();
            _squaresToSwap = new List<(Vector2Int, Vector2Int)>();
            _crossToCheckForMatch = new List<(Vector2Int, Vector2Int)>();
        }
        
        public void FirstSwipe(Vector2Int start, Vector2Int target) {
            if (!CheckIfInBoundaries(target.x, target.y)) return; 
            if (target.x < start.x && _fieldArray[target.x, target.y] == _emptyRepresentation) return; //на пустое место не подкидываем квадрат
            (_fieldArray[start.x, start.y], _fieldArray[target.x, target.y]) = (_fieldArray[target.x, target.y], _fieldArray[start.x, start.y]); //свапаем
            OnFirstSwipeEvent?.Invoke(start, target);
            Normalize();
        }

        void Normalize() {
            _squaresToSwap.Clear();
            _squaresToDelete.Clear();
            bool dropped = true;
            bool deleted = true;
            while (dropped || deleted) {
                DropSquares(out dropped);
                DeleteSquares(out deleted);
            }
            bool win = true;
            foreach (var square in _fieldArray) {
                if (square != 0) win = false;
            }
            OnFieldNormalizedEvent?.Invoke(_squaresToDelete, _squaresToSwap, win);
        }
        
        void DropSquares(out bool dropped) {
            _squaresToSwap.Add((_separator, _separator));
            int droppedSquares = 0;
            //проходимся по полю "снизу вверх", чтобы опускались сначала те квадраты, которые стоят ниже
            for (int i = _fieldArray.GetLength(0) - 1; i >= 0; i--) {
                for (int j = 0; j < _fieldArray.GetLength(1); j++) {
                    if (_fieldArray[i, j] == _orangeSquareRepresentation ||
                        _fieldArray[i, j] == _blueSquareRepresentation) {
                        var emptySquaresBelow = EmptySquaresBelow(i, j);
                        if (emptySquaresBelow != 0) {
                            _squaresToSwap.Add((new Vector2Int(i, j), new Vector2Int(i + emptySquaresBelow, j)));
                            _fieldArray[i + emptySquaresBelow, j] = _fieldArray[i, j];
                            _fieldArray[i, j] = _emptyRepresentation;
                            droppedSquares++;
                        }
                    }
                }
            }
            dropped = droppedSquares > 0;
        }

        void DeleteSquares(out bool deleted) {
            _squaresToDelete.Add(_separator);
            int start = _squaresToDelete.Count;
            int deletedSquareLines = 0;
            for (int i = 0; i < _fieldArray.GetLength(0); i++) {
                for (int j = 0; j < _fieldArray.GetLength(1); j++) {
                    if (_fieldArray[i, j] == _orangeSquareRepresentation ||
                        _fieldArray[i, j] == _blueSquareRepresentation) {
                        if (CrossCheckAndAddForDeletion(i, j, _fieldArray[i, j])) {
                            deletedSquareLines++;
                        }
                    }
                }
            }
            for (int i = start; i < _squaresToDelete.Count; i++) {
                //после того, как все вычислили, заменяем все удаленные квадраты на нули, начиная с сепаратора
                _fieldArray[_squaresToDelete[i].x, _squaresToDelete[i].y] = _emptyRepresentation;
            }
            deleted = deletedSquareLines > 0;
        }

        int EmptySquaresBelow(int i, int j) {
            int emptySquaresBelow = 0;
            for (int k = 1; k < _fieldArray.GetLength(0); k++) {
                if (i + k > _fieldArray.GetLength(0) - 1) return emptySquaresBelow;
                if (_fieldArray[i + k, j] == _emptyRepresentation) emptySquaresBelow++;
            }
            return emptySquaresBelow;
        }

        bool CrossCheckAndAddForDeletion(int i, int j, int representation) {
            bool squareMustBeDeleted = false;
            foreach (var coordsToCheck in GetCrossToCheckForMatch(i, j)) {
                int y1 = coordsToCheck.Item1.x;
                int x1 = coordsToCheck.Item1.y;
                int y2 = coordsToCheck.Item2.x;
                int x2 = coordsToCheck.Item2.y;
                if (CheckIfInBoundaries(y1, x1) && CheckIfInBoundaries(y2, x2)) {
                    if (_fieldArray[y1, x1] == representation && _fieldArray[y2, x2] == representation) {
                        _squaresToDelete.Add(new Vector2Int(y1, x1));
                        _squaresToDelete.Add(new Vector2Int(y2, x2));
                        squareMustBeDeleted = true;
                    }
                }
            }
            return squareMustBeDeleted;
        }

        List<(Vector2Int, Vector2Int)> GetCrossToCheckForMatch(int i, int j) {
            //получаем координаты, формирующие "крест". Предполагается, что у креста одна "сторона" это 3 квадрата,
            //но в идеале количество матчашихся квадратов задавать извне и проходиться циклом, вместо кортежа использовать список
            _crossToCheckForMatch.Clear();
            _crossToCheckForMatch.Add((new Vector2Int(i + 1, j), new Vector2Int(i + 2, j)));
            _crossToCheckForMatch.Add((new Vector2Int(i - 1, j), new Vector2Int(i - 2, j)));
            _crossToCheckForMatch.Add((new Vector2Int(i, j + 1), new Vector2Int(i, j + 2)));
            _crossToCheckForMatch.Add((new Vector2Int(i, j - 1), new Vector2Int(i, j - 2)));
            return _crossToCheckForMatch;
        }

        bool CheckIfInBoundaries(int i, int j) {
            if (i >= _fieldArray.GetLength(0) || i < 0 || j >= _fieldArray.GetLength(1) || j < 0) {
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts {
    public class FieldView: MonoBehaviour {

        [SerializeField] GridBase _grid;
        List<Square> _allSquares;

       public event Action<Vector2Int, Vector2Int> OnTryingToSwapFirstEvent;
       public event Action OnAllClearedEvent;
       
       readonly Vector2Int _separator = new(-Int32.MaxValue, -Int32.MaxValue);
       int _deletedSquares;
       int _droppedSquares;
       bool _win;


       public void Initialize(int[,] fieldArray) {
           _allSquares = _grid.CreateGrid(fieldArray);
       }

        public void TryToSwapFirst(Vector2Int start, Vector2Int target) {
            OnTryingToSwapFirstEvent?.Invoke(start, target);
        }
        
        public void SwapFirst(Vector2Int start, Vector2Int target) {
            Debug.Log("Blocking interaction");
            foreach (var square in _allSquares) {
                square.SetInteraction(false);
            }
            var square1 = _allSquares.FirstOrDefault(s => s.Coords == start);
            var square2 = _allSquares.FirstOrDefault(s => s.Coords == target);
            if (square1 != null) {
                square1.Move(target.x - start.x, target.y - start.y);
            }
            if (square2 != null) {
                square2.Move(start.x - target.x, start.y - target.y);
            }
        }
        
        public void Normalize(List<Vector2Int> squaresToDelete, List<(Vector2Int, Vector2Int)> squaresToDrop, bool win) {
            _win = win;
            StartCoroutine(NormalizeCor(squaresToDelete, squaresToDrop));
        }

        IEnumerator NormalizeCor(List<Vector2Int> squaresToDelete, List<(Vector2Int, Vector2Int)> squaresToDrop) {
            yield return new WaitForSeconds(1f); //TODO: задавать извне, ждем, пока свапнется первый квадрат
            _droppedSquares = 1;
            _deletedSquares = 1;
            Drop(squaresToDrop, squaresToDelete);
        }
        
        void Drop(List<(Vector2Int, Vector2Int)> squaresToDrop, List<Vector2Int> squaresToDelete) {
            Debug.Log("Drop");
            float delay = MathF.Abs(squaresToDrop[0].Item1.x - squaresToDrop[0].Item2.x)/2f;
            for (int i = _droppedSquares; i <= squaresToDrop.Count; i++) {
                _droppedSquares++;
                if (i < squaresToDrop.Count) {
                    float diff = Mathf.Abs(squaresToDrop[i].Item1.x - squaresToDrop[i].Item2.x)/2f;
                    if (diff > delay) {
                        delay = diff;
                    }
                }
                UnblockInteraction(squaresToDrop, squaresToDelete, delay);
                if (i == squaresToDrop.Count || squaresToDrop[i].Item1 == _separator) {
                    StartCoroutine(DeleteCor(squaresToDrop, squaresToDelete, delay));
                    return;
                }
                DropOneSquare(squaresToDrop[i]);
            }
        }
        IEnumerator DeleteCor(List<(Vector2Int, Vector2Int)> squaresToDrop, List<Vector2Int> squaresToDelete, float delay) {
            yield return new WaitForSeconds(delay);
            Debug.Log("Delete");
            for (int i = _deletedSquares; i < squaresToDelete.Count; i++) {
                _deletedSquares++;
                UnblockInteraction(squaresToDrop, squaresToDelete, 1f);
                if (squaresToDelete[i] == _separator) {
                   Drop(squaresToDrop, squaresToDelete);
                    yield break;
                }
                DeleteOneSquare(squaresToDelete[i]);
            }
        }
        
        void DropOneSquare((Vector2Int, Vector2Int) squareToDrop) {
            var square = _allSquares.FirstOrDefault(s => s.Coords == squareToDrop.Item1);
            if (square == null) {
                Debug.Log("Trying to drop non-existent square");
                return;
            }
            square.Move(squareToDrop.Item2.x - squareToDrop.Item1.x, 0);
        }
        
        void DeleteOneSquare(Vector2Int squareToDelete) {
            var square = _allSquares.FirstOrDefault(s => s.Coords == squareToDelete);
            if (square == null) {
                Debug.Log("Trying to delete non-existent square");
                return;
            }
            square.Delete();
        }
        
        void UnblockInteraction(List<(Vector2Int, Vector2Int)> squaresToDrop, List<Vector2Int> squaresToDelete, float delay) {
            if (_droppedSquares == squaresToDrop.Count && _deletedSquares == squaresToDelete.Count) {
                Debug.Log("Unblocking interaction");
                StartCoroutine(UnblockInteractionCor(delay));
            }
        }

        IEnumerator UnblockInteractionCor(float delay) {
            yield return new WaitForSeconds(delay);
            foreach (var square in _allSquares) {
                square.SetInteraction(true);
            }
            if (_win) OnAllClearedEvent?.Invoke();
        }
    }
}
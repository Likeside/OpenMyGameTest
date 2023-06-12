using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts {
    public class FieldView: MonoBehaviour {

        List<Square> _allSquares;
        
        readonly Vector2Int _separator = new(-Int32.MaxValue, -Int32.MaxValue);

        int _deletedSquares;
        int _droppedSquares;
        
        public void SwapFirst(Vector2Int start, Vector2Int target) {
            var square1 = _allSquares.FirstOrDefault(s => s.Coords == start);
            var square2 = _allSquares.FirstOrDefault(s => s.Coords == target);
            if (square1 != null) {
                square1.Move(target.x - start.x, target.y - start.y);
            }
            if (square2 != null) {
                square2.Move(start.x - target.x, start.y - target.sqrMagnitude);
            }
        }
        
        public void Normalize(List<Vector2Int> squaresToDelete, List<(Vector2Int, Vector2Int)> squaresToDrop) {
            _droppedSquares = 0;
            _deletedSquares = 0;
            Drop(squaresToDrop, squaresToDelete);
        }
        
        void Drop(List<(Vector2Int, Vector2Int)> squaresToDrop, List<Vector2Int> squaresToDelete) {
            for (int i = _droppedSquares; i < squaresToDrop.Count; i++) {
                _droppedSquares++;
                if (squaresToDrop[i].Item1 == _separator) {
                    Delete(squaresToDrop, squaresToDelete);
                    return;
                }
                DropOneSquare(squaresToDrop[i]);
            }
        }

        void Delete(List<(Vector2Int, Vector2Int)> squaresToDrop, List<Vector2Int> squaresToDelete) {
            for (int i = _deletedSquares; i < squaresToDelete.Count; i++) {
                _deletedSquares++;
                if (squaresToDelete[i] == _separator) {
                    Drop(squaresToDrop, squaresToDelete);
                    return;
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
            square.Move(squareToDrop.Item2.x - squareToDrop.Item1.x, squareToDrop.Item2.y = squareToDrop.Item1.y);
        }
        
        void DeleteOneSquare(Vector2Int squareToDelete) {
            var square = _allSquares.FirstOrDefault(s => s.Coords == squareToDelete);
            if (square == null) {
                Debug.Log("Trying to delete non-existent square");
                return;
            }
            square.Delete();
        }
    }
}
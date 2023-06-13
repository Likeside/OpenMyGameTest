using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    [SerializeField]  GameObject _square;
    [SerializeField] Sprite _orangeSprite;
    [SerializeField] Sprite _blueSprite; //todo: убрать куда-то, когда сделаю анимацию
    
    Vector2 _offset = Vector2.zero;
     Vector2 _startPosition;

     readonly List<GameObject> _gridSquares = new();
     readonly List<GameObject> _emptySquares = new();


     public List<Square> CreateGrid(int[,] fieldArray) {
       SpawnGridSquares(fieldArray, _square);
       SetGridSquarePositions(fieldArray);
       DeleteEmptySquares();
       return _gridSquares.Select(s => s.GetComponent<Square>()).ToList();
    }

    //спавним квадраты, их количество равно количеству элементов в массиве, на месте нулей также добавляем квадраты в список пустых
     void SpawnGridSquares(int[,] squareArray, GameObject square) {
        for (int row = 0; row < squareArray.GetLength(0); ++row)
        {
            for (int column = 0; column < squareArray.GetLength(1); ++column)
            {
                if(squareArray[row, column] == 0 ) 
                {
                    var empty = Instantiate(square);
                    _gridSquares.Add(empty);
                    _emptySquares.Add(empty);
                    
                }
                else {
                    var squareObj = Instantiate(square);
                    squareObj.GetComponent<Square>().Set(new Vector2Int(row, column), squareArray[row,column] == 1? _orangeSprite: _blueSprite);
                    _gridSquares.Add(squareObj);
                }
                _gridSquares[_gridSquares.Count-1].transform.SetParent(transform); //делаем все квадраты дочерними объектами сетки
            }
        }
    }
    
    //устанавливаем позицию квадратов
     void SetGridSquarePositions(int[,] squareArray) {
        int columnNumer = 0;
        int rowNumber = 0;

        var squareRect = _gridSquares[0].GetComponent<SpriteRenderer>();
        
        //шаг смещения квадрата равен его ширине умноженной на скейл. 
        _offset.x = squareRect.size.x * squareRect.transform.localScale.x; 
        _offset.y = squareRect.size.y * squareRect.transform.localScale.y; 

        //стартовая позиция (позиция верхнего левого квадрата) "центрирует" квадраты относительно родителя (сетки)
        _startPosition.x = -(squareArray.GetLength(1) / 2) * (_offset.x) + _offset.x / 2f; 
        _startPosition.y = (squareArray.GetLength(0) / 2) * (_offset.y) - _offset.y / 2f;

        foreach (var square in _gridSquares)
        {
            
            //если номер столбца превышает количество столбцов в массиве, переходим на следующий ряд
            if (columnNumer+1 > squareArray.GetLength(1))
            {
                columnNumer = 0;
                rowNumber++;
            }

            //вычисляем расстояние, на которое нужно подвинуть квадрат (шаг смещения умноженное на колонку и ряд)
            var offsetPosX = _offset.x * columnNumer;
            var offsetPosY = _offset.y * rowNumber;
            
            square.transform.localPosition = new Vector3(_startPosition.x + offsetPosX, _startPosition.y - offsetPosY, 0);
            columnNumer++;
        }
    }

//удаляем пустые квадраты
     void DeleteEmptySquares() {
        if (_emptySquares.Any())
        {
            foreach (var square in _emptySquares)
            {
                _gridSquares.Remove(square);
                Destroy(square);
            }
        }
    }

   
}

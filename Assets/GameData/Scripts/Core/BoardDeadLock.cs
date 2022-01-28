using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Match3.Enums;

namespace Match3.Core
{
    public class BoardDeadLock : MonoBehaviour
    {
        #region Public Methods
        public List<GamePiece> GetRowOrColoumnList(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
        {
            List<GamePiece> piecesList = new List<GamePiece>();
            int width = allPieces.GetLength(0);
            int height = allPieces.GetLength(1);
            for(int i = 0; i < listLength; i++)
            {
                if(checkRow)
                {
                    if(x + i < width && y < height)
                    {
                        piecesList.Add(allPieces[x + i, y]);
                    }
                }
                else
                {
                    if(x < width && y + i < height)
                    {
                        piecesList.Add(allPieces[x , y + i]);
                    }
                }
            }
            return piecesList;
        }

        public List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minLength = 2)
        {
            List<GamePiece> matches = new List<GamePiece>();
            var groups = gamePieces.GroupBy(n => n.PieceType);
            foreach(var item in groups)
            {
                if(item != null)
                {
                    if (item.Count() >= minLength && item.Key != GamePieceType.None)
                    {
                        matches = item.ToList();
                    }
                }
            }
            return matches;
        }

        public List<GamePiece> GetNeighbors(GamePiece[,] allPieces, int x, int y)
        {
            List<GamePiece> neighbors = new List<GamePiece>();
            int width = allPieces.GetLength(0);
            int height = allPieces.GetLength(1);
            Vector2[] searchDirections = new Vector2[4] 
            {
                new Vector2(-1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, -1f),
                new Vector2(0f, 1f)
            };
            foreach(var direction in searchDirections)
            {
                if(direction != null)
                {
                    if(x + (int)direction.x >= 0 && x + (int)direction.x < width && 
                       y + (int)direction.y >= 0 && y + (int)direction.y < height)
                    {
                        if(allPieces[x + (int)direction.x, y + (int)direction.y] != null && !neighbors.Contains(allPieces[x + (int)direction.x, y + (int)direction.y]))
                        {
                            neighbors.Add(allPieces[x + (int)direction.x, y + (int)direction.y]);
                        }
                    }
                }
            }
            return neighbors;
        }
        #endregion
    }
}
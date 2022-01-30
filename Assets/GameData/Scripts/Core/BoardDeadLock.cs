using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Match3.Enums;
using Match3.Utilities;

namespace Match3.Core
{
    public class BoardDeadLock : MonoBehaviour
    {
        #region Private Methods
        private List<GamePiece> GetRowOrColoumnList(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
        {
            List<GamePiece> piecesList = new List<GamePiece>();
            int width = allPieces.GetLength(0);
            int height = allPieces.GetLength(1);
            for (int i = 0; i < listLength; i++)
            {
                if (checkRow)
                {
                    if (x + i < width && y < height && allPieces[x + i, y] != null)
                    {
                        piecesList.Add(allPieces[x + i, y]);
                    }
                }
                else
                {
                    if (x < width && y + i < height && allPieces[x, y + i] != null)
                    {
                        piecesList.Add(allPieces[x, y + i]);
                    }
                }
            }
            return piecesList;
        }

        private List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minLength = 2)
        {
            List<GamePiece> matches = new List<GamePiece>();
            var groups = gamePieces.GroupBy(n => n.PieceType);
            foreach (var item in groups)
            {
                if (item != null)
                {
                    if (item.Count() >= minLength && item.Key != GamePieceType.None && item.Key != GamePieceType.Collectable)
                    {
                        matches = item.ToList();
                    }
                }
            }
            return matches;
        }

        private List<GamePiece> GetNeighbors(GamePiece[,] allPieces, int x, int y)
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
            foreach (var direction in searchDirections)
            {
                if (direction != null)
                {
                    if (x + (int)direction.x >= 0 && x + (int)direction.x < width &&
                       y + (int)direction.y >= 0 && y + (int)direction.y < height)
                    {
                        if (allPieces[x + (int)direction.x, y + (int)direction.y] != null && !neighbors.Contains(allPieces[x + (int)direction.x, y + (int)direction.y]))
                        {
                            neighbors.Add(allPieces[x + (int)direction.x, y + (int)direction.y]);
                        }
                    }
                }
            }
            return neighbors;
        }

        private bool HasMoveAt(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
        {
            List<GamePiece> pieces = GetRowOrColoumnList(allPieces, x, y, listLength, checkRow);
            List<GamePiece> matches = GetMinimumMatches(pieces, listLength - 1);
            GamePiece unmatchedPiece = null;
            if(pieces != null && matches != null)
            {
                if(pieces.Count == listLength && matches.Count == listLength - 1)
                {
                    unmatchedPiece = pieces.Except(matches).FirstOrDefault();
                }
                if(unmatchedPiece != null)
                {
                    List<GamePiece> neighbors = GetNeighbors(allPieces, unmatchedPiece.XIndex, unmatchedPiece.YIndex);
                    neighbors = neighbors.Except(matches).ToList();
                    neighbors = neighbors.FindAll(x => x.PieceType == matches[0].PieceType);
                    matches = matches.Union(neighbors).ToList();
                }
                if(matches.Count >= listLength)
                {
                    //string rowColoumnString = (checkRow) ? "Row" : "Coloumn";
                    //Debug.Log($"Avaiable Move-----> Move {matches[0].PieceType} piece to {unmatchedPiece.XIndex} ,{unmatchedPiece.YIndex} to form Match {rowColoumnString}".ToAqua().ToBold());
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Public Methods
        public bool IsDeadLocked(GamePiece[,] allPieces, int listLength = 3)
        {
            bool isDeadLocked = true;
            int width = allPieces.GetLength(0);
            int height = allPieces.GetLength(1);
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if(HasMoveAt(allPieces, i, j, listLength, true) || HasMoveAt(allPieces, i, j, listLength, false))
                    {
                        isDeadLocked = false;
                    }
                }
            }
            if(isDeadLocked)
            {
                Debug.Log($"-------Board Deadlocked--------".ToRed().ToBold());
            }
            return isDeadLocked;
        }
        #endregion
    }
}
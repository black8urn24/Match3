using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class BoardShuffler : MonoBehaviour
    {
        #region Public Methods
        public List<GamePiece> RemoveNormalPieces(GamePiece[,] gamePieces)
        {
            List<GamePiece> normalPieces = new List<GamePiece>();
            int width = gamePieces.GetLength(0);
            int height = gamePieces.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (gamePieces[i, j] != null)
                    {
                        Bomb bomb = gamePieces[i, j].GetComponent<Bomb>();
                        Collectable collectable = gamePieces[i, j].GetComponent<Collectable>();
                        if (bomb == null && collectable == null)
                        {
                            normalPieces.Add(gamePieces[i, j]);
                            gamePieces[i, j] = null;
                        }
                    }
                }
            }
            return normalPieces;
        }

        public void ShuffleGamePieces(List<GamePiece> gamePieces)
        {
            int maxCount = gamePieces.Count;
            GamePiece temp = null;
            for (int i = 0; i < maxCount - 1; i++)
            {
                int randomIndex = Random.Range(i, maxCount);
                if (randomIndex == i)
                {
                    continue;
                }
                temp = gamePieces[i];
                gamePieces[i] = gamePieces[randomIndex];
                gamePieces[randomIndex] = temp;
            }
        }

        public void MovePieces(GamePiece[,] gamePieces, float moveTime = 0.5f)
        {
            int width = gamePieces.GetLength(0);
            int height = gamePieces.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if(gamePieces[i,j] != null)
                    {
                        gamePieces[i, j].MovePiece(i, j, moveTime);
                    }
                }
            }
        }
        #endregion
    }
}
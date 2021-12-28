using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Utilities;

namespace Match3.Core
{
    public class GameTile : MonoBehaviour
    {
        #region Variables
        private int xIndex = -1;
        private int yIndex = -1;
        private GameBoard currentGameBoard = null;
        #endregion

        #region Properties
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Public Methods
        public void InitializeTile(int x, int y, GameBoard gameBoard)
        {
            xIndex = x;
            yIndex = y;
            currentGameBoard = gameBoard;
            Debug.Log($"Tile Initialized with details - {xIndex} and {yIndex}".ToOrange().ToBold());
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Utilities;
using Match3.Enums;

namespace Match3.Core
{
    public class GameTile : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TileType tileType = TileType.Normal;
        #endregion

        #region Variables
        private int xIndex = -1;
        private int yIndex = -1;
        private GameBoard currentGameBoard = null;
        #endregion

        #region Properties
        public int XIndex { get => xIndex; set => xIndex = value; }
        public int YIndex { get => yIndex; set => yIndex = value; }
        public TileType TileType { get => tileType; set => tileType = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnMouseDown()
        {
            if(currentGameBoard != null)
            {
                currentGameBoard.SetClickedTile(this);
            }
        }

        private void OnMouseEnter()
        {
            if(currentGameBoard != null)
            {
                currentGameBoard.SetTargetTile(this);
            }
        }

        private void OnMouseUp()
        {
            if(currentGameBoard != null)
            {
                currentGameBoard.ReleaseTile();
            }
        }
        #endregion

        #region Public Methods
        public void InitializeTile(int x, int y, GameBoard gameBoard)
        {
            XIndex = x;
            YIndex = y;
            currentGameBoard = gameBoard;
            //Debug.Log($"Tile Initialized with details - {xIndex} and {yIndex}".ToOrange().ToBold());
        }
        #endregion
    }
}
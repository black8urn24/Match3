using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Enums;
using Match3.Utilities;

namespace Match3.Core
{
    public class GamePiece : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GamePieceType pieceType = GamePieceType.None;
        #endregion

        #region Variables
        private int x = -1;
        private int y = -1;
        #endregion

        #region Properties
        public GamePieceType PieceType { get => pieceType; private set => pieceType = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        public void SetCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion
    }
}
using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class GameManager : Singleton<GameManager>
    {
        #region Inspector Variables
        [SerializeField] private ScreenFader initialScreenFader = null;
        [SerializeField] private GameBoard gameBoard = null;
        #endregion

        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            SetInitialReferences();
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Private Methods
        private void SetInitialReferences()
        {
            if(initialScreenFader != null)
            {
                initialScreenFader.FadeOut();
            }
            if(gameBoard != null)
            {
                gameBoard.SetupBoard();
            }
        }
        #endregion

        #region Coroutines
        #endregion

        #region Public Methods
        #endregion
    }
}
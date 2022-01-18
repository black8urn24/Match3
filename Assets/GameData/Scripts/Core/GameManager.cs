using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Match3.Core
{
    public class GameManager : Singleton<GameManager>
    {
        #region Inspector Variables
        [SerializeField] private ScreenFader initialScreenFader = null;
        [SerializeField] private GameBoard gameBoard = null;
        [SerializeField] private TextMeshProUGUI movesCounterText = null;
        [SerializeField] private int levelMoves = 30;
        #endregion

        #region Variables
        private bool isReadyToBegin = false;
        private bool isGameOver = false;
        private bool isWinner = false;
        private int currentMoves = -1;
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
            StartCoroutine(ExecuteGameLoop());
        }

        private void SetMovesText()
        {
            if (movesCounterText.text != null)
            {
                movesCounterText.text = currentMoves.ToString();
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator ExecuteGameLoop()
        {
            yield return StartCoroutine(StartGameRoutine());
            yield return StartCoroutine(PlayGameRoutine());
            yield return StartCoroutine(EndGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            while (!isReadyToBegin)
            {
                yield return null;
                yield return new WaitForSeconds(1f);
                isReadyToBegin = true;
            }
            if (initialScreenFader != null)
            {
                initialScreenFader.FadeOut();
            }
            yield return new WaitForSeconds(1f);
            if (gameBoard != null)
            {
                gameBoard.SetupBoard();
            }
            currentMoves = levelMoves;
            SetMovesText();
        }

        private IEnumerator PlayGameRoutine()
        {
            while (!isGameOver)
            {
                if(currentMoves <= 0)
                {
                    isGameOver = true;
                    isWinner = false;
                    if (initialScreenFader != null)
                    {
                        initialScreenFader.FadeIn();
                    }
                }
                yield return null;
            }
        }

        private IEnumerator EndGameRoutine()
        {
            if (isWinner)
            {

            }
            else
            {

            }
            yield return null;
        }
        #endregion

        #region Public Methods
        public void UpdateMoves()
        {
            currentMoves--;
            if(currentMoves <= 0)
            {
                currentMoves = 0;
            }
            SetMovesText();
        }
        #endregion
    }
}
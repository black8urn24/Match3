using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Match3.Core
{
    public class GameManager : Singleton<GameManager>
    {
        #region Inspector Variables
        [SerializeField] private ScreenFader initialScreenFader = null;
        [SerializeField] private GameBoard gameBoard = null;
        [SerializeField] private TextMeshProUGUI movesCounterText = null;
        [SerializeField] private int levelMoves = 30;
        [SerializeField] private int targetScore = 10000;
        [SerializeField] private Sprite goalSprite = null;
        [SerializeField] private Sprite winSprite = null;
        [SerializeField] private Sprite looseSprite = null;
        [SerializeField] private MessageWindowManager messageWindow = null;
        #endregion

        #region Variables
        private bool isReadyToBegin = false;
        private bool isGameOver = false;
        private bool isWinner = false;
        private int currentMoves = -1;
        private ScoreManager scoreManager = null;
        #endregion

        #region Properties
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            SetInitialReferences();
        }
        #endregion

        #region Private Methods
        private void SetInitialReferences()
        {
            scoreManager = ScoreManager.Instance;
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
            if(messageWindow != null)
            {
                messageWindow.SetWindow(goalSprite, "Goal : " + targetScore.ToString(), "Start", () => 
                {
                    isReadyToBegin = true;
                });
            }
            while (!isReadyToBegin)
            {
                yield return null;
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
                if(scoreManager != null)
                {
                    if(scoreManager.GetCurrentScore() >= targetScore)
                    {
                        isGameOver = true;
                        isWinner = true;
                    }
                }
                if(currentMoves <= 0)
                {
                    if(scoreManager.GetCurrentScore() >= targetScore)
                    {
                        isGameOver = true;
                        isWinner = true;
                    }
                    else
                    {
                        isGameOver = true;
                        isWinner = false;
                    }
                }
                yield return null;
            }
        }

        private IEnumerator EndGameRoutine()
        {
            if (isWinner)
            {
                if (messageWindow != null)
                {
                    messageWindow.SetWindow(winSprite, "You Win", "Okay", () =>
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    });
                }
            }
            else
            {
                if (messageWindow != null)
                {
                    messageWindow.SetWindow(looseSprite, "You loose", "Okay", () =>
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    });
                }
            }
            yield return new WaitForSeconds(1f);
            if (initialScreenFader != null)
            {
                initialScreenFader.FadeIn();
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
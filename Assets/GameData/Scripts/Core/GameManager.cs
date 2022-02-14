using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FullSerializer;
using Match3.Enums;

namespace Match3.Core
{
    [RequireComponent(typeof(LevelGoal))]
    public class GameManager : Singleton<GameManager>
    {
        #region Inspector Variables
        [SerializeField] private GameBoard gameBoard = null;
        [SerializeField] private Sprite goalSprite = null;
        [SerializeField] private Sprite winSprite = null;
        [SerializeField] private Sprite looseSprite = null;
        [Header("Testing")]
        [SerializeField] private bool loadLevelFromJsonFile = false;
        [SerializeField] private int levelIndex = -1;
        #endregion

        #region Variables
        private bool isReadyToBegin = false;
        private bool isGameOver = false;
        private bool isWinner = false;
        private int currentMoves = -1;
        private int targetScore = 10000;
        private ScoreManager scoreManager = null;
        private Level currentGameLevel = null;
        private LevelGoal levelGoal = null;
        private CollectionLevelGoal levelGoalCollected = null;
        #endregion

        #region Properties
        public Level CurrentGameLevel { get => currentGameLevel; set => currentGameLevel = value; }
        public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
        public LevelGoal LevelGoal { get => levelGoal; set => levelGoal = value; }
        #endregion

        #region Unity Methods
        public override void Awake()
        {
            base.Awake();
            LevelGoal = GetComponent<LevelGoal>();
            levelGoalCollected = GetComponent<CollectionLevelGoal>();
        }

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
            if (!loadLevelFromJsonFile)
            {
                currentMoves = levelGoal.MovesForLevel;
                levelGoal.MovesLeft = currentMoves;
                targetScore = levelGoal.ScoreGoals[0];
                levelGoal.TargetScore = targetScore;
                SetMovesText();
                if (UiManager.Instance != null)
                {
                    if (UiManager.Instance.ScoreMeter != null)
                    {
                        UiManager.Instance.ScoreMeter.SetLevelGoal(levelGoal);
                    }
                    if (LevelGoal.LevelCounterType == LevelCounterType.Timer)
                    {
                        if (UiManager.Instance.MovesCounterText != null)
                        {
                            UiManager.Instance.MovesCounterText.text = "\u221E";
                            UiManager.Instance.MovesCounterText.fontSize = 70;
                            UiManager.Instance.TimeCountDownParent.SetActive(true);
                            UiManager.Instance.MovesParent.SetActive(false);
                        }
                    }
                    else
                    {
                        UiManager.Instance.TimeCountDownParent.SetActive(false);
                        UiManager.Instance.MovesParent.SetActive(true);
                    }
                    if (levelGoalCollected != null)
                    {
                        UiManager.Instance.SetupCollectionGoalLayout(levelGoalCollected.GetCollectionGoals());
                        //levelGoalCollected.SetupGoalUI();
                    }
                }
                StartCoroutine(ExecuteGameLoop());
            }
        }

        private void SetMovesText()
        {
            if (UiManager.Instance != null && UiManager.Instance.MovesCounterText != null)
            {
                UiManager.Instance.MovesCounterText.text = currentMoves.ToString();
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator ExecuteGameLoop()
        {
            yield return StartCoroutine(StartGameRoutine());
            yield return StartCoroutine(PlayGameRoutine());
            yield return StartCoroutine(WaitForBoardRoutine(0.5f));
            yield return StartCoroutine(EndGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            if (UiManager.Instance != null && UiManager.Instance.MessageWindow != null)
            {
                UiManager.Instance.MessageWindow.SetWindow(goalSprite, "Goal : " + targetScore.ToString(), "Start", () =>
                {
                    isReadyToBegin = true;
                });
            }
            while (!isReadyToBegin)
            {
                yield return null;
            }
            if (UiManager.Instance != null && UiManager.Instance.InitialScreenFader != null)
            {
                UiManager.Instance.InitialScreenFader.FadeOut();
            }
            yield return new WaitForSeconds(1f);
            if (gameBoard != null)
            {
                gameBoard.SetupBoard();
            }
        }

        private IEnumerator PlayGameRoutine()
        {
            if (LevelGoal.LevelCounterType == LevelCounterType.Timer)
            {
                LevelGoal.StartCountdown();
            }
            while (!isGameOver)
            {
                isGameOver = levelGoal.IsGameOver();
                isWinner = levelGoal.IsWinner();
                yield return null;
            }
        }

        private IEnumerator EndGameRoutine()
        {
            if (isWinner)
            {
                if (UiManager.Instance != null && UiManager.Instance.MessageWindow != null)
                {
                    UiManager.Instance.MessageWindow.SetWindow(winSprite, "You Win", "Okay", () =>
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    });
                }
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayRandomWinClips();
                }
            }
            else
            {
                if (UiManager.Instance != null && UiManager.Instance.MessageWindow != null)
                {
                    UiManager.Instance.MessageWindow.SetWindow(looseSprite, "You loose", "Okay", () =>
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    });
                }
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayRandomLooseClips();
                }
            }
            yield return new WaitForSeconds(1f);
            if (UiManager.Instance != null && UiManager.Instance.InitialScreenFader != null)
            {
                UiManager.Instance.InitialScreenFader.FadeIn();
            }
            yield return null;
        }

        private IEnumerator WaitForBoardRoutine(float delay)
        {
            if (LevelGoal.LevelCounterType != LevelCounterType.Timer)
            {
                if (UiManager.Instance != null && UiManager.Instance.LevelTimer != null)
                {
                    UiManager.Instance.LevelTimer.IsPaused = true;
                }
            }
            if (gameBoard != null)
            {
                yield return new WaitForSeconds(gameBoard.GetGamePieceMoveSpeed());
                while (gameBoard.IsRefilling)
                {
                    yield return null;
                }
            }
            yield return new WaitForSeconds(delay);
        }
        #endregion

        #region Public Methods
        public void UpdateMoves()
        {
            if (LevelGoal.LevelCounterType == LevelCounterType.Moves)
            {
                currentMoves--;
                if (currentMoves <= 0)
                {
                    currentMoves = 0;
                }
                levelGoal.MovesLeft = currentMoves;
                SetMovesText();
            }
            else
            {
                if (UiManager.Instance != null && UiManager.Instance.MovesCounterText != null)
                {
                    UiManager.Instance.MovesCounterText.text = "\u221E";
                    UiManager.Instance.MovesCounterText.fontSize = 70;
                }
            }
        }

        public void AddScore(GamePiece gamePiece, int multiplier = 1, int bonus = 0)
        {
            if (gamePiece != null)
            {
                if (scoreManager != null)
                {
                    scoreManager.AddScore(gamePiece.ScoreValue * multiplier + bonus);
                    levelGoal.UpdateScoreStars(scoreManager.GetCurrentScore());
                    if (UiManager.Instance != null && UiManager.Instance.ScoreMeter != null)
                    {
                        UiManager.Instance.ScoreMeter.UpdateScoreMeter(scoreManager.GetCurrentScore(), levelGoal.ScoreStars);
                    }
                }
                if (AudioManager.Instance != null)
                {
                    if (gamePiece.PieceBreakSound != null)
                    {
                        AudioManager.Instance.PlayPieceDestroyClip(gamePiece.PieceBreakSound, PoolObjectsType.SFXAudioSource, false);
                    }
                }
            }
        }

        public void Addtime(int value)
        {
            if (LevelGoal.LevelCounterType == LevelCounterType.Timer)
            {
                LevelGoal.Addtime(value);
            }
        }

        public void CheckForCollectionGoals(GamePiece gamePiece)
        {
            if (levelGoalCollected != null)
            {
                levelGoalCollected.UpdateGoals(gamePiece);
            }
        }

        public void CheckForLevelLoad()
        {
            if (loadLevelFromJsonFile)
            {
                var vFilePath = GlobalConstants.Match3LevelFilePrefix + GlobalConstants.Match3LevelPrefix + levelIndex;
                if (FileUtilities.FileExists(vFilePath))
                {
                    currentGameLevel = FileUtilities.LoadJsonFile<Level>(vFilePath);
                }
                if (currentGameLevel != null)
                {
                    currentMoves = currentGameLevel.moves;
                    levelGoal.MovesLeft = currentMoves;
                    SetMovesText();
                    targetScore = currentGameLevel.targetScore;
                    levelGoal.TargetScore = targetScore;
                    if (UiManager.Instance != null)
                    {
                        if (UiManager.Instance.ScoreMeter != null)
                        {
                            UiManager.Instance.ScoreMeter.SetLevelGoal(levelGoal);
                        }
                        if (LevelGoal.LevelCounterType == LevelCounterType.Timer)
                        {
                            if (UiManager.Instance.MovesCounterText != null)
                            {
                                UiManager.Instance.MovesCounterText.text = "\u221E";
                                UiManager.Instance.MovesCounterText.fontSize = 70;
                                UiManager.Instance.TimeCountDownParent.SetActive(true);
                                UiManager.Instance.MovesParent.SetActive(false);
                            }
                        }
                        else
                        {
                            UiManager.Instance.TimeCountDownParent.SetActive(false);
                            UiManager.Instance.MovesParent.SetActive(true);
                        }
                        if (levelGoalCollected != null)
                        {
                            UiManager.Instance.SetupCollectionGoalLayout(levelGoalCollected.GetCollectionGoals());
                        }
                    }
                    StartCoroutine(ExecuteGameLoop());
                }
            }
        }
        #endregion
    }
}
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
        [SerializeField] private ScreenFader initialScreenFader = null;
        [SerializeField] private GameBoard gameBoard = null;
        [SerializeField] private TextMeshProUGUI movesCounterText = null;
        [SerializeField] private Sprite goalSprite = null;
        [SerializeField] private Sprite winSprite = null;
        [SerializeField] private Sprite looseSprite = null;
        [SerializeField] private MessageWindowManager messageWindow = null;
        [SerializeField] private ScoreMeter scoreMeter = null;
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
        private TimeLevelGoal timeLevelGoal = null;
        private LevelGoalCollected levelGoalCollected = null;
        #endregion

        #region Properties
        public Level CurrentGameLevel { get => currentGameLevel; set => currentGameLevel = value; }
        public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
        public TimeLevelGoal TimeLevelGoal { get => timeLevelGoal; set => timeLevelGoal = value; }
        #endregion

        #region Unity Methods
        public override void Awake()
        {
            base.Awake();
            levelGoal = GetComponent<LevelGoal>();
            TimeLevelGoal = GetComponent<TimeLevelGoal>();
            levelGoalCollected = GetComponent<LevelGoalCollected>();
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
                if (scoreMeter != null)
                {
                    scoreMeter.SetLevelGoal(levelGoal);
                }
                if (TimeLevelGoal != null)
                {
                    if (movesCounterText != null)
                    {
                        movesCounterText.text = "\u221E";
                        movesCounterText.fontSize = 70;
                    }
                }
                StartCoroutine(ExecuteGameLoop());
            }
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
            yield return StartCoroutine(WaitForBoardRoutine(0.5f));
            yield return StartCoroutine(EndGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            if (messageWindow != null)
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
        }

        private IEnumerator PlayGameRoutine()
        {
            if (TimeLevelGoal != null)
            {
                TimeLevelGoal.StartCountdown();
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
                if (messageWindow != null)
                {
                    messageWindow.SetWindow(winSprite, "You Win", "Okay", () =>
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
                if (messageWindow != null)
                {
                    messageWindow.SetWindow(looseSprite, "You loose", "Okay", () =>
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
            if (initialScreenFader != null)
            {
                initialScreenFader.FadeIn();
            }
            yield return null;
        }

        private IEnumerator WaitForBoardRoutine(float delay)
        {
            if (TimeLevelGoal != null)
            {
                if (TimeLevelGoal.LevelTimeUi != null)
                {
                    TimeLevelGoal.LevelTimeUi.IsPaused = true;
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
            if (TimeLevelGoal == null)
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
                if (movesCounterText != null)
                {
                    movesCounterText.text = "\u221E";
                    movesCounterText.fontSize = 70;
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
                    if (scoreMeter != null)
                    {
                        scoreMeter.UpdateScoreMeter(scoreManager.GetCurrentScore(), levelGoal.ScoreStars);
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
            if(TimeLevelGoal != null)
            {
                TimeLevelGoal.Addtime(value);
            }
        }

        public void CheckForCollectionGoals(GamePiece gamePiece)
        {
            if(levelGoalCollected != null)
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
                    if (scoreMeter != null)
                    {
                        scoreMeter.SetLevelGoal(levelGoal);
                    }
                    if(TimeLevelGoal != null)
                    {
                        if (movesCounterText != null)
                        {
                            movesCounterText.text = "\u221E";
                            movesCounterText.fontSize = 70;
                        }
                    }
                    StartCoroutine(ExecuteGameLoop());
                }
            }
        }
        #endregion
    }
}
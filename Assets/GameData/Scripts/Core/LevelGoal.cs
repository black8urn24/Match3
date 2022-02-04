using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Utilities;

namespace Match3.Core
{
    public abstract class LevelGoal : Singleton<LevelGoal>
    {
        #region Inspector Variables
        [SerializeField] private int scoreStars = 0;
        [SerializeField] private int[] scoreGoals = new int[3];
        [SerializeField] private int movesForLevel = 30;
        [SerializeField] private int timeLeft = 60;
        #endregion

        #region Variables
        private int targetScore = 0;
        private int currentMoves = 0;
        #endregion

        #region Properties
        public int ScoreStars { get => scoreStars; set => scoreStars = value; }
        public int[] ScoreGoals { get => scoreGoals; set => scoreGoals = value; }
        public int MovesForLevel { get => movesForLevel; set => movesForLevel = value; }
        public int TargetScore { get => targetScore; set => targetScore = value; }
        public int MovesLeft { get => currentMoves; set => currentMoves = value; }
        public int TimeLeft { get => timeLeft; set => timeLeft = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }
        #endregion

        #region Private Methods
        private void Init()
        {
            ScoreStars = 0;
            for (int i = 1; i < ScoreGoals.Length; i++)
            {
                if (ScoreGoals[i] < ScoreGoals[i - 1])
                {
                    Debug.Log($"LEVELGOAL : score goals are in increasing order".ToRed().ToBold());
                }
            }
        }

        private int UpdateScore(int score)
        {
            for (int i = 0; i < ScoreGoals.Length; i++)
            {
                if (score < ScoreGoals[i])
                {
                    return i;
                }
            }
            return ScoreGoals.Length;
        }
        #endregion

        #region Abstract Declerations
        public abstract bool IsWinner();
        public abstract bool IsGameOver();
        #endregion

        #region Public Methods
        public void UpdateScoreStars(int score)
        {
            ScoreStars = UpdateScore(score);
        }
        #endregion
    }
}
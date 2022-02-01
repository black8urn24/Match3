using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Utilities;

namespace Match3.Core
{
    public class LevelGoal : Singleton<LevelGoal>
    {
        #region Inspector Variables
        [SerializeField] private int scoreStars = 0;
        [SerializeField] private int[] scoreGoals = new int[3];
        [SerializeField] private int movesForLevel = 30;
        #endregion

        #region Variables
        public int ScoreStars { get => scoreStars; set => scoreStars = value; }
        public int[] ScoreGoals { get => scoreGoals; set => scoreGoals = value; }
        public int MovesForLevel { get => movesForLevel; set => movesForLevel = value; }
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
        public void Init()
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

        public int UpdateScore(int score)
        {
            for(int i = 0; i < ScoreGoals.Length; i++)
            {
                if(score < ScoreGoals[i])
                {
                    return i;
                }
            }
            return ScoreGoals.Length;
        }

        public void UpdateScoreStars(int score)
        {
            ScoreStars = UpdateScore(score);
        }
        #endregion
    }
}
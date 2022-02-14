using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Enums;

namespace Match3.Core
{
    public class ScoreLevelGoal : LevelGoal
    {
        #region Unity Methods
        public override void Start()
        {
            base.Start();
            LevelCounterType = LevelCounterType.Moves;
        }
        #endregion

        #region Abstract Implementation
        public override bool IsGameOver()
        {
            int maxScore = ScoreGoals[ScoreGoals.Length - 1];
            if (ScoreManager.Instance != null)
            {
                return ScoreManager.Instance.GetCurrentScore() >= maxScore;
            }
            return MovesLeft <= 0;
        }

        public override bool IsWinner()
        {
            if (ScoreManager.Instance != null)
            {
                return ScoreManager.Instance.GetCurrentScore() >= TargetScore;
            }
            return false;
        }
        #endregion
    }
}
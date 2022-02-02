using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class ScoreLevelGoal : LevelGoal
    {
        #region Abstract Implementation
        public override bool IsGameOver()
        {
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
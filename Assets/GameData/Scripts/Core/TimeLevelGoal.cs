using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class TimeLevelGoal : LevelGoal
    {
        #region Inspector Variables
        #endregion

        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Methods
        public override void Start()
        {
            base.Start();
            LevelCounterType = Enums.LevelCounterType.Timer;
            if (UiManager.Instance != null && UiManager.Instance.LevelTimer != null)
            {
                UiManager.Instance.LevelTimer.InitTimer(TimeLeft);
            }
        }
        #endregion

        #region Abstract Implementation
        public override bool IsWinner()
        {
            if (ScoreManager.Instance != null)
            {
                return (ScoreManager.Instance.GetCurrentScore() >= TargetScore);
            }
            return false;
        }

        public override bool IsGameOver()
        {
            int maxScore = ScoreGoals[ScoreGoals.Length - 1];
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.GetCurrentScore() >= maxScore)
                {
                    return true;
                }
            }
            return (TimeLeft <= 0);
        }
        #endregion
    }
}
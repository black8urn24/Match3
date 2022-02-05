using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class TimeLevelGoal : LevelGoal
    {
        #region Inspector Variables
        [SerializeField] private LevelTimeUiManager levelTimeUi = null;
        #endregion

        #region Unity Methods
        private void Start()
        {
            if(levelTimeUi != null)
            {
                levelTimeUi.InitTimer(TimeLeft);
            }
        }
        #endregion

        #region Public Methods
        public void StartCountdown()
        {
            StartCoroutine(CountdownRoutine());
        }
        #endregion

        #region Coroutines
        private IEnumerator CountdownRoutine()
        {
            while (TimeLeft > 0)
            {
                yield return new WaitForSeconds(1f);
                TimeLeft--;
                if(levelTimeUi != null)
                {
                    levelTimeUi.UpdateTimer(TimeLeft);
                }
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
                return ScoreManager.Instance.GetCurrentScore() >= maxScore;
            }
            return (TimeLeft <= 0);
        }
        #endregion
    }
}
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

        #region Properties
        public LevelTimeUiManager LevelTimeUi { get => levelTimeUi; set => levelTimeUi = value; }
        #endregion

        #region Unity Methods
        private void Start()
        {
            if (LevelTimeUi != null)
            {
                LevelTimeUi.InitTimer(TimeLeft);
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
                if (LevelTimeUi != null)
                {
                    LevelTimeUi.UpdateTimer(TimeLeft);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Match3.Core
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI scoreText = null;
        #endregion

        #region Variables
        private int currentScore = 0;
        private int counterValue = 0;
        private int increment = 5;
        #endregion

        #region Properties
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            UpdateScoreText(currentScore);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Coroutines
        private IEnumerator UpdateScoreRoutine()
        {
            int iterations = 0;
            while(counterValue < currentScore && iterations < 1000)
            {
                counterValue += increment;
                UpdateScoreText(counterValue);
                iterations++;
                yield return null;
            }
            counterValue = currentScore;
            UpdateScoreText(currentScore);
        }
        #endregion

        #region Public Methods
        public void UpdateScoreText(int score)
        {
            if(scoreText != null)
            {
                scoreText.text = score.ToString();
            }
        }

        public void AddScore(int value)
        {
            currentScore += value;
            StartCoroutine(UpdateScoreRoutine());
        }

        public int GetCurrentScore()
        {
            return currentScore;
        }
        #endregion
    }
}
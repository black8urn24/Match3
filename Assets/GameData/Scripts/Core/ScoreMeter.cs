using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Core
{
    public class ScoreMeter : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Image scoreFillImage = null;
        [SerializeField] private ScoreStar[] scoreStars = null;
        [SerializeField] private RectTransform starsParentTransform = null;
        #endregion

        #region Variables
        private LevelGoal levelGoal = null;
        private int maxScore = 0;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        public void SetLevelGoal(LevelGoal goal)
        {
            if (goal != null)
            {
                levelGoal = goal;
            }
            maxScore = levelGoal.ScoreGoals[levelGoal.ScoreGoals.Length - 1];
            float maxWidth = starsParentTransform.rect.width;
            if(maxScore > 0)
            {
                for(int i = 0; i < levelGoal.ScoreGoals.Length; i++)
                {
                    if(scoreStars != null)
                    {
                        float newX = (maxWidth * levelGoal.ScoreGoals[i] / maxScore) - (maxWidth * 0.5f);
                        RectTransform starRectTransform = scoreStars[i].GetComponent<RectTransform>();
                        if(starRectTransform != null)
                        {
                            starRectTransform.anchoredPosition = new Vector2(newX, starRectTransform.anchoredPosition.y);
                        }
                    }
                }
            }
        }

        public void UpdateScoreMeter(int score, int starCount)
        {
            if(levelGoal != null)
            {
                scoreFillImage.fillAmount = (float)score / (float)maxScore;
            }
            for(int i = 0; i < starCount; i++)
            {
                if(scoreStars[i] != null)
                {
                    scoreStars[i].Activate();
                }
            }
        }
        #endregion
    }
}
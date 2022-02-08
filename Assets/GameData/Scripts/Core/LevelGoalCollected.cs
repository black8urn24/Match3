using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class LevelGoalCollected : LevelGoal
    {
        #region Inspector Variables
        [SerializeField] private List<CollectionGoal> collectionGoals = null;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Private Methods
        private bool AreGoalsCompleted(List<CollectionGoal> collectionGoals)
        {
            if (collectionGoals == null || collectionGoals.Count <= 0)
            {
                return false;
            }
            foreach (var item in collectionGoals)
            {
                if (item.GetCurrentItemCount() != 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Public Methods
        public void UpdateGoals(GamePiece gamePiece)
        {
            if (gamePiece != null)
            {
                foreach (var item in collectionGoals)
                {
                    if (item != null)
                    {
                        item.CollectPiece(gamePiece);
                    }
                }
            }
        }
        #endregion

        #region Abstract Implementation
        public override bool IsGameOver()
        {
            if(AreGoalsCompleted(collectionGoals) && ScoreManager.Instance != null)
            {
                int maxScore = ScoreGoals[ScoreGoals.Length - 1];
                if(ScoreManager.Instance.GetCurrentScore() >= maxScore)
                {
                    return true;
                }
            }
            return MovesLeft <= 0;
        }

        public override bool IsWinner()
        {
            if (ScoreManager.Instance != null)
            {
                return (ScoreManager.Instance.GetCurrentScore() >= ScoreGoals[0] && AreGoalsCompleted(collectionGoals));
            }
            return false;
        }
        #endregion
    }
}
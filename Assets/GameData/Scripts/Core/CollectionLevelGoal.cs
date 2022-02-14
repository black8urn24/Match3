using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class CollectionLevelGoal : LevelGoal
    {
        #region Inspector Variables
        [SerializeField] private List<CollectionGoal> collectionGoals = null;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
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
            UpdateUI();
        }

        public void UpdateUI()
        {
            if(UiManager.Instance != null)
            {
                UiManager.Instance.UpdateCollectionGoalLayout();
            }
        }

        public List<CollectionGoal> GetCollectionGoals()
        {
            return collectionGoals;
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
            return LevelCounterType == Enums.LevelCounterType.Moves ? MovesLeft <= 0 : TimeLeft <= 0;
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
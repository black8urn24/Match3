using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class LevelGoalCollected : LevelGoal
    {
        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Abstract Implementation
        public override bool IsGameOver()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsWinner()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    [RequireComponent(typeof(GamePiece))]
    public class TimeBonus : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private int bonusValue = 2;
        #endregion

        #region Properties
        public int BonusValue { get => bonusValue; private set => bonusValue = value; }
        #endregion
    }
}
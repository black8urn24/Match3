using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    [RequireComponent(typeof(GamePiece))]
    public class TimeBonus : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] [Range(1f, 5f)] private int bonusValue = 2;
        [SerializeField] [Range(0f, 1f)] private float chanceForBonus = 0.1f;
        [SerializeField] private GameObject bonusGlow = null;
        [SerializeField] private GameObject ringGlow = null;
        #endregion

        #region Properties
        public int BonusValue { get => bonusValue; private set => bonusValue = value; }
        #endregion

        #region Unity Methods
        private void Start()
        {
            float random = Random.Range(0f, 1f);
            if(random > chanceForBonus)
            {
                BonusValue = 0;
            }
            SetState(BonusValue != 0);
        }
        #endregion

        #region Private Methods
        private void SetState(bool state)
        {
            if(bonusGlow != null)
            {
                bonusGlow.SetActive(state);
            }
            if(ringGlow != null)
            {
                ringGlow.SetActive(state);
            }
        }
        #endregion
    }
}
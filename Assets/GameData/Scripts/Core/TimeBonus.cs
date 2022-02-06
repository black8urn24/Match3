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
        [SerializeField] private List<Material> bonusMaterials = new List<Material>();
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
            if(GameManager.Instance != null)
            {
                if(GameManager.Instance.TimeLevelGoal == null)
                {
                    BonusValue = 0;
                }
            }
            SetState(BonusValue != 0);
            if(bonusValue != 0)
            {
                SetMaterial(bonusValue - 1, bonusGlow);
            }
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

        private void SetMaterial(int index, GameObject bonusGlow)
        {
            int targetIndex = Mathf.Clamp(index, 0, bonusMaterials.Count - 1);
            if(bonusGlow != null && bonusMaterials[targetIndex] != null)
            {
                ParticleSystemRenderer renderer = bonusGlow.GetComponent<ParticleSystemRenderer>();
                if(renderer != null)
                {
                    renderer.material = bonusMaterials[targetIndex];
                }
            }
        }
        #endregion
    }
}
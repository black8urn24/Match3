using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Core
{
    public class ScoreStar : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Image starIcon = null;
        [SerializeField] private AudioClip starClip = null;
        #endregion

        #region Variables
        private bool activated = false;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            SetStarState(false);
        }
        #endregion

        #region Private Methods
        private void SetStarState(bool state)
        {
            if (starIcon != null)
            {
                starIcon.gameObject.SetActive(state);
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator ActivateRoutine()
        {
            activated = true;
            yield return null;
            if(AudioManager.Instance != null && starClip != null)
            {
                AudioManager.Instance.PlaySingleClip(starClip, Enums.PoolObjectsType.ScoreStarAudioSource, false);
            }
            SetStarState(activated);
        }
        #endregion

        #region Public Methods
        public void Activate()
        {
            if(activated)
            {
                return;
            }
            StartCoroutine(ActivateRoutine());
        }
        #endregion
    }
}
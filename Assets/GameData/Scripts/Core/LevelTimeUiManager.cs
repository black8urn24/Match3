using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Match3.Core
{
    public class LevelTimeUiManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI timerText = null;
        [SerializeField] private Image clockImage = null;
        #endregion

        #region Variables
        private int maxTime = 60;
        private bool isPaused = false;
        #endregion

        #region Properties
        public bool IsPaused { get => isPaused; set => isPaused = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Private Methods
        private void SetTimerText(int timer)
        {
            if(timerText != null)
            {
                timerText.text = timer.ToString();
            }
        }
        #endregion

        #region Public Methods
        public void InitTimer(int time = 60)
        {
            maxTime = time;
            if(clockImage != null)
            {
                clockImage.type = Image.Type.Filled;
                clockImage.fillMethod = Image.FillMethod.Radial360;
                clockImage.fillOrigin = (int)Image.Origin360.Top;
            }
            SetTimerText(maxTime);
        }

        public void UpdateTimer(int currentTime)
        {
            if (IsPaused)
            {
                return;
            }
            if(clockImage != null)
            {
                clockImage.fillAmount = (float)currentTime / (float)maxTime;
            }
            SetTimerText(currentTime);
        }
        #endregion
    }
}
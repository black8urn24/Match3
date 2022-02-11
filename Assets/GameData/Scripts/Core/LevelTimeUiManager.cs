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
        [SerializeField] private int flashtimeLimit = 10;
        [SerializeField] private float flashInterval = 1f;
        [SerializeField] private AudioClip beepClip = null;
        [SerializeField] private Color flashColor = Color.red;
        #endregion

        #region Variables
        private int maxTime = 60;
        private bool isPaused = false;
        private Coroutine flashCoroutine = null;
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

        #region Coroutines
        private IEnumerator FlashRoutine(Image targetImage, Color targetColor, float interval)
        {
            if(targetImage != null)
            {
                Color orginalColor = targetImage.color;
                targetImage.CrossFadeColor(targetColor, flashInterval * 0.3f, true, true);
                yield return new WaitForSeconds(flashInterval * 0.2f);
                targetImage.CrossFadeColor(orginalColor, flashInterval * 0.3f, true, true);
                yield return new WaitForSeconds(flashInterval * 0.2f);
            }
        }

        private IEnumerator FlashRoutine(TextMeshProUGUI targetText, Color targetColor, float interval = 1f)
        {
            if(targetText != null)
            {
                Color orginalColor = targetText.color;
                targetText.color = targetColor;
                yield return new WaitForSeconds(interval * 0.5f);
                targetText.color = orginalColor;
                yield return new WaitForSeconds(interval * 0.5f);
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
            #region For Clock Image
            //if(clockImage != null)
            //{
            //    clockImage.fillAmount = (float)currentTime / (float)maxTime;
            //    if(currentTime <= flashtimeLimit)
            //    {
            //        flashCoroutine = StartCoroutine(FlashRoutine(clockImage, flashColor, flashInterval));
            //        if(AudioManager.Instance != null)
            //        {
            //            AudioManager.Instance.PlaySingleClip(beepClip, Enums.PoolObjectsType.ClockTickAudioSource, false);
            //        }
            //    }
            //}
            #endregion
            #region For Timer Text
            if (currentTime <= flashtimeLimit)
            {
                flashCoroutine = StartCoroutine(FlashRoutine(timerText, flashColor, flashInterval));
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySingleClip(beepClip, Enums.PoolObjectsType.ClockTickAudioSource, false);
                }
            }
            #endregion
            SetTimerText(currentTime);
        }
        #endregion
    }
}
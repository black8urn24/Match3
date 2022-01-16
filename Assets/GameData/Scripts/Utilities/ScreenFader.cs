using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Utilities
{
    public class ScreenFader : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Image faderImage = null;
        #endregion

        #region Variables
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            FadeOut();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Coroutines
        private IEnumerator FadeRoutine(float initialValue, float finalValue, float animationDuration = 1f)
        {
            float animationTime = 0f;
            Color currentColor = faderImage.color;
            Color initialColor = new Color(currentColor.r, currentColor.g, currentColor.b, initialValue);
            Color finalColor = new Color(currentColor.r, currentColor.g, currentColor.b, finalValue);
            while (animationTime < animationDuration)
            {
                animationTime += Time.deltaTime;
                float lerpValue = animationTime / animationDuration;
                faderImage.color = Color.Lerp(initialColor, finalColor, lerpValue);
                yield return null;
            }
        }
        #endregion

        #region Public Methods
        public void FadeIn()
        {
            StartCoroutine(FadeRoutine(0f, 1f, 1.5f));
        }

        public void FadeOut()
        {
            StartCoroutine(FadeRoutine(1f, 0f, 1.5f));
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Match3.Utilities
{
    public class MessageWindowManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GameObject panelRoot = null;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 onScreenPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] [Range(0f, 1f)] private float animationDuration = 1f;
        [SerializeField] private TextMeshProUGUI messageText = null;
        [SerializeField] private Text buttonText = null;
        #endregion

        #region Variables
        private bool isMoving = false;
        private RectTransform panelRectTransform = null;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            panelRectTransform = panelRoot.GetComponent<RectTransform>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Private Methods
        private void ShowWindow()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }
        }

        private void HideWindow()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        private void MoveWindow(Vector3 startPosition, Vector3 endposition, float animationDuration = 1f)
        {
            if(isMoving == false)
            {
                StartCoroutine(MoveWindowRoutine(startPosition, endposition, animationDuration));
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator MoveWindowRoutine(Vector3 startPosition, Vector3 endPosition, float animationDuration = 1f)
        {
            isMoving = true;
            float animationTime = 0f;
            while (animationTime < animationDuration)
            {
                animationTime += Time.deltaTime;
                float lerpTime = animationTime / animationDuration;
                lerpTime = lerpTime * lerpTime * lerpTime * (lerpTime * (lerpTime * 6 - 15) + 10);      // smoother step
                panelRectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, lerpTime);
                yield return null;
            }
            isMoving = false;
        }
        #endregion

        #region Public Methods
        public void MoveIn()
        {
            MoveWindow(startPosition, onScreenPosition, animationDuration);
        }

        public void MoveOut()
        {
            MoveWindow(onScreenPosition, endPosition, animationDuration);
        }

        public void SetMessageDetails(string messageString, string buttonString)
        {
            if(messageString != string.Empty)
            {
                messageText.text = messageString;
            }
            if(buttonString != string.Empty)
            {
                buttonText.text = buttonString;
            }
        }
        #endregion
    }
}
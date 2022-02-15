using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
        [SerializeField] private Image messageIconImage = null;
        [SerializeField] private Button clickButton = null;
        [SerializeField] private Sprite goalSprite = null;
        [SerializeField] private Sprite winSprite = null;
        [SerializeField] private Sprite looseSprite = null;
        #endregion

        #region Variables
        private bool isMoving = false;
        private RectTransform panelRectTransform = null;
        #endregion

        #region Events
        private event Action ShowMessageWindow;
        private event Action HideMessageWindow;
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

        private void OnEnable()
        {
            ShowMessageWindow += ShowWindow;
            HideMessageWindow += HideWindow;
        }

        private void OnDisable()
        {
            ShowMessageWindow -= ShowWindow;
            HideMessageWindow -= HideWindow;
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

        private void MoveWindow(Vector3 startPosition, Vector3 endposition, float animationDuration = 1f, bool movingIn = true)
        {
            if(isMoving == false)
            {
                StartCoroutine(MoveWindowRoutine(startPosition, endposition, animationDuration, movingIn));
            }
        }

        private void MoveIn()
        {
            MoveWindow(startPosition, onScreenPosition, animationDuration, true);
        }

        private void MoveOut()
        {
            MoveWindow(onScreenPosition, endPosition, animationDuration, false);
        }

        private void SetMessageDetails(Sprite iconSprite, string messageString, string buttonString, Action buttonClickAction = null)
        {
            if (iconSprite != null)
            {
                messageIconImage.sprite = iconSprite;
            }
            if (messageString != string.Empty)
            {
                messageText.text = messageString;
            }
            if (buttonString != string.Empty)
            {
                buttonText.text = buttonString;
            }
            if (buttonClickAction != null)
            {
                clickButton.onClick.RemoveAllListeners();
                clickButton.onClick.AddListener(() =>
                {
                    buttonClickAction();
                    MoveOut();
                });
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator MoveWindowRoutine(Vector3 startPosition, Vector3 endPosition, float animationDuration = 1f, bool movingIn = true)
        {
            isMoving = true;
            if(movingIn == true)
            {
                ShowMessageWindow?.Invoke();
            }
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
            if (movingIn == false)
            {
                HideMessageWindow?.Invoke();
            }
        }

        #endregion

        #region Public Methods
        public void SetTargetMessageWindow(int targetScore, Action buttonAction)
        {
            SetMessageDetails(goalSprite, "Goal : " + targetScore.ToString(), "Start", buttonAction);
            MoveIn();
        }

        public void SetWinMessageWindow(Action buttonAction)
        {
            SetMessageDetails(winSprite, "You Win", "Okay", buttonAction);
            MoveIn();
        }

        public void SetLooseMessageWindow(Action buttonAction)
        {
            SetMessageDetails(looseSprite, "You loose", "Okay", buttonAction);
            MoveIn();
        }
        #endregion
    }
}
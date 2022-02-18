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
        [SerializeField] private Sprite collectionSprite = null;
        [SerializeField] private Sprite timerSprite = null;
        [SerializeField] private Sprite movesSprite = null;
        [SerializeField] private Image targetImage = null;
        [SerializeField] private TextMeshProUGUI targetText = null;
        [SerializeField] private GameObject collectionGoalLayout = null;
        [SerializeField] private Sprite goalCompletionSprite = null;
        [SerializeField] private Sprite goalFailSprite = null;
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
            if (isMoving == false)
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

        private void SetGoalDetails(string caption = "", Sprite icon = null)
        {
            if (targetText != null && caption != "")
            {
                SetGoalDescription(caption);
            }
            if (targetImage != null && icon != null)
            {
                SetGoalImage(icon);
            }
        }

        
        #endregion

        #region Coroutines
        private IEnumerator MoveWindowRoutine(Vector3 startPosition, Vector3 endPosition, float animationDuration = 1f, bool movingIn = true)
        {
            isMoving = true;
            if (movingIn == true)
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

        public void SetTimerGoal(int timer)
        {
            SetGoalDetails(timer.ToString() + " Seconds", timerSprite);
        }

        public void SetMovesGoal(int moves)
        {
            SetGoalDetails(moves.ToString() + " Moves", movesSprite);
        }

        public void SetCollectionGoal(bool state = true)
        {
            if (collectionGoalLayout != null)
            {
                collectionGoalLayout.SetActive(state);
            }
            if (state)
            {
                SetGoalDetails("", collectionSprite);
            }
        }

        public GameObject GetColletionGoalLayout()
        {
            return collectionGoalLayout;
        }
        public void SetGoalDescription(string description = "", int xOffset = 0, int yOffset = 0)
        {
            if(targetText != null && description != "")
            {
                targetText.text = description;
                RectTransform transform = targetText.GetComponent<RectTransform>();
                if (transform != null)
                {
                    transform.anchoredPosition += new Vector2(xOffset, yOffset);
                }
            }
        }

        public void SetGoalImage(Sprite sprite = null)
        {
            if (sprite != null)
            {
                targetImage.sprite = sprite;
                targetImage.gameObject.SetActive(true);
            }
            else
            {
                targetImage.gameObject.SetActive(false);
            }
        }

        public Sprite GetGoalWinSprite()
        {
            return goalCompletionSprite;
        }

        public Sprite GetGoalFailSprite()
        {
            return goalFailSprite;
        }
        #endregion
    }
}
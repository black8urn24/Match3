using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

namespace Match3.Core
{
    public class Booster : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private string description = string.Empty;
        [SerializeField] private bool isEnabled = false;
        [SerializeField] private bool isDraggable = true;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private List<CanvasGroup> canvasGroups = null;
        [SerializeField] private UnityEvent boostEvent = null;
        [SerializeField] private int timeBonus = 15;
        [SerializeField] private Button boosterButton = null;
        [SerializeField] private Image boosterImage = null;
        #endregion

        #region Variables
        private RectTransform rectTransform = null;
        private Vector3 startPosition = Vector3.zero;
        private GameBoard gameBoard = null;
        private GameTile targetTile = null;
        private Camera mainCamera = null;
        public static GameObject activeGameBooster = null;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            SetInitialReferences();
        }
        #endregion

        #region Private Methods
        private void SetInitialReferences()
        {
            gameBoard = GameManager.Instance.GetGameBoard();
            rectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
            EnableBooster(false);
            if(boosterButton != null)
            {
                boosterButton.onClick.RemoveAllListeners();
                boosterButton.onClick.AddListener(() => 
                {
                    EnableBooster(!isEnabled);
                });
            }
        }
        #endregion

        #region Public Methods
        public void EnableBooster(bool state)
        {
            isEnabled = state;
            if(state)
            {
                Booster.activeGameBooster = gameObject;
            }
            else if(gameObject == Booster.activeGameBooster)
            {
                Booster.activeGameBooster = null;
            }
            boosterImage.color = state ? Color.white : Color.gray;
            if(descriptionText != null)
            {
                descriptionText.gameObject.SetActive(Booster.activeGameBooster != null);
                if(gameObject == Booster.activeGameBooster)
                {
                    descriptionText.text = description;
                }
            }
        }
        #endregion

        #region Interface Implementation
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(isEnabled && isDraggable && !isLocked)
            {
                startPosition = gameObject.transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(isEnabled && isDraggable && !isLocked && mainCamera != null)
            {
                Vector3 onScreenPosition;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, mainCamera, out onScreenPosition);
                boosterImage.transform.position = onScreenPosition;
                RaycastHit2D hit2D = Physics2D.Raycast(onScreenPosition, Vector3.forward, Mathf.Infinity);
                if (hit2D.collider != null)
                {
                    GameTile tile = hit2D.collider.GetComponent<GameTile>();
                    if (tile != null)
                    {
                        targetTile = tile;
                    }
                    else
                    {
                        targetTile = null;
                    }
                }
                else
                {
                    targetTile = null;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(isEnabled && isDraggable && !isLocked)
            {
                boosterImage.transform.position = startPosition;
                if(gameBoard != null && gameBoard.IsRefilling)
                {
                    return;
                }
                if(targetTile != null)
                {
                    if(boostEvent != null)
                    {
                        boostEvent.Invoke();
                    }
                    EnableBooster(false);
                }
            }
        }
        #endregion
    }
}
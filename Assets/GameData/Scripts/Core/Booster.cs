using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

namespace Match3.Core
{
    public class Booster : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private string description = string.Empty;
        [SerializeField] private bool isEnabled = false;
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

        private void Update()
        {
            HandleBoosterSelection();
        }
        #endregion

        #region Private Methods
        private void SetInitialReferences()
        {
            gameBoard = GameManager.Instance.GetGameBoard();
            rectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
            EnableBooster(false);
            if (boosterButton != null)
            {
                boosterButton.onClick.RemoveAllListeners();
                boosterButton.onClick.AddListener(() =>
                {
                    EnableBooster(!isEnabled);
                });
            }
        }

        private void HandleBoosterSelection()
        {
            if (isEnabled)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit2D hit2D = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, Mathf.Infinity);
                    if (hit2D.collider != null)
                    {
                        GameTile tile = hit2D.collider.GetComponent<GameTile>();
                        if (tile != null)
                        {
                            targetTile = tile;
                            Debug.Log($"Target tile - {targetTile.TileType}");
                        }
                        else
                        {
                            targetTile = null;
                            Debug.Log($"Target tile - NULL");
                        }
                    }
                    else
                    {
                        targetTile = null;
                        Debug.Log($"Target tile - NULL");
                    }
                    if (gameBoard != null && gameBoard.IsRefilling)
                    {
                        return;
                    }
                    if (targetTile != null)
                    {
                        if (boostEvent != null)
                        {
                            boostEvent.Invoke();
                        }
                        EnableBooster(false);
                        targetTile = null;
                        Booster.activeGameBooster = null;
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void EnableBooster(bool state)
        {
            isEnabled = state;
            if(GameManager.Instance != null)
            {
                GameManager.Instance.HandleBoosterBg(state);
            }
            if (state)
            {
                Booster.activeGameBooster = gameObject;
            }
            else if (gameObject == Booster.activeGameBooster)
            {
                Booster.activeGameBooster = null;
            }
            boosterImage.color = state ? Color.white : Color.gray;
            if (descriptionText != null)
            {
                descriptionText.gameObject.SetActive(Booster.activeGameBooster != null);
                if (gameObject == Booster.activeGameBooster)
                {
                    descriptionText.text = description;
                }
            }
        }

        public void RemoveSelectedTile()
        {
            if(gameBoard != null && targetTile != null)
            {
                gameBoard.ClearAndRefillBoard(targetTile.XIndex, targetTile.YIndex);
            }
        }

        public void AddTimer()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.Addtime(timeBonus);
            }
        }

        public void MakeColorBomb()
        {
            if(gameBoard != null && targetTile != null)
            {
                gameBoard.MakeColorBomb(targetTile.XIndex, targetTile.YIndex);
            }
        }
        #endregion
    }
}
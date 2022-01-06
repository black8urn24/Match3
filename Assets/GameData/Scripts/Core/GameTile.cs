using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Utilities;
using Match3.Enums;

namespace Match3.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GameTile : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TileType tileType = TileType.Normal;
        [SerializeField] [Range(0,2)] private int maxBreakableValue = 0;
        [SerializeField] private Sprite[] breakableSprites = null;
        [SerializeField] private Color normalTileColor = Color.black;
        #endregion

        #region Variables
        private int xIndex = -1;
        private int yIndex = -1;
        private GameBoard currentGameBoard = null;
        private SpriteRenderer spriteRenderer = null;
        private int currentBreakableValue = -1;
        #endregion

        #region Properties
        public int XIndex { get => xIndex; set => xIndex = value; }
        public int YIndex { get => yIndex; set => yIndex = value; }
        public TileType TileType { get => tileType; set => tileType = value; }
        public int CurrentBreakableValue { get => currentBreakableValue; set => currentBreakableValue = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            CurrentBreakableValue = maxBreakableValue;
        }

        private void OnMouseDown()
        {
            if(currentGameBoard != null)
            {
                currentGameBoard.SetClickedTile(this);
            }
        }

        private void OnMouseEnter()
        {
            if(currentGameBoard != null)
            {
                currentGameBoard.SetTargetTile(this);
            }
        }

        private void OnMouseUp()
        {
            if(currentGameBoard != null)
            {
                currentGameBoard.ReleaseTile();
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator BreakTileRoutine()
        {
            CurrentBreakableValue--;
            CurrentBreakableValue = Mathf.Clamp(CurrentBreakableValue, 0, maxBreakableValue);
            yield return new WaitForSeconds(0.15f);
            if(breakableSprites[CurrentBreakableValue] != null)
            {
                spriteRenderer.sprite = breakableSprites[CurrentBreakableValue];
            }
            if(CurrentBreakableValue <= 0)
            {
                TileType = TileType.Normal;
                spriteRenderer.color = normalTileColor;
            }
        }
        #endregion

        #region Public Methods
        public void InitializeTile(int x, int y, GameBoard gameBoard)
        {
            XIndex = x;
            YIndex = y;
            currentGameBoard = gameBoard;
            if(TileType == TileType.Breakable)
            {
                if (breakableSprites[CurrentBreakableValue] != null)
                {
                    spriteRenderer.sprite = breakableSprites[CurrentBreakableValue];
                }
            }
            //Debug.Log($"Tile Initialized with details - {xIndex} and {yIndex}".ToOrange().ToBold());
        }

        public void BreakTile()
        {
            if(TileType != TileType.Breakable)
            {
                return;
            }
            StartCoroutine(BreakTileRoutine());
        }
        #endregion
    }
}
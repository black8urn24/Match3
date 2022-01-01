using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

namespace Match3.Core
{
    public class GameBoard : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GameObject gameTilePrefab = null;
        [SerializeField] private int boardWidth = -1;
        [SerializeField] private int boardHeight = -1;
        [SerializeField] private float borderSize = -1f;
        [SerializeField] private Transform gamePieceParent = null;
        [SerializeField] private GameObject[] gamePiecePrefabs = null;
        [SerializeField] private float gamePieceMoveSpeed = 0.3f;
        [SerializeField] private Button reloadLevelButton = null;
        #endregion

        #region Variables
        private GameTile[,] allTiles;
        private GamePiece[,] allPieces;
        private GameTile clickedTile;
        private GameTile targetTile;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            SetInitialReferences();
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Private Methods
        private void SetInitialReferences()
        {
            allTiles = new GameTile[boardWidth, boardHeight];
            allPieces = new GamePiece[boardWidth, boardHeight];
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    GameObject tile = Instantiate(gameTilePrefab, transform);
                    tile.transform.position = new Vector3(i, j, 0);
                    tile.transform.rotation = Quaternion.identity;
                    tile.transform.name = "Tile[" + i + "," + j + "]";
                    GameTile gameTile = tile.GetComponent<GameTile>();
                    if (gameTile != null)
                    {
                        allTiles[i, j] = gameTile;
                        gameTile.InitializeTile(i, j, this);
                    }
                }
            }
            SetCameraDimensions();
            FillBoard();
            //HighlightMatches();
            if(reloadLevelButton != null)
            {
                reloadLevelButton.onClick.RemoveAllListeners();
                reloadLevelButton.onClick.AddListener(() => 
                {
                    SceneManager.LoadScene(0);
                });
            }
        }

        private void SetCameraDimensions()
        {
            Camera mainCamera = Camera.main;
            Vector3 finalCameraPosition = new Vector3((float)(boardWidth - 1) / 2f, (float)(boardHeight - 1) / 2f, -10f);
            mainCamera.transform.position = finalCameraPosition;
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            float verticalSize = (float)boardHeight / 2f + borderSize;
            float horizontalSize = ((float)boardWidth / 2f + borderSize) / aspectRatio;
            mainCamera.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
        }

        private GameObject GetRandomPiece()
        {
            int randomIndex = Random.Range(0, gamePiecePrefabs.Length);
            if (gamePiecePrefabs[randomIndex] == null)
            {
                Debug.Log($"GAMEBOARD - gamepiece is null at index - {randomIndex}".ToRed().ToBold());
                return null;
            }
            return gamePiecePrefabs[randomIndex];
        }

        private void FillBoard()
        {
            int maxIterations = 100;
            int currentIterations = 0;
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    GamePiece randomPiece = FillBoardAt(i, j);
                    while(HasMatchOnFill(i,j,3))
                    {
                        ClearPieceAt(i, j);
                        randomPiece = FillBoardAt(i, j);
                        currentIterations++;
                        if(currentIterations >= maxIterations)
                        {
                            Debug.Log($"BOARD::Max iterations made".ToRed().ToBold());
                            break;
                        }
                    }
                }
            }
        }

        private GamePiece FillBoardAt(int i, int j)
        {
            GameObject gamePiece = Instantiate(GetRandomPiece(), Vector3.zero, Quaternion.identity);
            if (gamePiece != null)
            {
                gamePiece.transform.SetParent(gamePieceParent);
                gamePiece.GetComponent<GamePiece>().Initialize(this);
                PlaceGamePiece(gamePiece.GetComponent<GamePiece>(), i, j);
                return gamePiece.GetComponent<GamePiece>();
            }
            return null;
        }

        private bool HasMatchOnFill(int i, int j, int minLength = 3)
        {
            List<GamePiece> leftMatches = FindMatches(i, j, Vector2.left, minLength);
            List<GamePiece> downMatches = FindMatches(i, j, Vector2.down, minLength);
            if(leftMatches == null)
            {
                leftMatches = new List<GamePiece>();
            }
            if(downMatches == null)
            {
                downMatches = new List<GamePiece>();
            }
            return (leftMatches.Count > 0 || downMatches.Count > 0);
        }

        private void SwitchTiles(GameTile currentTile, GameTile targetTile)
        {
            StartCoroutine(SwitchTilesRoutine(currentTile, targetTile));
        }

        private bool IsWithinBounds(int x, int y)
        {
            return (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight);
        }

        private bool IsNextTo(GameTile start, GameTile end)
        {
            if(Mathf.Abs(start.XIndex - end.XIndex) == 1 && start.YIndex == end.YIndex)
            {
                return true;
            }
            if (Mathf.Abs(start.YIndex - end.YIndex) == 1 && start.XIndex == end.XIndex)
            {
                return true;
            }
            return false;
        }

        private List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
        {
            List<GamePiece> matches = new List<GamePiece>();
            GamePiece startPiece = null;
            if(IsWithinBounds(startX, startY))
            {
                startPiece = allPieces[startX, startY];
            }
            if(startPiece != null)
            {
                matches.Add(startPiece);
            }
            else
            {
                return null;
            }
            int nextX;
            int nextY;
            int maxValue = (boardWidth > boardHeight) ? boardWidth : boardHeight;
            for(int i = 1; i < maxValue - 1; i++)
            {
                nextX = startX + Mathf.Clamp((int)searchDirection.x, -1, 1) * i;
                nextY = startY + Mathf.Clamp((int)searchDirection.y, -1, 1) * i;
                if(!IsWithinBounds(nextX, nextY))
                {
                    break;
                }
                GamePiece nextPiece = allPieces[nextX, nextY];
                if(nextPiece == null)
                {
                    break;
                }
                else
                {
                    if (nextPiece.PieceType == startPiece.PieceType && !matches.Contains(nextPiece))
                    {
                        matches.Add(nextPiece);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if(matches.Count >= minLength)
            {
                return matches;
            }
            return null;
        }

        private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
        {
            List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
            List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);
            if(upwardMatches == null)
            {
                upwardMatches = new List<GamePiece>();
            }
            if(downwardMatches == null)
            {
                downwardMatches = new List<GamePiece>();
            }
            var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
            return (combinedMatches.Count >= minLength) ? combinedMatches : null;
        }

        private List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
        {
            List<GamePiece> rightwardMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
            List<GamePiece> leftwardMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);
            if (rightwardMatches == null)
            {
                rightwardMatches = new List<GamePiece>();
            }
            if (leftwardMatches == null)
            {
                leftwardMatches = new List<GamePiece>();
            }
            var combinedMatches = rightwardMatches.Union(leftwardMatches).ToList();
            return (combinedMatches.Count >= minLength) ? combinedMatches : null;
        }

        private List<GamePiece> FindMatchesAt(int i, int j, int minLength = 3)
        {
            List<GamePiece> horizontalMatches = FindHorizontalMatches(i, j, minLength);
            List<GamePiece> verticalMatches = FindVerticalMatches(i, j, minLength);
            if (horizontalMatches == null)
            {
                horizontalMatches = new List<GamePiece>();
            }
            if (verticalMatches == null)
            {
                verticalMatches = new List<GamePiece>();
            }
            var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
            return combinedMatches;
        }

        private void HighlightTilesOff(int i, int j)
        {
            SpriteRenderer spriteRenderer = allTiles[i, j].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
            }
        }

        private void HighlightTilesOn(int i, int j, Color color)
        {
            SpriteRenderer spriteRenderer = allTiles[i, j].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }

        private void HighlightMatchesAt(int i, int j)
        {
            HighlightTilesOff(i, j);
            List<GamePiece> combinedMatches = FindMatchesAt(i, j);
            if (combinedMatches.Count > 0)
            {
                foreach (var item in combinedMatches)
                {
                    if (item != null)
                    {
                        HighlightTilesOn(item.XIndex, item.YIndex, item.GetComponent<SpriteRenderer>().color);
                    }
                }
            }
        }

        private void HighlightMatches()
        {
            for(int i = 0; i < boardWidth; i++)
            {
                for(int j = 0; j < boardHeight; j++)
                {
                    HighlightMatchesAt(i, j);
                }
            }
        }

        private void ClearPieceAt(int i, int j)
        {
            GamePiece gamePiece = allPieces[i, j];
            if (gamePiece != null)
            {
                allPieces[i, j] = null;
                Destroy(gamePiece.gameObject);
            }
            HighlightTilesOff(i, j);
        }

        private void ClearPieceAt(List<GamePiece> gamePieces)
        {
            if (gamePieces != null)
            {
                foreach (var item in gamePieces)
                {
                    if (item != null)
                    {
                        ClearPieceAt(item.XIndex, item.YIndex);
                    }
                }
            }
        }

        private void ClearPieces()
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    ClearPieceAt(i, j);
                }
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator SwitchTilesRoutine(GameTile currentTile, GameTile targetTile)
        {
            if(currentTile != null && targetTile != null)
            {
                GamePiece clickedGamePiece = allPieces[currentTile.XIndex, currentTile.YIndex];
                GamePiece targetGamePiece = allPieces[targetTile.XIndex, targetTile.YIndex];
                if(clickedGamePiece != null && targetGamePiece != null)
                {
                    clickedGamePiece.MovePiece(targetTile.XIndex, targetTile.YIndex, gamePieceMoveSpeed);
                    targetGamePiece.MovePiece(currentTile.XIndex, currentTile.YIndex, gamePieceMoveSpeed);
                    yield return new WaitForSeconds(gamePieceMoveSpeed);
                    List<GamePiece> clickedPieceMatches = FindMatchesAt(currentTile.XIndex, currentTile.YIndex);
                    List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.XIndex, targetTile.YIndex);
                    if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0)
                    {
                        clickedGamePiece.MovePiece(currentTile.XIndex, currentTile.YIndex, gamePieceMoveSpeed);
                        targetGamePiece.MovePiece(targetTile.XIndex, targetTile.YIndex, gamePieceMoveSpeed);
                    }
                    else
                    {
                        yield return new WaitForSeconds(gamePieceMoveSpeed);
                        ClearPieceAt(clickedPieceMatches);
                        ClearPieceAt(targetPieceMatches);
                        //HighlightMatchesAt(currentTile.XIndex, currentTile.YIndex);
                        //HighlightMatchesAt(targetTile.XIndex, targetTile.YIndex);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void SetClickedTile(GameTile tile)
        {
            if (clickedTile == null)
            {
                clickedTile = tile;
            }
        }

        public void SetTargetTile(GameTile tile)
        {
            if (clickedTile != null && IsNextTo(tile, clickedTile))
            {
                targetTile = tile;
            }
        }

        public void ReleaseTile()
        {
            if (clickedTile != null && targetTile != null)
            {
                SwitchTiles(clickedTile, targetTile);
            }
            clickedTile = null;
            targetTile = null;
        }

        public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
        {
            if (gamePiece == null)
            {
                Debug.Log($"GAMEBOARD - gamepiece is null".ToRed().ToBold());
                return;
            }
            gamePiece.transform.position = new Vector3(x, y, 0);
            gamePiece.transform.rotation = Quaternion.identity;
            if (IsWithinBounds(x, y))
            {
                allPieces[x, y] = gamePiece;
            }
            gamePiece.SetCoordinates(x, y);
        }
        #endregion
    }
}
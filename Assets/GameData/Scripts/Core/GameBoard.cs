using Match3.Utilities;
using Match3.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

namespace Match3.Core
{
    [RequireComponent(typeof(BoardDeadLock))]
    [RequireComponent(typeof(BoardShuffler))]
    public class GameBoard : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Board Prefabs")]
        [SerializeField] private GameObject gameTilePrefab = null;
        [SerializeField] private BackgroundTile gamePieceBackgroundTile = null;
        [SerializeField] private GameObject gameObstacleTilePrefab = null;
        [SerializeField] private GameObject adjacentBombPrefab = null;
        [SerializeField] private GameObject rowBombPrefab = null;
        [SerializeField] private GameObject coloumnBombPrefab = null;
        [SerializeField] private GameObject colorBombPrefab = null;
        [Header("Board Dimensions")]
        [SerializeField] private int boardWidth = -1;
        [SerializeField] private int boardHeight = -1;
        [SerializeField] private float borderSize = -1f;
        [Header("Board Properties")]
        [SerializeField] private Transform gamePieceParent = null;
        [SerializeField] private GameObject[] gamePiecePrefabs = null;
        [SerializeField] private List<GamePieceHolder> gamePieceHolders = null;
        [SerializeField] private List<BombSpriteHolder> bombSpriteHolders = null;
        [SerializeField] private float gamePieceMoveSpeed = 0.3f;
        [SerializeField] private float fillYOffset = 10f;
        [SerializeField] private float fallTime = 0.5f;
        [SerializeField] private Button reloadLevelButton = null;
        [SerializeField] private StartingObject[] startingTiles = null;
        [SerializeField] private StartingObject[] startingPieces = null;
        [SerializeField] private ParticleSystemManager particleSystemManager = null;
        [Header("Collectable Properties")]
        [SerializeField] private int maxCollectables = 3;
        [SerializeField] [Range(0, 1)] private float collectableChance = 0.1f;
        [SerializeField] private GameObject[] collectablePrefabs = null;
        #endregion

        #region Variables
        private GameTile[,] allTiles;
        private GamePiece[,] allPieces;
        private BackgroundTile[,] allBackgroundTiles;
        private GameTile clickedTile;
        private GameTile targetTile;
        private bool canSwitchTiles = true;
        private GameObject clickedTileBomb = null;
        private GameObject targetTileBomb = null;
        private int currentLevelCollectableCount = -1;
        private int scoreMultiplier = 0;
        private int bonusCounter = 0;
        private Level currentLevel = null;
        private bool isRefilling = false;
        private BoardDeadLock boardDeadLock = null;
        private BoardShuffler boardShuffler = null;
        #endregion

        #region Properties
        public bool IsRefilling { get => isRefilling; set => isRefilling = value; }
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
            if (reloadLevelButton != null)
            {
                reloadLevelButton.onClick.RemoveAllListeners();
                reloadLevelButton.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene(0);
                });
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CheckForLevelLoad();
                currentLevel = GameManager.Instance.CurrentGameLevel;
                if (currentLevel != null)
                {
                    Debug.Log($"Current Level id - {currentLevel.id}".ToAqua().ToBold());
                    boardHeight = currentLevel.boardHeight;
                    boardWidth = currentLevel.boardWidth;
                }
            }
            boardDeadLock = GetComponent<BoardDeadLock>();
            boardShuffler = GetComponent<BoardShuffler>();
        }

        private void MakeTile(GameObject prefab, int i, int j, int k)
        {
            if (prefab != null && IsWithinBounds(i, j))
            {
                GameObject tile = Instantiate(prefab, transform);
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

        private void MakeGamePiece(GameObject gamePiece, int x, int y, float fillYOffset = 10f, float fallTime = 0.1f)
        {
            if (gamePiece != null && IsWithinBounds(x, y))
            {
                gamePiece.transform.SetParent(gamePieceParent);
                gamePiece.GetComponent<GamePiece>().Initialize(this);
                PlaceGamePiece(gamePiece.GetComponent<GamePiece>(), x, y);
                if (fillYOffset != 0f)
                {
                    gamePiece.transform.position = new Vector3(x, y + fillYOffset, 0f);
                    gamePiece.GetComponent<GamePiece>().MovePiece(x, y, fallTime);
                }
            }
        }

        private GameObject MakeBomb(GameObject prefab, int x, int y)
        {
            GameObject bomb = null;
            if (prefab != null && IsWithinBounds(x, y))
            {
                bomb = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
                Bomb bombObject = bomb.GetComponent<Bomb>();
                if (bombObject != null)
                {
                    bombObject.Initialize(this);
                    bombObject.SetCoordinates(x, y);
                }
                bomb.transform.SetParent(gamePieceParent);
            }
            return bomb;
        }

        private void SetupTiles()
        {
            foreach (var item in startingTiles)
            {
                if (item != null)
                {
                    MakeTile(item.objectPrefab, item.x, item.y, item.z);
                }
            }
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (allTiles[i, j] == null)
                    {
                        MakeTile(gameTilePrefab, i, j, 0);
                    }
                }
            }
        }

        private void SetupPieces()
        {
            foreach (var item in startingPieces)
            {
                if (item != null)
                {
                    GameObject piece = Instantiate(item.objectPrefab, new Vector3(item.x, item.y, item.z), Quaternion.identity);
                    MakeGamePiece(piece, item.x, item.y, fillYOffset, fallTime);
                }
            }
        }

        private void SetupBackgroundGameTiles()
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (allBackgroundTiles[i, j] == null)
                    {
                        if (gamePieceBackgroundTile != null)
                        {
                            GameObject bgTile = Instantiate(gamePieceBackgroundTile.gameObject, new Vector2(i, fillYOffset), Quaternion.identity);
                            bgTile.transform.SetParent(gamePieceParent);
                            var backgroundTile = bgTile.GetComponent<BackgroundTile>();
                            if (backgroundTile != null)
                            {
                                var isEven = GetEvenTile(i, j, boardWidth);
                                backgroundTile.SetColor(isEven);
                                backgroundTile.MoveTileTo(new Vector2(i, fillYOffset), new Vector2(i, j), fallTime);
                            }
                        }
                    }
                }
            }
        }

        private bool GetEvenTile(int i, int j, int boardWidth)
        {
            return ((i + (j * boardWidth)) % 2) == 0;
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

        private GameObject GetRandomObject(GameObject[] gameObjects)
        {
            int index = Random.Range(0, gameObjects.Length);
            if (gameObjects[index] == null)
            {
                Debug.Log($"GAMEBOARD : Object found Null".ToRed().ToBold());
            }
            return gameObjects[index];
        }

        private GameObject GetRandomPiece()
        {
            return GetRandomObject(gamePiecePrefabs);
        }

        private GameObject GetRandomCollectable()
        {
            return GetRandomObject(collectablePrefabs);
        }

        private void FillBoard(float falseYOffset = 0f, float fallTime = 0.1f)
        {
            int maxIterations = 100;
            int currentIterations = 0;
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (allPieces[i, j] == null && allTiles[i, j].TileType != Enums.TileType.Obstacle)
                    {
                        GamePiece randomPiece = null;
                        if (j == boardHeight - 1 && CanAddCollectable())
                        {
                            randomPiece = FillRandomCollectableAt(i, j, falseYOffset, fallTime);
                            currentLevelCollectableCount++;
                        }
                        else
                        {
                            randomPiece = FillRandomGamePieceAt(i, j, falseYOffset, fallTime);
                            currentIterations = 0;
                            while (HasMatchOnFill(i, j, 3))
                            {
                                ClearPieceAt(i, j);
                                randomPiece = FillRandomGamePieceAt(i, j, falseYOffset, fallTime);
                                currentIterations++;
                                if (currentIterations >= maxIterations)
                                {
                                    Debug.Log($"BOARD::Max iterations made".ToRed().ToBold());
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillBoardWithCurrentLevelData(float falseYOffSet = 0f, float fallTime = 0.1f)
        {
            if (currentLevel != null)
            {
                List<LevelPiece> levelPieces = currentLevel.levelPieces;
                int xStart = 0;
                int yStart = boardHeight - 1;
                for (int i = 0; i < boardWidth; i++)
                {
                    for (int j = 0; j < boardHeight; j++)
                    {
                        if (allPieces[i, j] == null)
                        {
                            if (levelPieces != null)
                            {
                                int pieceIndex = (boardWidth * i) + j;
                                if (levelPieces[pieceIndex] != null)
                                {
                                    int pieceColorIndex = levelPieces[pieceIndex].pieceValue;
                                    if (gamePieceHolders != null)
                                    {
                                        foreach (var item in gamePieceHolders)
                                        {
                                            if ((int)item.pieceColor == pieceColorIndex)
                                            {
                                                GameObject targetGameObject = Instantiate(item.piecePrefab, Vector3.zero, Quaternion.identity);
                                                if (targetGameObject != null && IsWithinBounds(i, j))
                                                {
                                                    targetGameObject.transform.SetParent(gamePieceParent);
                                                    GamePiece targetPiece = targetGameObject.GetComponent<GamePiece>();
                                                    if (targetPiece != null)
                                                    {
                                                        targetPiece.Initialize(this);
                                                    }
                                                    targetPiece.transform.position = new Vector3(xStart + i, yStart - j, 0);
                                                    targetPiece.transform.rotation = Quaternion.identity;
                                                    if (IsWithinBounds(i, j))
                                                    {
                                                        allPieces[i, j] = targetPiece;
                                                    }
                                                    targetPiece.SetCoordinates(i, j);
                                                    if (falseYOffSet != 0f)
                                                    {
                                                        targetGameObject.transform.position = new Vector3(i, j + falseYOffSet, 0f);
                                                        targetPiece.MovePiece(i, j, fallTime);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillBoardFromList(List<GamePiece> gamePieces)
        {
            Queue<GamePiece> unusedPieces = new Queue<GamePiece>(gamePieces);
            int maxIterations = 100;
            int currentIterations = 0;
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (allPieces[i, j] == null && allTiles[i, j].TileType != TileType.Obstacle)
                    {
                        allPieces[i, j] = unusedPieces.Dequeue();
                        currentIterations = 0;
                        while (HasMatchOnFill(i, j))
                        {
                            unusedPieces.Enqueue(allPieces[i, j]);
                            allPieces[i, j] = unusedPieces.Dequeue();
                            currentIterations++;
                            if (currentIterations >= maxIterations)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private GamePiece FillRandomGamePieceAt(int i, int j, float yOffset = 0f, float fallTime = 0.1f)
        {
            if (IsWithinBounds(i, j))
            {
                GameObject gamePiece = Instantiate(GetRandomPiece(), Vector3.zero, Quaternion.identity);
                MakeGamePiece(gamePiece, i, j, yOffset, fallTime);
                return gamePiece.GetComponent<GamePiece>();
            }
            return null;
        }

        private GamePiece FillRandomCollectableAt(int i, int j, float yOffset = 0f, float fallTime = 0.1f)
        {
            if (IsWithinBounds(i, j))
            {
                GameObject gamePiece = Instantiate(GetRandomCollectable(), Vector3.zero, Quaternion.identity);
                MakeGamePiece(gamePiece, i, j, yOffset, fallTime);
                return gamePiece.GetComponent<GamePiece>();
            }
            return null;
        }

        private bool HasMatchOnFill(int i, int j, int minLength = 3)
        {
            List<GamePiece> leftMatches = FindMatches(i, j, Vector2.left, minLength);
            List<GamePiece> downMatches = FindMatches(i, j, Vector2.down, minLength);
            if (leftMatches == null)
            {
                leftMatches = new List<GamePiece>();
            }
            if (downMatches == null)
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
            if (Mathf.Abs(start.XIndex - end.XIndex) == 1 && start.YIndex == end.YIndex)
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
            if (IsWithinBounds(startX, startY))
            {
                startPiece = allPieces[startX, startY];
            }
            if (startPiece != null)
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
            for (int i = 1; i < maxValue - 1; i++)
            {
                nextX = startX + Mathf.Clamp((int)searchDirection.x, -1, 1) * i;
                nextY = startY + Mathf.Clamp((int)searchDirection.y, -1, 1) * i;
                if (!IsWithinBounds(nextX, nextY))
                {
                    break;
                }
                GamePiece nextPiece = allPieces[nextX, nextY];
                if (nextPiece == null)
                {
                    break;
                }
                else
                {
                    if (nextPiece.PieceType == startPiece.PieceType && !matches.Contains(nextPiece) && nextPiece.PieceType != GamePieceType.Collectable)
                    {
                        matches.Add(nextPiece);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (matches.Count >= minLength)
            {
                return matches;
            }
            return null;
        }

        private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
        {
            List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
            List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);
            if (upwardMatches == null)
            {
                upwardMatches = new List<GamePiece>();
            }
            if (downwardMatches == null)
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

        private List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
        {
            List<GamePiece> matches = new List<GamePiece>();
            foreach (var item in gamePieces)
            {
                matches = matches.Union(FindMatchesAt(item.XIndex, item.YIndex, minLength)).ToList();
            }
            return matches;
        }

        private List<GamePiece> FindAllMatchesOnBoard()
        {
            List<GamePiece> allMatches = new List<GamePiece>();
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    List<GamePiece> matches = FindMatchesAt(i, j);
                    allMatches = allMatches.Union(matches).ToList();
                }
            }
            return allMatches;
        }

        private void HighlightTilesOff(int i, int j)
        {
            if (allTiles[i, j].TileType != TileType.Breakable)
            {
                SpriteRenderer spriteRenderer = allTiles[i, j].GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
                }
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
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    HighlightMatchesAt(i, j);
                }
            }
        }

        private void HighlightPieces(List<GamePiece> gamePieces)
        {
            if (gamePieces != null)
            {
                foreach (var item in gamePieces)
                {
                    if (item != null)
                    {
                        HighlightTilesOn(item.XIndex, item.YIndex, item.GetComponent<SpriteRenderer>().color);
                    }
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
            //HighlightTilesOff(i, j);
        }

        private void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombedPieces)
        {
            if (gamePieces != null)
            {
                foreach (var item in gamePieces)
                {
                    if (item != null)
                    {
                        ClearPieceAt(item.XIndex, item.YIndex);
                        bonusCounter = 0;
                        if (gamePieces.Count >= 4)
                        {
                            bonusCounter = 20;
                        }
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.AddScore(item, scoreMultiplier, bonusCounter);
                            TimeBonus timeBonus = item.GetComponent<TimeBonus>();
                            if(timeBonus != null)
                            {
                                GameManager.Instance.Addtime(timeBonus.BonusValue);
                            }
                            GameManager.Instance.CheckForCollectionGoals(item);
                        }
                        if (particleSystemManager != null)
                        {
                            if (bombedPieces.Contains(item))
                            {
                                particleSystemManager.PlayBombEffect(item.XIndex, item.YIndex, 0);
                            }
                            else
                            {
                                particleSystemManager.PlayClearPieceEffect(item.XIndex, item.YIndex, 0);
                            }
                        }

                    }
                }
            }
        }

        private void ClearAllPieces()
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    ClearPieceAt(i, j);
                    if (particleSystemManager != null)
                    {
                        particleSystemManager.PlayClearPieceEffect(i, j);
                    }
                }
            }
        }

        private List<GamePiece> CollapseColoumn(int coloumn, float fallTime = 0.1f)
        {
            List<GamePiece> movingPieces = new List<GamePiece>();
            for (int i = 0; i < boardHeight - 1; i++)
            {
                if (allPieces[coloumn, i] == null && allTiles[coloumn, i].TileType != TileType.Obstacle)
                {
                    for (int j = i + 1; j < boardHeight; j++)
                    {
                        if (allPieces[coloumn, j] != null)
                        {
                            allPieces[coloumn, j].MovePiece(coloumn, i, fallTime * (j - i));
                            allPieces[coloumn, i] = allPieces[coloumn, j];
                            allPieces[coloumn, i].SetCoordinates(coloumn, i);
                            if (!movingPieces.Contains(allPieces[coloumn, i]))
                            {
                                movingPieces.Add(allPieces[coloumn, i]);
                            }
                            allPieces[coloumn, j] = null;
                            break;
                        }
                    }
                }
            }
            return movingPieces;
        }

        private List<GamePiece> CollapseColoumn(List<GamePiece> gamePieces)
        {
            List<GamePiece> movingPieces = new List<GamePiece>();
            List<int> coloumns = GetColoumns(gamePieces);
            foreach (var coloumn in coloumns)
            {
                movingPieces = movingPieces.Union(CollapseColoumn(coloumn)).ToList();
            }
            return movingPieces;
        }

        private List<GamePiece> CollapseColoumn(List<int> columnsToCollapse)
        {
            List<GamePiece> movingPieces = new List<GamePiece>();
            foreach (var item in columnsToCollapse)
            {
                movingPieces = movingPieces.Union(CollapseColoumn(item)).ToList();
            }
            return movingPieces;
        }

        private List<int> GetColoumns(List<GamePiece> gamePieces)
        {
            List<int> coloumns = new List<int>();
            foreach (var item in gamePieces)
            {
                if (!coloumns.Contains(item.XIndex))
                {
                    coloumns.Add(item.XIndex);
                }
            }
            return coloumns;
        }

        private void ClearAndRefillBoard(List<GamePiece> gamePieces)
        {
            StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
        }

        private bool IsCollapsed(List<GamePiece> gamePieces)
        {
            foreach (var item in gamePieces)
            {
                if (item != null)
                {
                    if ((item.transform.position.y - (float)item.YIndex) > 0.001f)
                    {
                        return false;
                    }
                    if ((item.transform.position.x - (float)item.XIndex) > 0.001f)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void BreakTileAt(List<GamePiece> gamePieces)
        {
            if (gamePieces != null)
            {
                foreach (var item in gamePieces)
                {
                    if (item != null)
                    {
                        BreakTileAt(item.XIndex, item.YIndex);
                    }
                }
            }
        }

        private void BreakTileAt(int i, int j)
        {
            GameTile tileToBreak = allTiles[i, j];
            if (tileToBreak != null && tileToBreak.TileType == TileType.Breakable)
            {
                if (particleSystemManager != null)
                {
                    particleSystemManager.PlayBreakableTilesEffect(tileToBreak.CurrentBreakableValue, i, j, 0);
                }
                tileToBreak.BreakTile();
            }
        }

        private List<GamePiece> GetRowPieces(int row)
        {
            List<GamePiece> rowPieces = new List<GamePiece>();
            for (int i = 0; i < boardWidth; i++)
            {
                if (allPieces[i, row] != null)
                {
                    rowPieces.Add(allPieces[i, row]);
                }
            }
            return rowPieces;
        }

        private List<GamePiece> GetColoumnPieces(int coloumn)
        {
            List<GamePiece> coloumnPieces = new List<GamePiece>();
            for (int i = 0; i < boardHeight; i++)
            {
                if (allPieces[coloumn, i] != null)
                {
                    coloumnPieces.Add(allPieces[coloumn, i]);
                }
            }
            return coloumnPieces;
        }

        private List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
        {
            List<GamePiece> adjacentPieces = new List<GamePiece>();
            for (int i = x - offset; i <= x + offset; i++)
            {
                for (int j = y - offset; j <= y + offset; j++)
                {
                    if (IsWithinBounds(i, j))
                    {
                        adjacentPieces.Add(allPieces[i, j]);
                    }
                }
            }
            return adjacentPieces;
        }

        private List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
        {
            List<GamePiece> allPiecesToClear = new List<GamePiece>();
            foreach (var item in gamePieces)
            {
                if (item != null)
                {
                    List<GamePiece> piecesToClear = new List<GamePiece>();
                    Bomb bomb = item.GetComponent<Bomb>();
                    if (bomb != null)
                    {
                        switch (bomb.BombType)
                        {
                            case BombType.None:
                                break;
                            case BombType.Color:
                                break;
                            case BombType.Adjacent:
                                piecesToClear = GetAdjacentPieces(bomb.XIndex, bomb.YIndex, 1);
                                break;
                            case BombType.Coloumn:
                                piecesToClear = GetColoumnPieces(bomb.XIndex);
                                break;
                            case BombType.Row:
                                piecesToClear = GetRowPieces(bomb.YIndex);
                                break;
                            default:
                                break;
                        }
                        allPiecesToClear = allPiecesToClear.Union(piecesToClear).ToList();
                        allPiecesToClear = RemoveCollectables(allPiecesToClear);
                    }
                }
            }
            return allPiecesToClear;
        }

        private bool IsCornerMatch(List<GamePiece> gamePieces)
        {
            bool vertical = false;
            bool horizontal = false;
            int xStart = -1;
            int yStart = -1;
            foreach (var item in gamePieces)
            {
                if (item != null)
                {
                    if (xStart == -1 || yStart == -1)
                    {
                        xStart = item.XIndex;
                        yStart = item.YIndex;
                        continue;
                    }
                    if (item.XIndex != xStart && item.YIndex == yStart)
                    {
                        horizontal = true;
                    }
                    if (item.XIndex == xStart && item.YIndex != yStart)
                    {
                        vertical = true;
                    }
                }
            }
            return horizontal && vertical;
        }

        private GameObject DropBomb(int x, int y, Vector2 swapDireciton, List<GamePiece> gamePieces)
        {
            GameObject bomb = null;
            if (gamePieces.Count >= 4)
            {
                if (IsCornerMatch(gamePieces))
                {
                    if (adjacentBombPrefab != null)
                    {
                        bomb = MakeBomb(adjacentBombPrefab, x, y);
                    }
                }
                else
                {
                    if (gamePieces.Count >= 5)
                    {
                        if (colorBombPrefab != null)
                        {
                            bomb = MakeBomb(colorBombPrefab, x, y);
                        }
                    }
                    else
                    {
                        if (swapDireciton.x != 0)
                        {
                            if (rowBombPrefab != null)
                            {
                                bomb = MakeBomb(rowBombPrefab, x, y);
                            }
                        }
                        else
                        {
                            if (coloumnBombPrefab != null)
                            {
                                bomb = MakeBomb(coloumnBombPrefab, x, y);
                            }
                        }
                    }
                }
            }
            return bomb;
        }

        private void ActivateBomb(GameObject bomb)
        {
            if (bomb != null)
            {
                int x = (int)bomb.transform.position.x;
                int y = (int)bomb.transform.position.y;
                if (IsWithinBounds(x, y))
                {
                    allPieces[x, y] = bomb.GetComponent<GamePiece>();
                }
            }
        }

        private List<GamePiece> FindAllPiecesByType(GamePieceType gamePieceType)
        {
            List<GamePiece> gamePieces = new List<GamePiece>();
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (allPieces[i, j].PieceType == gamePieceType)
                    {
                        gamePieces.Add(allPieces[i, j]);
                    }
                }
            }
            return gamePieces;
        }

        private bool IsColorBomb(GamePiece gamePiece)
        {
            if (gamePiece == null)
            {
                return false;
            }
            Bomb bomb = gamePiece.GetComponent<Bomb>();
            if (bomb != null)
            {
                return (bomb.BombType == BombType.Color) ? true : false;
            }
            return false;
        }

        private List<GamePiece> FindCollectablesAt(int row)
        {
            List<GamePiece> gamePieces = new List<GamePiece>();
            for (int i = 0; i < boardWidth; i++)
            {
                if (allPieces[i, row] != null)
                {
                    Collectable collectable = allPieces[i, row].GetComponent<Collectable>();
                    if (collectable != null && collectable.CollectableType == CollectableType.ClearedAtBottom)
                    {
                        gamePieces.Add(collectable);
                    }
                }
            }
            return gamePieces;
        }

        private List<GamePiece> FindAllCollectables()
        {
            List<GamePiece> collectables = new List<GamePiece>();
            for (int i = 0; i < boardHeight; i++)
            {
                List<GamePiece> gamePieces = FindCollectablesAt(i);
                collectables = collectables.Union(gamePieces).ToList();
            }
            return collectables;
        }

        private bool CanAddCollectable()
        {
            return (Random.Range(0f, 1f) <= collectableChance && collectablePrefabs.Length > 0 && currentLevelCollectableCount < maxCollectables);
        }

        private List<GamePiece> RemoveCollectables(List<GamePiece> bombedPieces)
        {
            List<GamePiece> collectablePieces = FindAllCollectables();
            List<GamePiece> removablePieces = new List<GamePiece>();
            foreach (var item in collectablePieces)
            {
                if (item != null)
                {
                    Collectable collectable = item.GetComponent<Collectable>();
                    if (collectable != null)
                    {
                        if (collectable.CollectableType != CollectableType.ClearedByBomb)
                        {
                            removablePieces.Add(item);
                        }
                    }
                }
            }
            return bombedPieces.Except(removablePieces).ToList();
        }

        private Sprite GetBombSprite(GamePieceType pieceColor)
        {
            Sprite targetSprite = null;
            if (bombSpriteHolders != null)
            {
                foreach (var item in bombSpriteHolders)
                {
                    if (item.pieceType == pieceColor)
                    {
                        targetSprite = item.pieceSprite;
                    }
                }
            }
            return targetSprite;
        }

        private void ShuffleBoard()
        {
            if (canSwitchTiles)
            {
                StartCoroutine(ShuffleBoardRoutine());
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator SwitchTilesRoutine(GameTile clickedTile, GameTile targetTile)
        {
            if (canSwitchTiles && !GameManager.Instance.IsGameOver)
            {
                if (clickedTile != null && targetTile != null)
                {
                    GamePiece clickedGamePiece = allPieces[clickedTile.XIndex, clickedTile.YIndex];
                    GamePiece targetGamePiece = allPieces[targetTile.XIndex, targetTile.YIndex];
                    if (clickedGamePiece != null && targetGamePiece != null)
                    {
                        clickedGamePiece.MovePiece(targetTile.XIndex, targetTile.YIndex, gamePieceMoveSpeed);
                        targetGamePiece.MovePiece(clickedTile.XIndex, clickedTile.YIndex, gamePieceMoveSpeed);
                        yield return new WaitForSeconds(gamePieceMoveSpeed);
                        List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.XIndex, clickedTile.YIndex);
                        List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.XIndex, targetTile.YIndex);
                        #region Color Matches
                        List<GamePiece> coloredMatches = new List<GamePiece>();
                        if (IsColorBomb(clickedGamePiece) && !IsColorBomb(targetGamePiece))
                        {
                            clickedGamePiece.PieceType = targetGamePiece.PieceType;
                            coloredMatches = FindAllPiecesByType(clickedGamePiece.PieceType);
                        }
                        else if (!IsColorBomb(clickedGamePiece) && IsColorBomb(targetGamePiece))
                        {
                            targetGamePiece.PieceType = clickedGamePiece.PieceType;
                            coloredMatches = FindAllPiecesByType(targetGamePiece.PieceType);
                        }
                        else if (IsColorBomb(clickedGamePiece) && IsColorBomb(targetGamePiece))
                        {
                            foreach (var item in allPieces)
                            {
                                if (!coloredMatches.Contains(item))
                                {
                                    coloredMatches.Add(item);
                                }
                            }
                        }
                        #endregion
                        if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0 && coloredMatches.Count == 0)
                        {
                            clickedGamePiece.MovePiece(clickedTile.XIndex, clickedTile.YIndex, gamePieceMoveSpeed);
                            targetGamePiece.MovePiece(targetTile.XIndex, targetTile.YIndex, gamePieceMoveSpeed);
                        }
                        else
                        {
                            yield return new WaitForSeconds(gamePieceMoveSpeed);
                            Vector2 swipeDirection = new Vector2(targetTile.XIndex - clickedTile.XIndex, targetTile.YIndex - clickedTile.YIndex);
                            #region Bombs
                            clickedTileBomb = DropBomb(clickedTile.XIndex, clickedTile.YIndex, swipeDirection, clickedPieceMatches);
                            targetTileBomb = DropBomb(targetTile.XIndex, targetTile.YIndex, swipeDirection, targetPieceMatches);
                            if (clickedTileBomb != null && targetGamePiece != null)
                            {
                                GamePiece clickedBombPiece = clickedTileBomb.GetComponent<GamePiece>();
                                if (!IsColorBomb(clickedBombPiece))
                                {
                                    Sprite targetSprite = GetBombSprite(targetGamePiece.PieceType);
                                    if (targetSprite != null)
                                    {
                                        clickedBombPiece.ChangeSprite(targetSprite);
                                        clickedBombPiece.PieceType = targetGamePiece.PieceType;
                                    }
                                    //clickedBombPiece.ChangeColor(targetGamePiece);
                                }
                            }
                            if (targetTileBomb != null && clickedGamePiece != null)
                            {
                                GamePiece targetBombPiece = targetTileBomb.GetComponent<GamePiece>();
                                if (!IsColorBomb(targetBombPiece))
                                {
                                    Sprite targetSprite = GetBombSprite(clickedGamePiece.PieceType);
                                    if (targetSprite != null)
                                    {
                                        targetBombPiece.ChangeSprite(targetSprite);
                                        targetBombPiece.PieceType = clickedGamePiece.PieceType;
                                    }
                                    //targetBombPiece.ChangeColor(clickedGamePiece);
                                }
                            }
                            #endregion
                            List<GamePiece> piecesToClear = clickedPieceMatches.Union(targetPieceMatches).ToList().Union(coloredMatches).ToList();
                            yield return StartCoroutine(ClearAndRefillBoardRoutine(piecesToClear));
                            if (GameManager.Instance != null)
                            {
                                GameManager.Instance.UpdateMoves();
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
        {
            canSwitchTiles = false;
            IsRefilling = true;
            List<GamePiece> matches = gamePieces;
            scoreMultiplier = 0;
            do
            {
                scoreMultiplier++;
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
                yield return null;
                yield return StartCoroutine(RefillRoutine());
                matches = FindAllMatchesOnBoard();
                yield return new WaitForSeconds(0.5f);
            } while (matches.Count != 0);
            // check for deadlock
            if (boardDeadLock != null)
            {
                var isDeadLock = boardDeadLock.IsDeadLocked(allPieces, 3);
                if (isDeadLock)
                {
                    yield return new WaitForSeconds(1f);
                    //ClearAllPieces();
                    yield return StartCoroutine(ShuffleBoardRoutine());
                    yield return new WaitForSeconds(1f);
                    yield return StartCoroutine(RefillRoutine());
                }
            }
            canSwitchTiles = true;
            IsRefilling = false;
        }

        private IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
        {
            List<GamePiece> movingPieces = new List<GamePiece>();
            List<GamePiece> matches = new List<GamePiece>();
            float visualDelay = 0.25f;
            bool isFinished = false;
            //HighlightPieces(gamePieces);
            yield return new WaitForSeconds(visualDelay);
            while (!isFinished)
            {
                List<GamePiece> bombedPieces = GetBombedPieces(gamePieces);
                gamePieces = gamePieces.Union(bombedPieces).ToList();
                bombedPieces = GetBombedPieces(gamePieces);
                gamePieces = gamePieces.Union(bombedPieces).ToList();
                List<GamePiece> collectablePieces = FindCollectablesAt(0);
                List<GamePiece> allCollectables = FindAllCollectables();
                List<GamePiece> blockers = gamePieces.Intersect(allCollectables).ToList();
                collectablePieces = collectablePieces.Union(blockers).ToList();
                currentLevelCollectableCount -= collectablePieces.Count;
                //Debug.Log($"Current collectable count - {currentLevelCollectableCount}".ToAqua().ToBold());
                gamePieces = gamePieces.Union(collectablePieces).ToList();
                List<int> coloumnsToCollapse = GetColoumns(gamePieces);
                ClearPieceAt(gamePieces, bombedPieces);
                BreakTileAt(gamePieces);
                if (clickedTileBomb != null)
                {
                    ActivateBomb(clickedTileBomb);
                    clickedTileBomb = null;
                }
                if (targetTileBomb != null)
                {
                    ActivateBomb(targetTileBomb);
                    targetTileBomb = null;
                }
                yield return new WaitForSeconds(visualDelay / 2f);
                movingPieces = CollapseColoumn(coloumnsToCollapse);
                while (!IsCollapsed(movingPieces))
                {
                    yield return null;
                }
                yield return new WaitForSeconds(visualDelay);
                matches = FindMatchesAt(movingPieces);
                collectablePieces = FindCollectablesAt(0);
                matches = matches.Union(collectablePieces).ToList();
                if (matches.Count == 0)
                {
                    isFinished = true;
                    break;
                }
                else
                {
                    scoreMultiplier++;
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayRandomBonusClips();
                    }
                    yield return StartCoroutine(ClearAndCollapseRoutine(matches));
                }
            }
            yield return null;
        }

        private IEnumerator RefillRoutine()
        {
            FillBoard(fillYOffset, fallTime);
            yield return null;
        }

        private IEnumerator ShuffleBoardRoutine()
        {
            List<GamePiece> allGamePieces = new List<GamePiece>();
            foreach (var item in allPieces)
            {
                allGamePieces.Add(item);
            }
            while (!IsCollapsed(allGamePieces))
            {
                yield return null;
            }
            List<GamePiece> normalPieces = boardShuffler.RemoveNormalPieces(allPieces);
            boardShuffler.ShuffleGamePieces(normalPieces);
            FillBoardFromList(normalPieces);
            boardShuffler.MovePieces(allPieces);
            List<GamePiece> matches = FindAllMatchesOnBoard();
            StartCoroutine(ClearAndRefillBoardRoutine(matches));
        }
        #endregion

        #region Public Methods
        public void SetupBoard()
        {
            Debug.Log($"Board Width - {boardWidth} and boardHeight - {boardHeight}".ToAqua().ToBold());
            allTiles = new GameTile[boardWidth, boardHeight];
            allPieces = new GamePiece[boardWidth, boardHeight];
            allBackgroundTiles = new BackgroundTile[boardWidth, boardHeight];
            SetupBackgroundGameTiles();
            SetupTiles();
            SetupPieces();
            List<GamePiece> allCollectables = FindAllCollectables();
            currentLevelCollectableCount = allCollectables.Count;
            //Debug.Log($"Colectables found on board - {currentLevelCollectableCount}".ToAqua().ToBold());
            SetCameraDimensions();
            if (currentLevel == null)
            {
                FillBoard(fillYOffset, fallTime);
            }
            else
            {
                FillBoardWithCurrentLevelData(fillYOffset, fallTime);
            }
            //HighlightMatches();
        }

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

        public float GetGamePieceMoveSpeed()
        {
            return gamePieceMoveSpeed;
        }

        public void ClearAndRefillBoard(int x, int y)
        {
            if(IsWithinBounds(x,y))
            {
                GamePiece pieceToClear = allPieces[x, y];
                List<GamePiece> gamePieces = new List<GamePiece>();
                gamePieces.Add(pieceToClear);
                ClearAndRefillBoard(gamePieces);
            }
        }

        public void MakeColorBomb(int x, int y)
        {
            if(IsWithinBounds(x,y))
            {
                GamePiece gamePiece = allPieces[x, y];
                if(gamePiece != null)
                {
                    ClearPieceAt(x, y);
                    GameObject bombObject = MakeBomb(colorBombPrefab, x, y);
                    ActivateBomb(bombObject);
                }
            }
        }
        #endregion
    }

    [System.Serializable]
    public class StartingObject
    {
        public GameObject objectPrefab;
        public int x;
        public int y;
        public int z;
    }

    [System.Serializable]
    public class GamePieceHolder
    {
        public PieceColor pieceColor;
        public GameObject piecePrefab;
    }

    [System.Serializable]
    public class BombSpriteHolder
    {
        public GamePieceType pieceType;
        public Sprite pieceSprite;
    }
}
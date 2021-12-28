using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        #endregion

        #region Variables
        private GameTile[,] allTiles;
        private GamePiece[,] allPieces;
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
            FillRandomPieces();
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

        GameObject GetRandomPiece()
        {
            int randomIndex = Random.Range(0, gamePiecePrefabs.Length);
            if(gamePiecePrefabs[randomIndex] == null)
            {
                Debug.Log($"GAMEBOARD - gamepiece is null at index - {randomIndex}".ToRed().ToBold());
                return null;
            }
            return gamePiecePrefabs[randomIndex];
        }

        private void PlaceGamePiece(GamePiece gamePiece, int x, int y)
        {
            if(gamePiece == null)
            {
                Debug.Log($"GAMEBOARD - gamepiece is null".ToRed().ToBold());
                return;
            }
            gamePiece.transform.position = new Vector3(x, y, 0);
            gamePiece.transform.rotation = Quaternion.identity;
            gamePiece.SetCoordinates(x, y);
        }

        private void FillRandomPieces()
        {
            for(int i = 0; i < boardWidth; i++)
            {
                for(int j = 0; j < boardHeight; j++)
                {
                    GameObject gamePiece = Instantiate(GetRandomPiece(), Vector3.zero, Quaternion.identity);
                    if(gamePiece != null)
                    {
                        gamePiece.transform.SetParent(gamePieceParent);
                        PlaceGamePiece(gamePiece.GetComponent<GamePiece>(), i, j);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        #endregion
    }
}
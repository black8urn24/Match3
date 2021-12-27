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
        #endregion

        #region Variables
        private GameTile[,] allTiles;
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
                    }
                }
            }
            SetCameraDimensions();
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
        #endregion

        #region Public Methods
        #endregion
    }
}
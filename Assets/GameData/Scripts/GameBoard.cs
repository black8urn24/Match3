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
                    if(gameTile != null)
                    {
                        allTiles[i, j] = gameTile;
                    }
                }
            }
            #endregion

            #region Public Methods
            #endregion
        }
    }
}
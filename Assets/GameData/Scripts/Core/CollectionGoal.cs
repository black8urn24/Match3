using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class CollectionGoal : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GamePiece prefabToCollect = null;
        [SerializeField] [Range(1, 50)] private int numberToCollect = 1;
        #endregion

        #region Variables
        private SpriteRenderer spriteRenderer = null;
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
            if(prefabToCollect != null)
            {
                spriteRenderer = prefabToCollect.GetComponent<SpriteRenderer>();
            }
        }
        #endregion

        #region Public Methods
        public void CollectPiece(GamePiece gamePiece)
        {
            if(gamePiece != null)
            {
                SpriteRenderer renderer = gamePiece.GetComponent<SpriteRenderer>();
                if(renderer != null)
                {
                    if(renderer.sprite == spriteRenderer.sprite && gamePiece.PieceType == prefabToCollect.PieceType)
                    {
                        numberToCollect--;
                        numberToCollect = Mathf.Clamp(numberToCollect, 0, numberToCollect);
                    }
                }
            }
        }

        public int GetCurrentItemCount()
        {
            return numberToCollect;
        }

        public Sprite GetCurrentGoalSprite()
        {
            return spriteRenderer != null ? spriteRenderer.sprite : prefabToCollect.GetComponent<SpriteRenderer>().sprite;
        }
        #endregion
    }
}
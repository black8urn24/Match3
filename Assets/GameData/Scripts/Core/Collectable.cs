using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Enums;

namespace Match3.Core
{
    public class Collectable : GamePiece
    {
        #region Inspector Variables
        [SerializeField] private CollectableType collectableType = CollectableType.None;
        #endregion

        #region Variables
        #endregion

        #region Properties
        public CollectableType CollectableType { get => collectableType; set => collectableType = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Match3.Core
{
    public class CollectionGoalPanel : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private CollectionGoal collectionGoal = null;
        [SerializeField] private TextMeshProUGUI numberText = null;
        [SerializeField] private Image prefabImage = null;
        #endregion

        #region Variables
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
        public void SetupPanel()
        {
            if (collectionGoal != null && numberText != null && prefabImage != null)
            {
                prefabImage.sprite = collectionGoal.GetCurrentGoalSprite();
                numberText.text = collectionGoal.GetCurrentItemCount().ToString();
            }
        }

        public void UpdatePanel()
        {
            if (collectionGoal != null && numberText != null)
            {
                numberText.text = collectionGoal.GetCurrentItemCount().ToString();
            }
        }

        public void SetCollectionGoal(CollectionGoal goal)
        {
            if (goal != null)
            {
                collectionGoal = goal;
            }
        }
        #endregion
    }
}
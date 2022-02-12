using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Match3.Core
{
    public class UiManager : Singleton<UiManager>
    {
        #region Inspector Variables
        [SerializeField] private RectTransform collectionLayout = null;
        [SerializeField] private int collectionGoalBaseWidth = 125;

        #endregion

        #region Variables
        private CollectionGoalPanel[] collectionGoalsPanels;
        #endregion

        #region Properties
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
        public void SetupCollectionGoalLayout(List<CollectionGoal> collectionGoals)
        {
            if(collectionLayout != null && collectionGoals != null && collectionGoals.Count != 0)
            {
                collectionLayout.sizeDelta = new Vector2(collectionGoals.Count * collectionGoalBaseWidth, collectionLayout.sizeDelta.y);
                collectionGoalsPanels = collectionLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();
            }
            for(int i = 0; i < collectionGoalsPanels.Length; i++)
            {
                if (i < collectionGoals.Count && collectionGoals[i] != null)
                {
                    collectionGoalsPanels[i].gameObject.SetActive(true);
                    collectionGoalsPanels[i].SetCollectionGoal(collectionGoals[i]);
                    collectionGoalsPanels[i].SetupPanel();
                }
                else
                {
                    collectionGoalsPanels[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateCollectionGoalLayout()
        {
            foreach (var item in collectionGoalsPanels)
            {
                if(item != null)
                {
                    if(item.gameObject.activeInHierarchy)
                    {
                        item.UpdatePanel();
                    }
                }
            }
        }
        #endregion
    }
}
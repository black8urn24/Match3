using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Match3.Utilities;

namespace Match3.Core
{
    public class UiManager : Singleton<UiManager>
    {
        #region Inspector Variables
        [SerializeField] private RectTransform collectionLayout = null;
        [SerializeField] private int collectionGoalBaseWidth = 125;
        [SerializeField] private ScreenFader initialScreenFader = null;
        [SerializeField] private TextMeshProUGUI movesCounterText = null;
        [SerializeField] private MessageWindowManager messageWindow = null;
        [SerializeField] private ScoreMeter scoreMeter = null;
        [SerializeField] private GameObject movesParent = null;
        [SerializeField] private GameObject timeCountDownParent = null;
        #endregion

        #region Variables
        private CollectionGoalPanel[] collectionGoalsPanels;
        #endregion

        #region Properties
        public ScreenFader InitialScreenFader { get => initialScreenFader; set => initialScreenFader = value; }
        public TextMeshProUGUI MovesCounterText { get => movesCounterText; set => movesCounterText = value; }
        public MessageWindowManager MessageWindow { get => messageWindow; set => messageWindow = value; }
        public ScoreMeter ScoreMeter { get => scoreMeter; set => scoreMeter = value; }
        public GameObject MovesParent { get => movesParent; set => movesParent = value; }
        public GameObject TimeCountDownParent { get => timeCountDownParent; set => timeCountDownParent = value; }
        #endregion

        #region Unity Methods
        public override void Awake()
        {
            base.Awake();
            if(MessageWindow != null)
            {
                MessageWindow.gameObject.SetActive(true);
            }
            if(InitialScreenFader != null)
            {
                InitialScreenFader.gameObject.SetActive(true);
            }
        }

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
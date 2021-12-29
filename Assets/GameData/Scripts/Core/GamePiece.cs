using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Enums;
using Match3.Utilities;

namespace Match3.Core
{
    public class GamePiece : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GamePieceType pieceType = GamePieceType.None;
        [SerializeField] private GamePieceInterpolationType interpolationType = GamePieceInterpolationType.Linear;
        #endregion

        #region Variables
        private int x = -1;
        private int y = -1;
        private bool isMoving = false;
        #endregion

        #region Properties
        public GamePieceType PieceType { get => pieceType; private set => pieceType = value; }
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MovePiece((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MovePiece((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Coroutines
        private IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
        {
            Vector3 startPosition = transform.position;
            bool isDestinationReached = false;
            float timeElapsed = 0f;
            isMoving = true;
            while (!isDestinationReached)
            {
                if (Vector3.Distance(transform.position, destination) < 0.01f)
                {
                    isDestinationReached = true;
                    transform.position = destination;
                    SetCoordinates((int)destination.x, (int)destination.y);
                    break;
                }
                timeElapsed += Time.deltaTime;
                float lerpTime = Mathf.Clamp(timeElapsed / timeToMove, 0f, 1f);
                switch (interpolationType)
                {
                    case GamePieceInterpolationType.Linear:
                        break;
                    case GamePieceInterpolationType.EaseIn:
                        lerpTime = Mathf.Sin(lerpTime * Mathf.PI * 0.5f);
                        break;
                    case GamePieceInterpolationType.EaseOut:
                        lerpTime = 1 - Mathf.Cos(lerpTime * Mathf.PI * 0.5f);
                        break;
                    case GamePieceInterpolationType.SmoothStep:
                        lerpTime = lerpTime * lerpTime * (3 - 2 * lerpTime);
                        break;
                    case GamePieceInterpolationType.SmootherStep:
                        lerpTime = lerpTime * lerpTime * lerpTime * (lerpTime * (lerpTime * 6 - 15) + 10);
                        break;
                    default:
                        break;
                }
                transform.position = Vector3.Lerp(startPosition, destination, lerpTime);
                yield return null;
            }
            isMoving = false;
        }
        #endregion

        #region Public Methods
        public void SetCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void MovePiece(int destinationX, int destinationY, float timeToMove)
        {
            if (isMoving == false)
            {
                StartCoroutine(MoveRoutine(new Vector3(destinationX, destinationY, 0f), timeToMove));
            }
        }
        #endregion
    }
}
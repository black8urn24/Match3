using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class BackgroundTile : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Color evenColor = Color.white;
        [SerializeField] private Color oddColor = Color.white;
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
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void MoveTo(Vector2 initialPosition, Vector2 finalPosition, float animationDuration = 1f)
        {
            StartCoroutine(MoveToRoutine(initialPosition, finalPosition, animationDuration));
        }
        #endregion

        #region Coroutines
        private IEnumerator MoveToRoutine(Vector2 initialPosition, Vector2 finalPosition, float animationDuration = 1f)
        {
            float animationTime = 0f;
            while(animationTime < animationDuration)
            {
                animationTime += Time.deltaTime;
                float lerpTime = animationTime / animationDuration;
                transform.position = Vector3.Lerp(initialPosition, finalPosition, lerpTime);
                yield return null;
            }
        }
        #endregion

        #region Public Methods
        public void SetColor(bool isEvenColor)
        {
            SetInitialReferences();
            if(isEvenColor)
            {
                if(spriteRenderer != null)
                {
                    spriteRenderer.color = evenColor;
                }
            }
            else
            {
                if(spriteRenderer != null)
                {
                    spriteRenderer.color = oddColor;
                }
            }
        }

        public void MoveTileTo(Vector2 initialPosition, Vector2 finalPosition, float animationDuration = 1f)
        {
            MoveTo(initialPosition, finalPosition, animationDuration);
        }
        #endregion
    }
}
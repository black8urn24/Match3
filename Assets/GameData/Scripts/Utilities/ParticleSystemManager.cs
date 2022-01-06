using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Enums;

namespace Match3.Utilities
{
    public class ParticleSystemManager : MonoBehaviour
    {
        #region Inspector Variables
        #endregion

        #region Variables
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Private Methods
        #endregion

        #region Coroutines
        private IEnumerator DisableEffectsWithDelay(GameObject poolObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (poolObject != null)
            {
                poolObject.SetActive(false);
            }
        }
        #endregion

        #region Public Methods
        public void PlayClearPieceEffect(int x, int y, int z = 0)
        {
            if (ObjectPoolManager.instance != null)
            {
                GameObject clearPieceFx = ObjectPoolManager.instance.GetPoolObject(PoolObjectsType.PieceClearEffect);
                if (clearPieceFx != null)
                {
                    clearPieceFx.SetActive(true);
                    clearPieceFx.transform.position = new Vector3(x, y, z);
                    ParticlePlayer particlePlayer = clearPieceFx.GetComponent<ParticlePlayer>();
                    if (particlePlayer != null)
                    {
                        particlePlayer.PlayParticles();
                    }
                    StartCoroutine(DisableEffectsWithDelay(clearPieceFx, 1f));
                }
            }
        }

        public void PlayBreakableTilesEffect(int breakCount, int x, int y, int z = 0)
        {
            GameObject breakFx = null;
            ParticlePlayer particlePlayer = null;
            if (breakCount > 1)
            {
                if (ObjectPoolManager.instance != null)
                {
                    breakFx = ObjectPoolManager.instance.GetPoolObject(PoolObjectsType.SingleBreakableTileEffect);
                }
            }
            else
            {
                if (ObjectPoolManager.instance != null)
                {
                    breakFx = ObjectPoolManager.instance.GetPoolObject(PoolObjectsType.DoubleBreakableTileEffect);
                }
            }
            if (breakFx != null)
            {
                breakFx.SetActive(true);
                breakFx.transform.position = new Vector3(x, y, z);
                particlePlayer = breakFx.GetComponent<ParticlePlayer>();
                if (particlePlayer != null)
                {
                    particlePlayer.PlayParticles();
                }
                StartCoroutine(DisableEffectsWithDelay(breakFx, 1f));
            }
        }
        #endregion
    }
}
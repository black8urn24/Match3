using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Utilities
{
    public class ParticleSystemManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private GameObject clearPieceEffect = null;
        [SerializeField] private GameObject breakablePieceEffect = null;
        [SerializeField] private GameObject doubleBreakablePieceEffect = null;
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

        #region Public Methods
        public void PlayClearPieceEffect(int x, int y, int z = 0)
        {
            if(clearPieceEffect != null)
            {
                GameObject clearPieceFx = Instantiate(clearPieceEffect, new Vector3(x, y, z), Quaternion.identity);
                ParticlePlayer particlePlayer = clearPieceFx.GetComponent<ParticlePlayer>();
                if(particlePlayer != null)
                {
                    particlePlayer.PlayParticles();
                }
            }
        }

        public void PlayBreakableTilesEffect(int breakCount, int x, int y, int z = 0)
        {
            GameObject breakFx = null;
            ParticlePlayer particlePlayer = null;
            if(breakCount > 1)
            {
                if(doubleBreakablePieceEffect != null)
                {
                    breakFx = Instantiate(doubleBreakablePieceEffect, new Vector3(x, y, z), Quaternion.identity);
                }
            }
            else
            {
                if(breakablePieceEffect != null)
                {
                    breakFx = Instantiate(breakablePieceEffect, new Vector3(x, y, z), Quaternion.identity);
                }
            }
            if (breakFx != null)
            {
                particlePlayer = breakFx.GetComponent<ParticlePlayer>();
                if (particlePlayer != null)
                {
                    particlePlayer.PlayParticles();
                }
            }
        }
        #endregion
    }
}
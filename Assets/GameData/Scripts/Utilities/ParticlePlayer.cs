using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Utilities
{
    public class ParticlePlayer : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private ParticleSystem[] particles = null;
        #endregion

        #region Variables
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Public Methods
        public void PlayParticles()
        {
            if(particles != null)
            {
                foreach(var item in particles)
                {
                    item.Stop();
                    item.Play();
                }
            }
        }
        #endregion
    }
}
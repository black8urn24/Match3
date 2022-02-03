using Match3.Enums;
using Match3.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class AudioManager : Singleton<AudioManager>
    {
        #region Inspector Variables
        [Header("Clips")]
        [SerializeField] private List<AudioClip> musicClips = null;
        [SerializeField] private List<AudioClip> winClips = null;
        [SerializeField] private List<AudioClip> looseClips = null;
        [SerializeField] private List<AudioClip> bonusClips = null;
        [Header("Clip properties")]
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.5f;
        [SerializeField] [Range(0f, 1.15f)] private float maxPitch = 1f;
        [SerializeField] [Range(0f, 1f)] private float minPitch = 0.5f;
        #endregion

        #region Variables
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            PlayRandomMusic();
        }
        #endregion

        #region Private Methods
        private AudioClip PickRandomClip(List<AudioClip> audioClips)
        {
            if (audioClips != null)
            {
                int randomIndex = Random.Range(0, audioClips.Count);
                return audioClips[randomIndex];
            }
            return null;
        }

        private void PlayClip(AudioClip audioClip, float volume, PoolObjectsType poolObjectsType = PoolObjectsType.None, bool loop = false)
        {
            GameObject targetSource = null;
            if (ObjectPoolManager.instance != null)
            {
                AudioSource targetAudioSource = null;
                targetSource = ObjectPoolManager.instance.GetPoolObject(poolObjectsType);
                if (targetSource != null)
                {
                    targetAudioSource = targetSource.GetComponent<AudioSource>();
                    if (targetAudioSource != null)
                    {
                        targetSource.SetActive(true);
                        targetAudioSource.clip = audioClip;
                        float randomPitch = Random.Range(minPitch, maxPitch);
                        targetAudioSource.pitch = randomPitch;
                        targetAudioSource.volume = volume;
                        targetAudioSource.loop = loop;
                        targetAudioSource.Play();
                        if (loop == false)
                        {
                            float delay = audioClip.length;
                            StartCoroutine(ReturnObjectsToPool(targetSource, delay));
                        }
                    }
                }
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator ReturnObjectsToPool(GameObject poolObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (poolObject != null)
            {
                poolObject.SetActive(false);
            }
        }
        #endregion

        #region Public Methods
        public void PlayRandomMusic()
        {
            AudioClip musicClip = PickRandomClip(musicClips);
            if (musicClip != null)
            {
                PlayClip(musicClip, musicVolume, PoolObjectsType.MusicAudioSource, true);
            }
        }

        public void PlayRandomWinClips()
        {
            AudioClip winClip = PickRandomClip(winClips);
            if (winClip != null)
            {
                PlayClip(winClip, sfxVolume, PoolObjectsType.WinAudioSource, false);
            }
        }

        public void PlayRandomLooseClips()
        {
            AudioClip looseClip = PickRandomClip(looseClips);
            if (looseClip != null)
            {
                PlayClip(looseClip, 0.5f, PoolObjectsType.LooseAudioSource, false);
            }
        }

        public void PlayRandomBonusClips()
        {
            AudioClip bonusClip = PickRandomClip(bonusClips);
            if (bonusClip != null)
            {
                PlayClip(bonusClip, sfxVolume, PoolObjectsType.BonusAudioSource, false);
            }
        }

        public void PlayPieceDestroyClip(AudioClip audioClip, PoolObjectsType poolObjectsType = PoolObjectsType.None, bool loop = false)
        {
            PlayClip(audioClip, sfxVolume, poolObjectsType, loop);
        }

        public void PlaySingleClip(AudioClip audioClip, PoolObjectsType poolObjectsType = PoolObjectsType.None, bool loop = false)
        {
            PlayClip(audioClip, sfxVolume, poolObjectsType, loop);
        }
        #endregion
    }
}
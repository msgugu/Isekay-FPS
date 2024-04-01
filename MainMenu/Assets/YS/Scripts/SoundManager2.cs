using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YS.SoundManager
{
    public class Sound
    {
        public string name; // Bgm 이름
        public AudioClip clip;
    }
    
    
    public class SoundManager : MonoBehaviour
    {
        static public SoundManager instance { get; private set; }
    
        #region Singleton
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
    
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        #endregion
    
        public AudioClip[] ARShootSounds;
        public AudioClip[] SRShootSounds;
        public AudioClip[] footStepSounds;
    
        public AudioSource audioSource;
    
        public void ARShootSound()
        {
            PlayRandomSound(ARShootSounds);
        }
    
        public void SRShootSound()
        {
            PlayRandomSound(SRShootSounds);
        }
    
        public void PlayeFootStepSound()
        {
            PlayRandomSound(footStepSounds);
        }
    
    
        private void PlayRandomSound(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return;
            }
    
            int randomIndex = Random.Range(0, clips.Length);
            audioSource.PlayOneShot(clips[randomIndex]);
        }
    
    }
    
    //public AudioSource[] audioSourceEffect;
    //public AudioSource audioSourceBgm;
    //
    //public Sound[] effectSound;
    //public Sound[] bgmSound;
    
    //public void PlaySound(AudioClip clip)
    //{
    //    if (clip != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //}

}
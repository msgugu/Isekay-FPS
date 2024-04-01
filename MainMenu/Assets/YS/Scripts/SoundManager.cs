using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;


public class SoundManager : MonoBehaviour
{
    // 총의 종류와 사운드 클립 배열을 저장하는 곳
    public Dictionary<string, AudioClip[]> gunSounds;

    [System.Serializable]
    public struct GunSound // 총 타입, 소리 배열
    {
        public string gunType;
        public AudioClip[] clips;
    }

    [System.Serializable]
    public struct FootstepSound // 지형 타입, 걷기 뛰기 소리 배열
    {
        public string surfaceType;
        public AudioClip[] walkingFootstepSounds; // 걷는 소리 클립 배열
        public AudioClip[] runningFootstepSounds; // 달리는 소리 클립 배열
    }

    public AudioClip reloadSound; // 장전 소리
    public AudioClip fireModeChangeSound; // 사격 모드 변경 사운드
    public AudioClip[] emptyMagazineSounds; // 탄이 모두 소모 되었을때 마지막 텅 하는 소리
    public AudioClip[] noAmmoSounds; // 탄이 모두 비었는데 계속 쏘려고 하면 나는 소리

    public GunSound[] gunSoundArray; // 여러가지 총들을 위한 사격 소리 배열
    public FootstepSound[] footstepSounds; // 여러가지 지형에 대한 발 소리 배열

    #region Singleton
    static public SoundManager instance;
    //private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            gunSounds = new Dictionary<string, AudioClip[]>();

            foreach(var gunSound in gunSoundArray)
            {
                gunSounds[gunSound.gunType] = gunSound.clips;
            }
        }
        else
        {
            Destroy(gameObject);
        }

        //audioSource = GetComponent<AudioSource>();
        //if (audioSource == null)
        //{
        //    audioSource = gameObject.AddComponent<AudioSource>();
        //}
    }
    #endregion


    /// <summary>
    /// 총 소리 배열에 있는 클립을 랜덤 재생
    /// </summary>
    /// <param name="gunType"> 총기 타입 ex) AR, SR </param>
    /// <param name="position"> 총기 오브젝트 위치 (소리가 나올 위치) </param>
    public void PlayRandomSound(string gunType, Vector3 position)
    {
        if (!gunSounds.ContainsKey(gunType))
        {
            return;
        }

        AudioClip[] clips = gunSounds[gunType];
        if (clips.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        AudioClip clip = clips[randomIndex];

        // 동적 AudioSource 생성 및 사운드 재생
        GameObject soundObject = new GameObject("SoundObject_" + gunType);
        soundObject.transform.position = position; // 사운드 재생 위치 설정
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1.0f; // 3D 사운드로 설정
        audioSource.minDistance = 10.0f; // 최소 거리
        audioSource.maxDistance = 50.0f; // 최대 거리

        audioSource.clip = clip;
        audioSource.Play();

        Destroy(soundObject, clip.length); // 사운드 재생이 끝나면 오브젝트 파괴
    }

    /// <summary>
    /// 총 장전 소리
    /// </summary>
    /// <param name="position"> 오브젝트의 위치 </param>
    public void PlayReloadSound(Vector3 position)
    {
        if (reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(reloadSound, position);
        }
    }

    // 사격 하다가 마지막에 탄이 없어서 팅 하고 걸리는 소리
    public void PlayEmptyMagazineSound(Vector3 position)
    {
        PlayRandomSoundArray(emptyMagazineSounds, position);
    }

    // 더 이상 남은 탄약이 없어서 쏘려고 시도 할 때 마다 틱틱 거리는 소리
    public void PlayNoAmmoSound(Vector3 position)
    {
        PlayRandomSoundArray(noAmmoSounds, position);
    }

    // 배열로 오디오클립이 저장 되어있는 탄이 없을때 나오는 소리를 랜덤 재생
    void PlayRandomSoundArray(AudioClip[] soundArray, Vector3 position)
    {
        if (soundArray.Length == 0) return;

        int randomIndex = Random.Range(0, soundArray.Length);
        AudioClip clip = soundArray[randomIndex];
        AudioSource.PlayClipAtPoint(clip, position);
    }

    // 사격 모드 변경 사운드
    public void PlayFireModeChangeSound(Vector3 position)
    {
        if(fireModeChangeSound != null)
        {
            AudioSource.PlayClipAtPoint(fireModeChangeSound, position);
        }
    }
}
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static SoundManager;


/// <summary>
/// 사운드 관리자
/// </summary>
public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// 총의 종류와 사운드 클립 배열을 저장하는 곳
    /// </summary>
    public Dictionary<string, AudioClip[]> gunSounds;

    [System.Serializable]
    /// 총 타입, 소리 배열
    public struct GunSound
    { 
        /// <summary>
        /// 총 타입
        /// </summary>
        public string gunType;

        /// <summary>
        /// 총기 소리 클립 배열
        /// </summary>
        public AudioClip[] clips;
    }

    [System.Serializable]
    /// 지형 타입, 걷기 뛰기 소리 배열
    public struct FootstepSound
    {
        /// <summary>
        /// 지형 타입
        /// </summary>
        public string surfaceType;

        /// <summary>
        /// 걷는 소리 클립 배열
        /// </summary>
        public AudioClip[] walkingFootstepSounds;

        /// <summary>
        /// 달리는 소리 클립 배열
        /// </summary>
        public AudioClip[] runningFootstepSounds;
    }

    /// <summary>
    /// 장전 소리
    /// </summary>
    public AudioClip reloadSound;

    /// <summary>
    /// 사격 모드 변경 사운드
    /// </summary>
    public AudioClip fireModeChangeSound;

    /// <summary>
    /// 탄이 모두 소모 되었을때 마지막 팅 하는 소리
    /// </summary>
    public AudioClip[] emptyMagazineSounds;

    /// <summary>
    /// 탄이 모두 비었는데 계속 쏘려고 하면 나는 소리
    /// </summary>
    public AudioClip[] noAmmoSounds;

    /// <summary>
    /// 총 타입, 사운드의 정보 배열
    /// </summary>
    public GunSound[] gunSoundArray;

    /// <summary>
    /// 지형 타입, 발걸음 사운드 정보 배열
    /// </summary>
    public FootstepSound[] footstepSounds;

    #region Singleton
    /// <summary>
    /// 어디서든 사용 할 수 있게 인스턴스화 시킨다
    /// </summary>
    static public SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 씬이 바뀌어도 파괴되지 않게 함
            DontDestroyOnLoad(gameObject);

            // 초기화 하고 정보 다시 추가
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
    }
    #endregion


    /// <summary>
    /// 총 소리 랜덤 재생
    /// </summary>
    /// <param name="gunType"> 총기 타입 ex) AR, SR </param>
    /// <param name="position"> 총기 오브젝트 위치 (오디오가 재생될 위치) </param>
    public void PlayRandomSound(string gunType, Vector3 position)
    {
        // 총기 타입이 없으면 종료
        if (!gunSounds.ContainsKey(gunType)) return;

        AudioClip[] clips = gunSounds[gunType];
        
        // 오디오 클립이 없으면 종료
        if (clips.Length == 0) return;

        // 랜덤으로 재생
        int randomIndex = Random.Range(0, clips.Length);
        AudioClip clip = clips[randomIndex];

        // 동적 AudioSource 생성 및 사운드 재생
        GameObject soundObject = new GameObject("SoundObject_" + gunType);
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        // 오디오소스 컴포넌트 인스펙터 설정 값
        audioSource.spatialBlend = 1.0f; // 3D 사운드로 설정
        audioSource.minDistance = 10.0f; // 최소 거리
        audioSource.maxDistance = 50.0f; // 최대 거리

        audioSource.clip = clip;
        audioSource.Play();

        // 사운드 재생이 끝나면 오브젝트 파괴
        Destroy(soundObject, clip.length);
    }

    /// <summary>
    /// 총 장전 소리
    /// </summary>
    /// <param name="position"> 오디오 재생 위치 </param>
    public void PlayReloadSound(Vector3 position)
    {
        if (reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(reloadSound, position);
        }
    }

    /// <summary>
    /// 사격 하다가 마지막에 탄이 없어서 팅 하고 걸리는 소리
    /// </summary>
    /// <param name="position"> 사운드 재생 위치 </param>
    public void PlayEmptyMagazineSound(Vector3 position)
    {
        PlayRandomSoundArray(emptyMagazineSounds, position);
    }

    /// <summary>
    /// 더 이상 남은 탄약이 없어서 쏘려고 시도 할 때 마다 틱틱 거리는 소리
    /// </summary>
    /// <param name="position"> 사운드 재생 위치 </param>
    public void PlayNoAmmoSound(Vector3 position)
    {
        PlayRandomSoundArray(noAmmoSounds, position);
    }

    /// <summary>
    /// 배열로 오디오클립이 저장 되어있는 탄이 없을때 나오는 소리를 랜덤 재생
    /// </summary>
    /// <param name="soundArray"> 오디오클립 배열 </param>
    /// <param name="position"> 사운드 재생 위치 </param>
    void PlayRandomSoundArray(AudioClip[] soundArray, Vector3 position)
    {
        // 배열이 없으면 종료
        if (soundArray.Length == 0) return;

        // 랜덤 함수를 적용해서 0 ~ 배열의 크기 만큼
        int randomIndex = Random.Range(0, soundArray.Length);
        AudioClip clip = soundArray[randomIndex];
        AudioSource.PlayClipAtPoint(clip, position);
    }

    /// <summary>
    /// 사격 모드 변경 사운드
    /// </summary>
    /// <param name="position"> 사운드 재생 위치 </param>
    public void PlayFireModeChangeSound(Vector3 position)
    {
        if(fireModeChangeSound != null)
        {
            AudioSource.PlayClipAtPoint(fireModeChangeSound, position);
        }
    }
}

//private AudioSource audioSource;
//audioSource = GetComponent<AudioSource>();
//if (audioSource == null)
//{
//    audioSource = gameObject.AddComponent<AudioSource>();
//}
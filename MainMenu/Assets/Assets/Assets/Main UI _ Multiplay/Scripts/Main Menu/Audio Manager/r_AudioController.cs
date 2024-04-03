using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 버튼 클릭 소리와 같은 오디오 이펙트를 재생하는 데 사용
public class r_AudioController : MonoBehaviour
{
    public static r_AudioController instance;  // SingleTone instance

    // 오디오 재생담당
    [Header("Audio Source")]
    public AudioSource m_AudioSource; 

    // 클릭소리에 사용될 오디오 클립
    [Header("Audio Clip")]
    public AudioClip m_ClickSound;


    
    private void Awake()
    {
        // Singleton pattern구현 (단일 인스턴스만 사용)
        if (instance)
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    /// <summary>
    /// 클릭 소리를 재생 (단, 오디오 소스 및 클립이 존해하는 경우만)
    /// </summary>
    public void PlayClickSound()
    {
        if (m_AudioSource != null && m_ClickSound != null)
            m_AudioSource.PlayOneShot(m_ClickSound);  // 오디오 소스를 통해 클릭 사운드를 한 번 재생
    }

}

using UnityEngine;

namespace InGame.UI
{
    public class UIAudioSource : MonoBehaviour
    {     
        // UIAudioSource의 인스턴스를 담는 정적 변수
        private static UIAudioSource m_Instance;

        // UIAudioSource의 인스턴스에 접근하기 위한 속성
        public static UIAudioSource Instance { get { return m_Instance; } }
  

        // 오디오 볼륨을 조절하는 변수, UI인스펙터에서 수정가능
        [SerializeField]
        [Range(0f, 1f)]
        private float m_Volume = 1f;

        /// <summary>
        /// 볼륨 크기를 설정하는 속성
        /// </summary>
        public float volume { get { return this.m_Volume; } set { this.m_Volume = value; } }

        private AudioSource m_AudioSource;

        // MonoBehaviour의 Awake 메서드를 재정의
        protected void Awake()
        {
            // 이미 UIAudioSource의 인스턴스가 존재하는지 확인
            // => 싱글톤 패턴 단일 인스턴스만 유지하도록하여 오디오가 겹치는 현상을 방지한다.
            if (m_Instance != null)
            {
                // 경고 메시지 출력하고 Awake 메서드 종료
                Debug.LogWarning("두 개 이상의 UIAudioSource(UIAudio 소스)가 씬에 있습니다. 하나만 있는지 확인하십시오");
                return; // 종료
            }

            // 현재 UIAudioSource 인스턴스를 설정
            m_Instance = this;

            // 현재 GameObject에 부착된 AudioSource 컴포넌트 참조
            this.m_AudioSource = this.gameObject.GetComponent<AudioSource>();

            // Awake 시에 오디오가 자동으로 재생되지 않도록 설정
            this.m_AudioSource.playOnAwake = false;
        }

        /// <summary>
        /// 주어진 AudioClip을 재생하는 메서드
        /// </summary>
        /// <param name="clip"></param>
        public void PlayAudio(AudioClip clip)
        {
            // 지정된 볼륨으로 AudioClip을 재생
            this.m_AudioSource.PlayOneShot(clip, this.m_Volume);
        }

        // 주어진 AudioClip을, 지정된 볼륨(default volume)으로 재생하는 메서드
        public void PlayAudio(AudioClip clip, float volume)
        {
            // 지정된 볼륨으로 AudioClip을 재생
            // 지정된 볼륨에 현재 설정된 UI 볼륨을 곱하여 최종 볼륨 계산
            this.m_AudioSource.PlayOneShot(clip, this.m_Volume * volume);
        }
    }
}

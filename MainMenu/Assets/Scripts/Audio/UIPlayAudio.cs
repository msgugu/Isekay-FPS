using UnityEngine;
using UnityEngine.EventSystems; // UI 이벤트 처리를 위한 네임스페이스

namespace InGame.UI
{
    // 컴포넌트 메뉴에 추가되어 인스펙터에서 쉽게 추가
    [AddComponentMenu("UI/Audio/Play Audio")]
    public class UIPlayAudio : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        // 이벤트 타입을 정의하는 열거형. 오디오 재생을 트리거할 다양한 마우스 이벤트를 포함
        public enum Event
        {
            None,           // 오디오를 재생하지 않음
            PointerEnter,   // 마우스 포인터가 요소 위로 들어올 때
            PointerExit,    // 마우스 포인터가 요소에서 나갈 때
            PointerDown,    // 마우스 버튼을 누를 때
            PointerUp,      // 마우스 버튼을 떼었을 때
            Click,          // 클릭 이벤트 시
            DoubleClick     // 더블 클릭 이벤트 시
        }

        // 인스펙터에서 설정할 수 있는 필드
        [SerializeField] private AudioClip m_AudioClip; // 재생할 오디오 클립
        [SerializeField][Range(0f, 1f)] private float m_Volume = 1f; // 오디오 볼륨 (0에서 1 사이)
        [SerializeField] private Event m_PlayOnEvent = Event.None; // 오디오를 재생할 이벤트

        // 오디오 클립에 대한 getter와 setter
        public AudioClip audioClip { get { return this.m_AudioClip; } set { this.m_AudioClip = value; } }

        // 볼륨에 대한 getter와 setter
        public float volume { get { return this.m_Volume; } set { this.m_Volume = value; } }

        // 재생할 이벤트에 대한 getter와 setter
        public Event playOnEvent { get { return this.m_PlayOnEvent; } set { this.m_PlayOnEvent = value; } }

        private bool m_Pressed = false; // 마우스 버튼이 눌린 상태를 추적하는 플래그

        // 이벤트 핸들러 구현
        // (마우스 포인터가 UI 요소 위로 들어왔을 때) 마우스 버튼이 눌리지 않았다면 PointerEnter 이벤트를 트리거
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.m_Pressed)
                this.TriggerEvent(Event.PointerEnter);
        }

        // (마우스 포인터가 UI 요소에서 나갔을 때) 마우스 버튼이 눌리지 않았다면 PointerExit 이벤트를 트리거
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!this.m_Pressed)
                this.TriggerEvent(Event.PointerExit);
        }

        // 이 메소드는 왼쪽 마우스 버튼의 클릭만을 처리한다.
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return; // 왼쪽 마우스 버튼 클릭만 처리

            // 왼쪽 마우스 버튼이 눌렸을 때 PointerDown 이벤트를 트리거
            this.TriggerEvent(Event.PointerDown);
            this.m_Pressed = true; // 마우스 버튼이 눌렸음을 표시
        }

        // 마우스 버튼을 떼었을 때 호출되는 메소드
        public void OnPointerUp(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼의 입력만 처리
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // PointerUp 이벤트 트리거
            this.TriggerEvent(Event.PointerUp);

            // 만약 마우스 버튼이 눌린 상태였다면
            if (this.m_Pressed)
            {
                // 만약 클릭 카운트가 1보다 크다면 (더블 클릭이라면)
                if (eventData.clickCount > 1)
                {
                    // DoubleClick 이벤트 트리거
                    this.TriggerEvent(Event.DoubleClick);
                    // 클릭 카운트 초기화
                    eventData.clickCount = 0;
                }
                else
                {
                    // 단일 클릭일 경우 Click 이벤트 트리거
                    this.TriggerEvent(Event.Click);
                }
            }

            // 마우스 버튼이 눌리지 않은 상태로 설정
            this.m_Pressed = false;
        }

        // 지정된 이벤트가 발생했을 때 오디오를 재생할지 결정하는 메소드
        private void TriggerEvent(Event e)
        {
            // 설정된 이벤트와 발생한 이벤트가 같다면 오디오 재생
            if (e == this.m_PlayOnEvent)
            {
                this.PlayAudio();
            }
        }

        // 오디오를 재생하는 메소드
        public void PlayAudio()
        {
            // 컴포넌트나 게임 오브젝트가 비활성화 상태라면 오디오 재생을 하지 않음
            if (!this.enabled || !this.gameObject.activeInHierarchy)
            {
                return;
            }

            // 오디오 클립이 설정되지 않았다면 오디오 재생을 하지 않음
            if (this.m_AudioClip == null)
            {
                return;
            }

            // UIAudioSource의 인스턴스가 존재하지 않는다면 경고 로그를 출력하고 오디오 재생을 하지 않음
            if (UIAudioSource.Instance == null)
            {
                Debug.LogWarning("You dont have UIAudioSource in your scene. Cannot play audio clip.");
                return;
            }

            // 설정된 오디오 클립과 볼륨으로 오디오를 재생
            UIAudioSource.Instance.PlayAudio(this.m_AudioClip, this.m_Volume);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 이벤트 처리를 위한 네임스페이스

namespace InGame.UI
{
    // Selectable 클래스를 상속받아 UI 요소가 선택될 수 있게 하며, ISubmitHandler 인터페이스를 구현하여 제출 이벤트를 처리
    public class Test_OptionRow : Selectable, ISubmitHandler
    {
        // 컴파일러 경고를 일시적으로 비활성화 (미사용 변수에 대한 경고 등)
#pragma warning disable 0649
        [Header("Option Properties")] // 인스펙터에서 구분을 위한 헤더
        [SerializeField] private Selectable m_Target; // 이 옵션 UI가 제출될 때 동작을 위임할 대상
        [SerializeField] private GameObject m_Description; // 옵션 설명을 표시할 GameObject
#pragma warning restore 0649

        // Awake는 객체가 생성될 때 호출되는 Unity 생명주기 메서드
        protected override void Awake()
        {
            base.Awake(); // 부모 클래스의 Awake 메서드 호출

            // m_Description이 설정되어 있다면 비활성화 상태로 초기화
            if (this.m_Description != null)
                this.m_Description.SetActive(false);
        }

        // OnSubmit은 이 UI 요소에 대한 제출(Enter 키 또는 클릭 등) 이벤트가 발생했을 때 호출
        public virtual void OnSubmit(BaseEventData eventData)
        {
            // m_Target이 null이 아닌 경우만 처리
            if (this.m_Target == null)
                return;

            // m_Target이 ISubmitHandler 인터페이스를 구현하고 있다면, 제출 이벤트를 m_Target에 위임
            if (this.m_Target is ISubmitHandler)
                (this.m_Target as ISubmitHandler).OnSubmit(eventData);
        }

        // 특정 플랫폼(에디터, 스탠드얼론, WebGL)에서만 동작하는 코드
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // 마우스 포인터가 UI 요소에 들어올 때 호출
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData); // 부모 클래스의 처리를 호출

            // m_Description이 설정되어 있다면 활성화 상태로 변경하여 설명을 표시
            if (this.m_Description != null)
                this.m_Description.SetActive(true);
        }

        // 마우스 포인터가 UI 요소에서 나갈 때 호출
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData); // 부모 클래스의 처리를 호출

            // m_Description이 설정되어 있다면 비활성화 상태로 변경하여 설명을 숨김
            if (this.m_Description != null)
                this.m_Description.SetActive(false);
        }
#else
        // 터치 기반 인터페이스에서 UI 요소가 선택될 때 호출
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData); // 부모 클래스의 처리를 호출

            // m_Description이 설정되어 있다면 활성화 상태로 변경
            if (this.m_Description != null)
                this.m_Description.SetActive(true);
        }

        // UI 요소가 선택 해제될 때 호출
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData); // 부모 클래스의 처리를 호출

            // m_Description이 설정되어 있다면 비활성화 상태로 변경
            if (this.m_Description != null)
                this.m_Description.SetActive(false);
        }
#endif
    }
}
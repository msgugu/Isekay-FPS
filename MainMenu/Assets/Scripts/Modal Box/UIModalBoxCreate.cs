using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace InGame.UI
{
    public class UIModalBoxCreate : MonoBehaviour
    {
        // 모달 박스에 표시될 텍스트 내용들
        [SerializeField] private string m_Text1;        // 첫 번째 텍스트
        [SerializeField] private string m_Text2;        // 두 번째 텍스트
        [SerializeField] private string m_ConfirmText;  // 확인 버튼 텍스트
        [SerializeField] private string m_CancelText;   // 취소 버튼 텍스트

        [SerializeField] private Button m_HookToButton; // 모달박스를 생성할 때 사용할 버튼

        // 확인버튼 클릭 시 발동할 Unity 이벤트
        public UnityEvent onConfirm = new UnityEvent();

        // 취보버튼 클릭 시 발동할 Unity 이벤트
        public UnityEvent onCancel = new UnityEvent();

        // 컴포넌트가 활성화되면 실행되는 메서드입니다.
        protected void OnEnable()
        {
            // 할당된 버튼에 CreateAndShow 메서드를 클릭 이벤트 리스너로 추가
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.AddListener(CreateAndShow);
        }

        // 컴포넌트가 비활성화될 때 실행되는 메서드
        protected void OnDisable()
        {
            // 할당된 버튼에서 CreateAndShow 메서드를 클릭 이벤트 리스너에서 제거
            if (this.m_HookToButton != null)
                this.m_HookToButton.onClick.RemoveListener(CreateAndShow);
        }

        // 버튼 클릭 시 호출되어 모달 박스를 생성 및 표시하는 메서드
        // 설정된 텍스트와 이벤트 리스너를 추가
        public void CreateAndShow()
        {
            // UIModalBoxManager를 사용하여 모달 박스 인스턴스를 생성
            UIModalBox box = UIModalBoxManager.Instance.Create(this.gameObject);

            // 모달 박스에 설정된 텍스트들을 지정
            box.SetText1(this.m_Text1);
            box.SetText2(this.m_Text2);

            // 모달 박스의 확인, 취소 버튼 텍스트를 설정
            box.SetConfirmButtonText(this.m_ConfirmText);
            box.SetCancelButtonText(this.m_CancelText);

            // 확인, 취소 버튼 클릭 시 실행할 이벤트를 모달 박스에 연결
            box.onConfirm.AddListener(OnConfirm);
            box.onCancel.AddListener(OnCancel);

            // 모달 박스를 표
            box.Show();
        }

        // 확인버튼 클릭 시 호출되는 메서드
        public void OnConfirm()
        {
            if (this.onConfirm != null)
            {
                this.onConfirm.Invoke();
            }
        }
        // 취소버튼 클릭 시 호출되는 메서드
        public void OnCancel()
        {
            if (this.onCancel != null)
            {
                this.onCancel.Invoke();
            }
        }
    }
}

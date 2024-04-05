using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace InGame.UI
{
    public class UIModalBox : MonoBehaviour
    {
        [SerializeField] private Text m_Text1;                      // 첫 번째 텍스트
        [SerializeField] private Text m_Text2;                      // 두 번째 줄 텍스트

        [SerializeField] private Button m_ConfirmButton;             // 확인 버튼
        [SerializeField] private Text m_ConfirmButtonText;           // 확인 버튼의 텍스트
        [SerializeField] private Button m_CancelButton;              // 취소 버튼
        [SerializeField] private Text m_CancelButtonText;            // 취소 버튼의 텍스트


        private UIWindow m_Window;                                   // ModalBox를 포함하는 UIWindow 컴포넌트
        private bool m_IsActive = false;                             // ModalBox의 활성화 상태

        // ModalBox의 확인 및 취소에 대한 이벤트를 정의
        public UnityEvent onConfirm = new UnityEvent(); // 확인 이벤트
        public UnityEvent onCancel = new UnityEvent();  // 취소 이벤트

        [SerializeField] private string m_ConfirmInput = "Submit"; // 확인 액션에 대응하는 입력 이름
        [SerializeField] private string m_CancelInput = "Cancel"; // 취소 액션에 대응하는 입력 이름


        /// <summary>
        /// ModalBox가 활성화되어 있는지를 반환하는 속성 
        /// </summary>
        public bool isActive
        {
            get { return this.m_IsActive; }
        }

        protected void Awake()
        {
            // UIWindow 컴포넌트를 찾아 할당 => ModalBox의 표시 및 숨김을 관리
            if (this.m_Window == null)
            {
                this.m_Window = this.gameObject.GetComponent<UIWindow>();
            }

            // UIWindow의 이스케이프 키 동작을 비활성화
            this.m_Window.escapeKeyAction = UIWindow.EscapeKeyAction.None;

            // UIWindow의 전환 완료 이벤트에 리스너를 등록
            this.m_Window.onTransitionComplete.AddListener(OnWindowTransitionEnd);

            // 확인 및 취소 버튼에 대한 클릭 이벤트 리스너를 등록
            if (this.m_ConfirmButton != null)
            {
                this.m_ConfirmButton.onClick.AddListener(Confirm);
            }

            if (this.m_CancelButton != null)
            {
                this.m_CancelButton.onClick.AddListener(Close);
            }
        }

        protected void Update()
        {
            // 키보드 입력을 감지하여 모달 박스를 닫거나 확인 동작을 수행
            if (!string.IsNullOrEmpty(this.m_CancelInput) && Input.GetButtonDown(this.m_CancelInput))
                this.Close();
            if (!string.IsNullOrEmpty(this.m_ConfirmInput) && Input.GetButtonDown(this.m_ConfirmInput))
                this.Confirm();
        }

        /// <summary>
        /// 첫 번째 텍스트를 설정하는 메서드
        /// </summary>
        /// <param name="text"></param>
        public void SetText1(string text)
        {
            if (this.m_Text1 != null)
            {
                this.m_Text1.text = text;
                this.m_Text1.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
        }

        /// <summary>
        /// 두 번째 텍스트를 설정하는 메서드
        /// </summary>
        /// <param name="text"></param>
        public void SetText2(string text)
        {
            if (this.m_Text2 != null)
            {
                this.m_Text2.text = text;
                this.m_Text2.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
        }

        /// <summary>
        /// ModalBox의 확인버튼 텍스트를 설정하는 메서드
        /// </summary>
        /// <param name="text">The confirm button text.</param>
        public void SetConfirmButtonText(string text)
        {
            if (this.m_ConfirmButtonText != null)
            {
                this.m_ConfirmButtonText.text = text;
            }
        }

        /// <summary>
        /// ModalBox의 취소버튼 텍스트를 성정하는 메서드
        /// </summary>
        /// <param name="text">The cancel button text.</param>
        public void SetCancelButtonText(string text)
        {
            if (this.m_CancelButtonText != null)
            {
                this.m_CancelButtonText.text = text;
            }
        }

        /// <summary>
        /// 모달 박스를 표시하는 메서드
        /// </summary>
        public void Show()
        {
            this.m_IsActive = true;

            if (this.m_Window != null)
            {
                this.m_Window.Show();
            }
        }

        /// <summary>
        /// 모달 박스를 닫는 메서드
        /// </summary>
        public void Close()
        {
            this.m_IsActive = false;

            // 모달이 포함된 UIWindow 컴포넌트를 사용해 ModalBox를 숨김
            if (this.m_Window != null)
            {
                this.m_Window.Hide();
            }

            // 취소 이벤트를 호출
            // 최소버튼 클릭시 필요한 추가 동작을 정의
            if (this.onCancel != null)
            {
                this.onCancel.Invoke();
            }
        }

        // 확인 버튼을 클릭했을 때 실행되는 메서드
        public void Confirm()
        {
            this.m_IsActive = false;

            // 모달이 포함된 UIWindow 컴포넌트를 사용해 ModalBox를 숨김
            if (this.m_Window != null)
            {
                this.m_Window.Hide();
            }

            // 확인 이벤트를 호출
            // 확인 버튼 클릭 시 필요한 추가 동작을 정의
            if (this.onConfirm != null)
            {
                this.onConfirm.Invoke();
            }
        }

        // UIWindow의 전환(표시 또는 숨김)이 완료되었을 때 실행되는 콜백 메서드
        public void OnWindowTransitionEnd(UIWindow window, UIWindow.VisualState state)
        {
            // ModalBox가 숨겨진 상태일 때 ModalBox를 파괴
            // ModalBox가 사용이 완료된 후 불필요한 리소스를 정리
            if (state == UIWindow.VisualState.Hidden)
            {
               // Destroy(this.gameObject);
            }
        }
    }
}

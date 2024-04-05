using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;  // 게임이 에디터에서 실행 중일 때만 필요함
#endif

namespace InGame.UI
{
    public class Test_ExitButton : MonoBehaviour
    {
        [SerializeField] private Button m_Button;           // 인스펙터에서 설정할 수 있는 Button 변수설정
        [SerializeField] private bool m_autoHook = true;    // 버튼의 onClick 이벤트에 자동으로 ExitGame 메소드를 연결할지 결정하는 flag

        protected void Awake()
        {
            if (this.m_Button == null)                                   // m_Button이 인스펙터에서 설정되지 않았다면
                this.m_Button = this.gameObject.GetComponent<Button>();  // 현재 GameObject에서 Button 컴포넌트를 자동으로 찾아 할당 
        }

        // GameObject가 활성화될 때마다 호출
        protected void OnEnable()
        {
            if (this.m_Button != null && this.m_autoHook)     // m_Button이 존재하고 자동 연결이 활성화되어 있다면
            {
                {
                    this.m_Button.onClick.AddListener(ExitGame);  // ExitGame 메소드를 버튼의 onClick 이벤트에 리스너로 추가
                }
            }
        }

        // GameObject가 비활성화될 때마다 호출
        protected void OnDisable()
        {
            if (this.m_Button != null && this.m_autoHook)        // m_Button이 존재하고, 자동 연결이 활성화되어 있다면 
            {
                this.m_Button.onClick.RemoveListener(ExitGame);  // onClick 이벤트에서 ExitGame 메소드 리스너를 제거
            }
        }


        // 게임 종료 메소드
        public void ExitGame() 
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;  // 에디터에서 실행 중이라면 재생을 중지(개발 중 테스트 용도)
#else
            Application.Quit();   // 빌드된 게임에서 실행 중이라면 게임 종료
#endif
        }
    }
}

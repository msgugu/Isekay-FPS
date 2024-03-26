using UnityEngine;
using System.Collections.Generic;

namespace InGame.UI
{
	public class UIWindowManager : MonoBehaviour {

        private static UIWindowManager m_Instance;

        /// <summary>
        /// 윈도우 매니저의 현재 인스턴스를 가져온다.
        /// </summary>
        public static UIWindowManager Instance
        {
            get { return m_Instance; }
        }

        // ESC키 입력을 받기위한 이름
        [SerializeField] private string m_EscapeInputName = "Cancel";

        // ESC키 입력이 사용되었는지 여부를 나타내는 변수   
        private bool m_EscapeUsed = false;

        /// <summary>
        /// ESC 키 입력 이름을 가져온다.
        /// </summary>
        public string escapeInputName
        {
            get { return this.m_EscapeInputName; }
        }

        /// <summary>
        /// 현재 프레임에서 윈도우를 숨기기 위해 ESC 키 입력이 사용되었는지 여부를 나타내는 값을 가져온다.
        /// </summary>
        public bool escapedUsed
        {
            get { return this.m_EscapeUsed; }
        }

        // 상속받는 곳에서 Awake()메서드를 재정의 할 수 있도록 추상함수로 선언 
        protected virtual void Awake()
        {
            if (m_Instance == null)  // UI Window가 비어있을때
            {
                m_Instance = this;   // 윈도우 매니저의 현재 인스턴스값 설정
            }
            else
            {
                Destroy(this.gameObject);  // 다른 인스턴스가 존재할 경우 현재 인스턴스 파괴
            }
        }

        protected virtual void Update()
        {
            // Esc입력 사용여부 변수 초기화 
            if (this.m_EscapeUsed)
                this.m_EscapeUsed = false;

            // ESC키 입력확인
            if (Input.GetButtonDown(this.m_EscapeInputName))
            {
                // 현재 열린 작은(부속)창이 있는지 확인하고 발견되면 메서드를 종료
                UIModalBox[] modalBoxes = FindObjectsOfType<UIModalBox>();

                if (modalBoxes.Length > 0) // 작은(부속)창이 있을때
                {
                    foreach (UIModalBox box in modalBoxes)
                    {
                        // 박스가 활성화되어 있으면 종료
                        // 작은(부속)창이 활성화되어있고, GameObject가 활성화된 상태인지, 계층구조내에서 활성상태인지 
                        if (box.isActive && box.isActiveAndEnabled && box.gameObject.activeInHierarchy)
                            return;
                    }
                }

                // UIWindow목록을 가져온다.
                List<UIWindow> windows = UIWindow.GetWindows();

                // 루프를통해 필요한 경우 윈도우를 숨긴다
                foreach (UIWindow window in windows)
                {
                    // 윈도우가 ESC 키 동작을 가지고 있는지 확인
                    // => ESC 키가 눌렸을 때 창을 숨기는 동작실행
                    if (window.escapeKeyAction != UIWindow.EscapeKeyAction.None)
                    {
                        // ESC 키로 숨겨져야 하는지 확인
                        if (window.IsOpen && (window.escapeKeyAction == UIWindow.EscapeKeyAction.Hide // 현재 창이 열려있는지 확인한다 && 
                            || window.escapeKeyAction == UIWindow.EscapeKeyAction.Toggle              // 현재 창이 ESC키를 눌렀을때 숨겨져야 하는지 || 토클이 되어야 하는지 확인한다.
                            || (window.escapeKeyAction == UIWindow.EscapeKeyAction.HideIfFocused  // 부속창이 현재 포커스를 가지고 있는지확인, 포커스를 가지고 있고 ESC키가 눌렸으며,
                            && window.IsFocused)))                                                // escapeKeyAction이 "HideFocused"로 설정되어 있으면 창을 숨긴다.

                        {
                            // 윈도우 숨기기
                            window.Hide();

                            // ESC 입력 사용으로 표시
                            this.m_EscapeUsed = true;
                        }
                    }
                }

                // ESC가 윈도우 숨기기에 사용되었으면 종료
                if (this.m_EscapeUsed)
                    return;

                // 만약 필요로 한다면 윈도우를 다시 보여준다.
                foreach (UIWindow window in windows)
                {
                    // 윈도우기 표지되지 않으며 ESC키 동작이 토클이 아닐때
                    if (!window.IsOpen && window.escapeKeyAction == UIWindow.EscapeKeyAction.Toggle)
                    {
                        // 윈도우 표시 
                        window.Show();
                    }
                }
            }
        }
    }
}
 

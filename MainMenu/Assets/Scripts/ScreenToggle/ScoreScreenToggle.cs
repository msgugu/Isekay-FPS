using UnityEngine;
using System.Collections.Generic;

namespace InGame.UI
{
    public class ScoreScreenToggle : MonoBehaviour
    {
        private UIWindow m_Window;

        protected void Awake()
        {
            this.m_Window = this.gameObject.GetComponent<UIWindow>();
        }

        void Update()
        {
            
            // m_Window가 null이면 이후 코드를 실행하지 않고 반환
            if (this.m_Window == null)
                return;

            // UIWindow.GetWindows()를 호출하여 현재 활성화되어 있는 모든 UIWindow 인스턴스의 리스트를 가져옴
            List<UIWindow> windows = UIWindow.GetWindows();

            // windows 리스트를 순회하면서 각 창의 상태를 확인 => foreach반복문을 사용하여 계층구조 오류방지
            foreach (UIWindow window in windows)
            {
                // 현재 창(window)가 열려있고(this.m_Window와 다른 창일 경우) 다른 창이 열려있으면 함수 종료
                if (window.IsOpen && window != this.m_Window)
                    return;
            }

            // 키 입력 처리
            // Tab 키를 누르는 순간
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                this.m_Window.Show();  // 점수판을 보여줌
            }

            // Tab 키에서 손을 떼는 순간
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                this.m_Window.Hide();  // 점수판을 숨김
            } 
        }
    }
}

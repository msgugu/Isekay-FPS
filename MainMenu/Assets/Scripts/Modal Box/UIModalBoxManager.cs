using UnityEngine;

namespace InGame.UI
{
    public class UIModalBoxManager : ScriptableObject
    {
        // 싱글턴 패턴을 사용하여 UIModalBoxManager 인스턴스를 관리
        // 이는 애플리케이션 전체에서 단 하나의 인스턴스만 존재하게 보장
        private static UIModalBoxManager m_Instance;
        public static UIModalBoxManager Instance
        {
            get
            {   
                if (m_Instance == null)
                    // 인스턴스가 아직 생성되지 않았다면, "ModalBoxManager"라는 이름의 리소스를 로드하여 할당
                    m_Instance = Resources.Load("ModalBoxManager") as UIModalBoxManager;

                return m_Instance;
            }
        }


        // 모달 박스 프리팹에 대한 참조를 저장(인스텍터내 설정)
        [SerializeField] private GameObject m_ModalBoxPrefab;


        /// <summary>
        /// 모달 박스 프리팹을 가져옵니다.
        /// </summary>
        public GameObject prefab
        {
            get
            {
                return this.m_ModalBoxPrefab;
            }
        }

        /// <summary>
        /// 새로운 모달 박스를 생성하고 초기화
        /// </summary>
        /// <param name="rel"> 모달 박스가 생성될 캔버스를 찾기 위한 기준이 되는 게임 오브젝트 </param>
        /// <returns> 생성된 UIModalBox 컴포넌트를 반환 </returns>
        public UIModalBox Create(GameObject rel)
        {
            // 프리팹이나 참조 게임 오브젝트가 없다면 null을 반환
            if (this.m_ModalBoxPrefab == null || rel == null)
                return null;

            // 주어진 게임 오브젝트의 상위 요소 중 Canvas 컴포넌트를 찾음
            Canvas canvas = UIUtility.FindInParents<Canvas>(rel);

            if (canvas != null)
            {
                // 캔버스를 부모로 하여 모달 박스 프리팹의 인스턴스를 생성
                GameObject obj = Instantiate(this.m_ModalBoxPrefab, canvas.transform, false);

                // 생성된 오브젝트에 UIModalBox 컴포넌트를 찾아 반환
                return obj.GetComponent<UIModalBox>();
            }

            // 캔버스를 찾을 수 없다면 null을 반환 
            return null;
        }
    }
}

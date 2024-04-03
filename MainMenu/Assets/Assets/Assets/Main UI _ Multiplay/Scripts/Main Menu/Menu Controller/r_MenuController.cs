using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 메뉴타입 열거형 (패널값을 찾기위해 사용) => Menu Type
/// </summary>
[System.Serializable]
public enum r_MenuType
{
    Empty,
    Username,
    RoomBrowser,
    CreateRoom
}


/// <summary>
/// 패널과 해당 컴포넌트들을 구성된 합쳐진 상태로 관리하기 위한 시리얼라이즈 가능한 클래스 => Menu Item
/// </summary>
[System.Serializable]
public class m_MenuItem
{
    // 메뉴타입 지정
    [Header("Panel Type")]  
    public r_MenuType m_MenuType;

    // 해당메뉴의 패널 객체
    [Header("Panel")]
    public GameObject m_Panel;

    [Header("Buttons")]
    public Button m_OpenButton;   // 메뉴를 여는 버튼
    public Button m_CloseButton;  // 메뉴를 닫는 버튼
   

    // 시작 시 활성화 여부
    [Header("Start Setup")]
    public bool m_OnStart;     
}

/// <summary>
/// 멀티게임에서 메뉴 컨트롤러 관리
/// </summary>
public class r_MenuController : MonoBehaviour
{
    // 메뉴패널 목록
    [Header("Menu Panels")]
    public List<m_MenuItem> m_MenuPanels = new List<m_MenuItem>();

    // 메뉴버튼 패널
    [Header("Menu Buttons Panel")]
    public GameObject m_MenuButtonsPanel;

    
    [Header("Username UI")]
    public InputField m_UsernameInput;    // 사용자 이름 입력필드
    public Button m_ApplyUsernameButton;  // 사용자 이름 적용버튼
    
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
            r_PhotonHandler.instance.ConnectToPhoton();

        DisableAllPanels();
        SetupMenuPanel(true);
        CheckUsername();
        HandleButtons();
    }
   

    /// <summary>
    /// Player UserName 설정
    /// </summary>
    // Set Username
    private void CheckUsername()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PlayerPrefs.HasKey("username"))
            {
                m_MenuButtonsPanel.SetActive(true);
                SetupMenuPanel(false);

                // PlayerPrefs에서 사용자 이름 불러와 설정
                PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("username");
                m_UsernameInput.text = PlayerPrefs.GetString("username");
            }
            else // 그외 랜덤으로 닉네임 부여
                m_UsernameInput.text = "Player" + Random.Range(1, 999);
        }
    }
    /// <summary>
    /// Username 저장 
    /// </summary>
    /// <param name="_Username"></param> 
    private void SaveUsername(string _Username) // 매개변수를 통해 저장할 사용자 이름을 설정해준다.
    {
        PlayerPrefs.SetString("username", _Username);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("username");
    }


    /// <summary>
    /// 패널 활성화 및 비활성화 관리 (Handle UI)
    /// </summary>
    private void SetupMenuPanel(bool _State)
    {
        foreach (m_MenuItem _MenuItem in m_MenuPanels)
        {
            if (_MenuItem.m_OnStart)
                _MenuItem.m_Panel.SetActive(_State);
        }
    }

    /// <summary>
    /// 모든 메뉴 패널의 활성화 및 버튼 이벤트 리스너 설정
    /// </summary>
    private void HandleButtons()
    {
        // 사용자 이름 적용 버튼 이벤트 리스너 설정
        m_ApplyUsernameButton.onClick.AddListener(delegate { if (!string.IsNullOrEmpty(m_UsernameInput.text))
                SaveUsername(m_UsernameInput.text); r_AudioController.instance.PlayClickSound(); });
        
        // 각 메뉴 아이템에 대해 열기 및 닫기 버튼 이벤트 리스너 설정
        foreach (m_MenuItem _MenuItem in m_MenuPanels)
        {
            if (_MenuItem.m_OpenButton != null)
            {
                _MenuItem.m_OpenButton.onClick.AddListener(delegate
                {
                    DisableAllPanels();
                    m_MenuButtonsPanel.SetActive(false);
                    r_AudioController.instance.PlayClickSound();

                    if (_MenuItem.m_Panel != null)
                        _MenuItem.m_Panel.SetActive(true);
                });
            }

            if (_MenuItem.m_CloseButton != null)
                _MenuItem.m_CloseButton.onClick.AddListener(delegate { if (_MenuItem.m_Panel != null) DisableAllPanels(); r_AudioController.instance.PlayClickSound(); m_MenuButtonsPanel.SetActive(true); });
        }
    }

    /// <summary>
    /// 모든 패널을 비활성화
    /// </summary>
    private void DisableAllPanels()
    {
        foreach (m_MenuItem _Panel in m_MenuPanels)
            _Panel.m_Panel.SetActive(false);
    }
}
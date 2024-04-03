using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class r_LobbyControllerUI : MonoBehaviour
{
    // Player List 표시될 UI Container
    [Header("Player List Content")]
    public Transform m_PlayerListContent; // 플레이어 목록이 표시될 UI 요소의 부모 컨테이너
    public GameObject m_PlayerListEntry;  // 플레이어 목록의 각 항목을 나타내는 UI 요소의 Prefabs

    // 로비 UI버튼
    [Header("Lobby UI Buttons")]
    public Button m_SearchGameButton;  // 로비 생성버튼 (방을 생성하고 로비를 생성하고자 할 때)
    public Button m_LeaveLobbyButton;  // 로비 Leave 버튼 (플레이어가 로비에서 나가고자 할 때)

    // 메뉴UI
    [Header("Menu UI")]
    public GameObject m_MenuPanel;  // 메뉴패널 UI를 나타낸다 (메뉴옵션들이 이 패널 내에 위치)
    public GameObject m_LobbyPanel; // 로비패널 UI를 나타낸다 (로비옵션들이 이 패널 내에 위치)

    //// 로비 게임정보 UI
    //[Header("Lobby Game Information UI")]
    //public Image m_GameMapImage;
    //public Text m_GameInformationText;

}

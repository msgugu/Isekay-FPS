using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class r_CreateRoomControllerUI : MonoBehaviour
{

    // 방 이름을 입력하는 UI 요소
    [Header("Room Name Field")]
    public InputField m_RoomNameInput;
   
    // 게임시작 버튼
    [Header("Game Start")]
    public Button m_CreateRoomButton;

    // 게임 맵, 게임 모드, 플레이어 제한을 표시하는 텍스트 UI 요소
    [Header("Room Name Field")]
    public Text m_PlayerLimitText;
    // public Text m_GameMapText;
    // public Text m_GameModesText;


    // 플레이어 제한을 탐색하는 UI 요소
    [Header("Player Limit UI")]
    public Button m_NextPlayerLimitButton;
    public Button m_PreviousPlayerLimitButton;

}

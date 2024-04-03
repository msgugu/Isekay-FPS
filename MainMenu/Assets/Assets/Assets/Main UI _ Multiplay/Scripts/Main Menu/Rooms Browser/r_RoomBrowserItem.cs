using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class r_RoomBrowserItem : MonoBehaviour
{
    
    [Header("Room Browser UI")]
    public Text m_RoomNameText;    // 방 이름을 표시 (Text Field)
    public Text m_PlayersText;     // 현재 플레이어 수를 표시 (Text Field)
    // public Text m_MapNameText;  // 맵 이름을 표시
    // public Text m_GameModeText; // 게임 모드를 표시

    // 참가버튼 UI버튼 설정
    [Header("Join Room UI")]
    public Button m_JoinRoomButton;

    // 현재 방에 대한 정보를 저장하는 변수
    [Header("Room Configuration")]
    public RoomInfo m_RoomInfo;


    // m_JoinRoomButton에 클릭 이벤트 리스너를 추가
    // 클릭 사운드를 재생
    private void Awake() => m_JoinRoomButton.onClick.AddListener(delegate { JoinRoom(); r_AudioController.instance.PlayClickSound(); });



    /// <summary>
    /// Browser에 Room Information를 설정
    /// </summary>
    /// <param name="_RoomInfo"></param>
    public void SetupRoom(RoomInfo _RoomInfo)
    {
        m_RoomInfo = _RoomInfo;  // 전달받은 방 정보를 현재 인스턴스에 저장

        m_RoomNameText.text = m_RoomInfo.Name;  // 방 이름을 UI에 설정
        m_PlayersText.text = m_RoomInfo.PlayerCount + "/" + m_RoomInfo.MaxPlayers; // 현재 방의 플레이어수와 최대 플레이어 수를 표시
        // m_MapNameText.text = m_RoomInfo.CustomProperties["GameMap"].ToString();
        // m_GameModeText.text = m_RoomInfo.CustomProperties["GameMode"].ToString();

    }

    // 방 참여버튼이 눌렸을때 호출
    public void JoinRoom()
    {
        if (m_RoomInfo.PlayerCount == m_RoomInfo.MaxPlayers)
        {
            Debug.Log("방에 빈공간이 없습니다.");
            return;
        }

        // PhotonNetwork를 사용하여 방에 참여
        PhotonNetwork.JoinRoom(m_RoomInfo.Name);
    }

}
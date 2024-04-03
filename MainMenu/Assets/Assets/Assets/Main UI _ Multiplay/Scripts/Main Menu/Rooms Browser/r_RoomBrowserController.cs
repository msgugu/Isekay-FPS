using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class r_RoomBrowserController : MonoBehaviour
{
    public static r_RoomBrowserController instance; // Singleton instance;

    // 탐색 가능한 방 List
    [Header("Room Browser")]
    public List<RoomInfo> m_RoomBrowserList = new List<RoomInfo>();

    // 방 List 표시할 UI Container
    [Header("Room Browser Content")]
    public Transform m_RoomBrowserContent;

    // 개별 방을 표시할 UI Prefabs
    [Header("Room Browser Item")]
    public r_RoomBrowserItem m_RoomBrowserItem;

    // 방 List 갱신버튼
    [Header("Room Browser Refresh")]
    public Button m_RoomBrowserRefreshButton;



    private void Awake()
    {
        // Singleton 구현
        if (instance)
        {
            Destroy(this);
            Destroy(this.gameObject);
        }

        instance = this;
        HandleButtons();
    }

    /// <summary>
    /// refresh버튼을 누를 때의 이벤트 핸들러 설정
    /// </summary>
    private void HandleButtons() => m_RoomBrowserRefreshButton.onClick.AddListener(delegate { PhotonNetwork.JoinLobby(TypedLobby.Default); r_AudioController.instance.PlayClickSound(); });


    /// <summary>
    /// 방 List를 새로고침
    /// 기존 목록을 제거 => 다시 목록을 추가
    /// </summary>
    public void RefreshRoomBrowser()
    {
        RemoveRoomsBrowserItems(); // 기존 방 List 항목제거

        // 사용 가능한 모든방 정보 순회
        foreach (RoomInfo _RoomInfo in m_RoomBrowserList)
        {
            // 방이 보이지 않는 경우 메서드 종료
            if (!_RoomInfo.IsVisible) 
                return;

            // 방에 플레이어 수 제한이 있을 경우만 방 목록 항목을 생성
            if (_RoomInfo.MaxPlayers > 0)
            {
                r_RoomBrowserItem _RoomBrowserItem = (r_RoomBrowserItem)Instantiate(m_RoomBrowserItem, m_RoomBrowserContent.transform);
                _RoomBrowserItem.SetupRoom(_RoomInfo);   // 생성된 방 목록 항목에 방 정보를 설정
            }
        }
    }
    /// <summary>
    ///  방 List UI에서 모든 방 제거
    /// </summary>
    public void RemoveRoomsBrowserItems()
    {
        // 불필요한 작업방지
        if (m_RoomBrowserContent.childCount == 0)
            return;

        foreach (Transform _RoomBrowserItem in m_RoomBrowserContent)
            Destroy(_RoomBrowserItem.gameObject);
    }
}
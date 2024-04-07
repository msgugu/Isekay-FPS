using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

/// <summary>
/// 프로토타입 UI
/// </summary>
public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    private void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        // 포톤서버 셋팅한걸 사용하는 의미
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 서버 연결 
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버입장");
        PhotonNetwork.JoinLobby();
        // 씬 동기화 (씬 이동할때 같이 되도록 하는)
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// 로비 연결
    /// </summary>
    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("title");
        Debug.Log("로비입장");
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    /// <summary>
    /// 방만들기
    /// </summary>
    public void CreateRoom()
    {
        // 방이름 아무것도 없으면 
        if (string.IsNullOrEmpty(roomNameInputField.text)) 
            return;

        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("loading");
    }

    /// <summary>
    /// 방에 입장하면 자동으로 실행하는 함수 
    /// </summary>
    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        // 호스트만 스타트 버튼 만들기
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    /// <summary>
    /// 호스트 변경시 게임 스타트 버트 켜기
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    /// <summary>
    /// 방만들기 실패하면 뜨는 예외 함수
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed" + message;
        MenuManager.instance.OpenMenu("error");
    }

    /// <summary>
    /// 게임시작
    /// </summary>
    public void StartGame()
    {
        // Game Scene 불러오기
        PhotonNetwork.LoadLevel(1);
    }

    /// <summary>
    /// 방나가기
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("loading");
    }
    /// <summary>
    /// 방나갔을때 호출되는 함수
    /// </summary>
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("title");
    }

    /// <summary>
    /// 방 리스트 업데이트
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    /// <summary>
    /// 룸에 플레이어 만들기
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}

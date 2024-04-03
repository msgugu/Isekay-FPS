using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;

// Photon 네트워크 이벤트를 관리
public class r_PhotonHandler : MonoBehaviourPunCallbacks
{
    public static r_PhotonHandler instance;

   
    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;
        PhotonNetwork.IsMessageQueueRunning = false;  // Photon 메시지 큐를 일시 중지
    }

    private void Start()
    {
        // 이 오브젝트를 씬 변경 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(this);

        // 모든 클라이언트가 같은 씬을 자동으로 동기화하도록 설정
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    // Photon 서버에 연결합니다.
    public void ConnectToPhoton() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnected() => Debug.Log("서버에 연결되었습니다.");

    // 마스터 서버에 연결되면 기본로비 입장
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(TypedLobby.Default);


    // 새로운 방을 생성
    public void CreateRoom(string _RoomName, RoomOptions _RoomOptions) => PhotonNetwork.CreateRoom(_RoomName, _RoomOptions, null, null);

    // 방 생성시 호출
    public override void OnCreatedRoom() => Debug.Log("방이 생성되었습니다.");

    // 로비 입장시 호출
    public override void OnJoinedLobby() => Debug.Log("로비와 연결되었습니다.");

    // 로비 연결끊기
    public void Disconnet() => PhotonNetwork.Disconnect();
     
    public override void OnDisconnected(DisconnectCause cause) => print("연결이 끊겼습니다.");
   

    // 방 List 업데이트시 호출
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (r_RoomBrowserController.instance != null)
        {
            r_RoomBrowserController.instance.m_RoomBrowserList = roomList;
            r_RoomBrowserController.instance.RefreshRoomBrowser();
        }

        r_LobbyController.instance.ReceiveRoomList(roomList);
    }

    /// <summary>
    /// 방 입장시 호출
    /// </summary>
    public override void OnJoinedRoom()
    {
        // Photon 메시지 큐를 활성화
        PhotonNetwork.IsMessageQueueRunning = true;

        // 현재 방의 상태 확인
        string _RoomState = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomState"];

        // 방 상태에 따른 동작수행
        switch (_RoomState)
        {
            case "InLobby": r_LobbyController.instance.EnterLobby(); Debug.Log("로비에 입장"); break;
            case "InGame": LoadGame(); Debug.Log("게임이 진행중"); break;
        }
    }

    /// <summary>
    /// 방을 떠나면 호출
    /// </summary>
    public override void OnLeftRoom()
    {
        // 메인 메뉴 씬으로 이동합
        if (SceneManager.GetActiveScene().buildIndex != 0)
            PhotonNetwork.LoadLevel(0);
    }

    /// <summary>
    /// 다른 플레이어가 방을 떠나면 호출
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 현재 방의 상태를 확인
        string _RoomState = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomState"];

        // 방 상태에 따라 다른 동작을 수행
        switch (_RoomState)
        {
            case "InLobby": r_LobbyController.instance.ListLobbyPlayers(); break;   //
            case "InGame": r_InGameController.instance.CheckMasterClient(); break;  // 
        }
    }

     public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // gameScene
        if(scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerManager"), Vector3.zero,Quaternion.identity);
        }
    }
   

    /// <summary>
    /// 게임 씬을 로드
    /// </summary>
    /// 
    
    public void LoadGame()
    {
        // 마스터 클라이언트만 씬을 로드할 수 있음
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1); //"GameScene"은 로드하고 싶은 씬의 이
        /*
        GameObject obj = null;
        foreach ( var p in  PhotonNetwork.PlayerList)
        {
            obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(obj);
        }

        //GameObject obj = GameObject.Instantiate(null) as GameObject;
        //DontDestroyOnLoad(obj);
        */
                                                  
    }
    

}
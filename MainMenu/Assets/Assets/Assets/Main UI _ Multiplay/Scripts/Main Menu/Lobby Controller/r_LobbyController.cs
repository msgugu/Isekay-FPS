using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class r_LobbyController : MonoBehaviour
{
    public static r_LobbyController instance; // SingleTone Instance;

    // PhotonView 컴포넌트
    [Header("PhotonView")]
    public PhotonView m_PhotonView;

    // 로비 UI 컴포넌트
    [Header("Lobby UI")]
    public r_LobbyControllerUI m_LobbyUI;

    // 사용 가능한 방 목록
    [Header("Rooms List")]
    private List<RoomInfo> m_RoomList = new List<RoomInfo>();

    // 게임내 매칭시간
    [Header("Matchmaking Time")]
    public float m_SearchGameTime;        // 게임검색 시간
    public float m_StartGameTime;         // 게임시작 대기시간
    public float m_RejoinLobbyStartTime;  // 로비 재입장 대기시간

    // 게임시작시 필요한 최소 플레이어 수
    [Header("Matchmaking Configuration")]
    public int m_RequiredPlayers;

    private bool m_StartingGame;  // 게임시작 중인지 체크
    private bool m_RejoinLobby;   // 로비 재입장 중인지 체크

    private void Awake()
    {
        // Singleton Pattern 구현
        if (instance)
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;
        HandleButtons();
    }

    private void Start() => HandleRejoinLobby();

    private void Update()
    {
        if (InGameLobby())
            HandleLobbySearching();
    }

    /// <summary>
    /// RoomList를 받아 m_RoomList에 저장
    /// </summary>
    public void ReceiveRoomList(List<RoomInfo> _RoomList) => m_RoomList = _RoomList;


    /// <summary>
    /// 로비 재입장 처리 (IngameLobby상태 확인후 로비재입장)
    /// </summary>
    public void HandleRejoinLobby()
    {
        if (InGameLobby())
        {
            m_RejoinLobby = true;
            EnterLobby();
        }
    }

    /// <summary>
    /// 로비입장 상태관리 (로비 UI, 플레이어 목록 표시등을 관리)
    /// </summary>
    public void EnterLobby()
    {
        Debug.Log("로비에 입장하셨습니다.");

        SetLobbyMenu(true);
        ListLobbyPlayers();
        // DisplayGameInformation();
    }


    /// <summary>
    ///  로비에서 나갈때 상태관리 (PhotonNetwork를 통해 방을 나가고 로컬 플레이어 목록을 초기화)
    /// </summary>
    public void LeaveLobby()
    {
        if (InGameLobby())
            PhotonNetwork.LeaveRoom();

        CleanLocalPlayerList();
        SetLobbyMenu(false);
    }


    /// <summary>
    /// 로비에서 게임 검색 및 게임 시작 처리
    /// 필요한 플레이어 수에 도달하면 게임을 시작
    /// </summary>
    private void HandleLobbySearching()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= m_RequiredPlayers && !m_StartingGame)
            StartCoroutine(StartGame());
    }

    /// <summary>
    /// 현재 상태확인 (게임,로비 상태확인)
    /// </summary>
    /// <returns></returns>
    public bool InGameLobby()
    {
        if (PhotonNetwork.InRoom && ((string)PhotonNetwork.CurrentRoom.CustomProperties["RoomState"] == "InLobby"))
            return true; else return false;
    }

    /// <summary>
    /// 게임 검색 Process
    /// 사용 가능한 방이 있는 경우 랜덤하게 방에 참여, 없으면 새로운 방을 생성
    /// </summary>
    public IEnumerator SearchGame()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        yield return new WaitForSeconds(m_SearchGameTime);

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
            if (m_RoomList.Count > 0)
            {
                PhotonNetwork.JoinRandomRoom();
               // DisplayGameInformation();
            }
            else
            {
                r_PhotonHandler.instance.CreateRoom("Room " + Random.Range(0, 999), r_CreateRoomController.instance.SetRoomOptions(true));

                yield return new WaitForSeconds(2f);

                // DisplayGameInformation();
            }
        }
    }

    /// <summary>
    /// 게임 시작 Process
    /// 게임시작에 필요한 플레이어 수에 도달하면 게임을 로드
    /// </summary>
    public IEnumerator StartGame()
    {
        Debug.Log("게임시작");

        m_StartingGame = true;

        yield return new WaitForSeconds(m_RejoinLobby ? m_RejoinLobbyStartTime : m_StartGameTime);

        if (PhotonNetwork.CurrentRoom.PlayerCount < m_RequiredPlayers)
        {
            m_StartingGame = false;
            yield break;
        }

        Hashtable _State = new Hashtable(); _State.Add("RoomState", "InGame");
        PhotonNetwork.CurrentRoom.SetCustomProperties(_State);

        r_PhotonHandler.instance.LoadGame();
        m_StartingGame = true;
    }


    /// <summary>
    /// (로비에 입장하기 전) 로컬 플레이어 목록을 관리
    /// </summary>
    public void AddLocalPlayerToList(Player _PhotonPlayer)
    {
        GameObject _PlayerListEntry = (GameObject)Instantiate(m_LobbyUI.m_PlayerListEntry.gameObject, m_LobbyUI.m_PlayerListContent);
        _PlayerListEntry.GetComponent<r_LobbyEntry>().SetupLobbyPlayer(_PhotonPlayer);
    }

    /// <summary>
    /// 로컬 플레이어 목록 초기화
    /// </summary>
    public void CleanLocalPlayerList()
    {
        if (m_LobbyUI.m_PlayerListContent.childCount > 0)
        {
            foreach (Transform _PhotonPlayer in m_LobbyUI.m_PlayerListContent)
                Destroy(_PhotonPlayer.gameObject);
        }
    }
    /// <summary>
    /// 로비에 참여한 플레이어 목록표시
    /// </summary>
    public void ListLobbyPlayers() => m_PhotonView.RPC("ListLobbyPlayersRPC", RpcTarget.All);

    [PunRPC]
    public void ListLobbyPlayersRPC()
    {
        if (m_LobbyUI.m_PlayerListContent.childCount > 0)
        {
            foreach (Transform _PhotonPlayer in m_LobbyUI.m_PlayerListContent)
                Destroy(_PhotonPlayer.gameObject);
        }

        foreach (Player _PhotonPlayer in PhotonNetwork.PlayerList)
        {
            GameObject _LobbyPlayer = Instantiate(m_LobbyUI.m_PlayerListEntry, m_LobbyUI.m_PlayerListContent);
            _LobbyPlayer.GetComponent<r_LobbyEntry>().SetupLobbyPlayer(_PhotonPlayer);
        }
    }

    /// <summary>
    /// 로비 UI 및 버튼 이벤트 처리
    /// </summary>
    private void HandleButtons()
    {
        m_LobbyUI.m_LeaveLobbyButton.onClick.AddListener(delegate { StopAllCoroutines(); LeaveLobby(); r_AudioController.instance.PlayClickSound(); });
        m_LobbyUI.m_SearchGameButton.onClick.AddListener(delegate { StartCoroutine(SearchGame()); AddLocalPlayerToList(PhotonNetwork.LocalPlayer); SetLobbyMenu(true); r_AudioController.instance.PlayClickSound(); });
    }

    /// <summary>
    /// 게임 정보를 표시
    /// 방에 있는 경우 게임 맵과 모드를 표시하고, 아닐 경우 검색 중으로 표시
    /// </summary>
    // public void DisplayGameInformation()
    // {
    // m_LobbyUI.m_GameInformationText.text = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.CustomProperties["GameMap"].ToString() + " / " + PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString() : "찾는중...";

    // if (PhotonNetwork.InRoom) m_LobbyUI.m_GameMapImage.sprite = r_CreateRoomController.instance.m_GameMaps[(int)PhotonNetwork.CurrentRoom.CustomProperties["GameMapImageID"]].m_MapImage;
    // else m_LobbyUI.m_GameMapImage.sprite = null;
    // }

    /// <summary>
    /// 로비 메뉴 UI설정
    /// </summary>
    public void SetLobbyMenu(bool _State)
    {
        m_LobbyUI.m_LobbyPanel.SetActive(_State ? true : false);
        m_LobbyUI.m_MenuPanel.SetActive(_State ? false : true);
    }

}
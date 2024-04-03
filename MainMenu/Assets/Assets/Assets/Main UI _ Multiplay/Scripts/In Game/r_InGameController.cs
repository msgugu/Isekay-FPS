using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class r_InGameController : MonoBehaviour
{
    // Singleton instance
    public static r_InGameController instance;

    // 플레이어 프리팹 참조
    [Header("Player Prefab")]
    public GameObject m_PlayerPrefab;

    // 스폰 위치를 위한 배열
    [Header("SpawnPoints")]
    public bool m_Spawned;              // 플레이어가 이미 스폰되었는지 체크
    public Transform[] m_SpawnPoints;   // 가능한 스폰 포인트들

    // UI 컨트롤을 위한 버튼 참조
    [Header("UI")]
    public Button m_SpawnButton;    // 플레이어 스폰 버튼
    public Button m_LeaveButton;    // 방을 떠나는 버튼
    public Button m_EndGameButton;  // 게임 종료 버튼



    private void Awake()
    {
        // Singleton patton구현
        if (instance)
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;
        HandleButtons();
    }

    private void HandleButtons()
    {
        // 마스터 클라이언트 체크하여 버튼 상태 업데이트
        CheckMasterClient();

        // 스폰, 룸을 떠나기, 게임 종료를 위한 버튼 이벤트 리스너 추가
        m_SpawnButton.onClick.AddListener(delegate { SpawnPlayer(); r_AudioController.instance.PlayClickSound(); });
        m_LeaveButton.onClick.AddListener(delegate { LeaveRoom(); r_AudioController.instance.PlayClickSound(); } );
        m_EndGameButton.onClick.AddListener(delegate { StartCoroutine(EndGame()); m_EndGameButton.interactable = false; r_AudioController.instance.PlayClickSound(); });
    }

    // 현재 플레이어가 마스터 클라이언트인 경우에만 게임 종료 버튼을 활성화
    public void CheckMasterClient()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            m_EndGameButton.interactable = true; else m_EndGameButton.interactable = false;
    }
   

    
    public void SpawnPlayer()
    {
        // 이미 플레이어가 스폰되었다면 더 이상 스폰하지 않음
        if (m_Spawned) return;

        // 랜덤한 스폰 포인트 선택
        Transform m_SpawnPoint = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)];

        if (m_SpawnPoint)
        {
            // PhotonNetwork.Instantiate를 사용해 네트워크 상에서 플레이어 생성
            GameObject _Player = (GameObject)PhotonNetwork.Instantiate("Player/" + m_PlayerPrefab.name, m_SpawnPoint.position, m_SpawnPoint.rotation, 0);

            // 플레이어 설정
            _Player.GetComponent<r_CharacterConfig>().SetupLocalPlayer();
        }

        m_Spawned = true;
        m_SpawnButton.interactable = false; // 스폰 버튼 비활성화
    }
 

  
    private IEnumerator EndGame()
    {
        // 룸의 상태를 InLobby로 변경하여 게임 종료 상태로 설정
        Hashtable _State = new Hashtable(); _State.Add("RoomState", "InLobby");
        PhotonNetwork.CurrentRoom.SetCustomProperties(_State);

        // 일정 시간 대기 후, 마스터 클라이언트가 모든 플레이어를 메인 씬으로 로드
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(0);
    }

    // 현재 룸을떠남
    private void LeaveRoom() => PhotonNetwork.LeaveRoom(); 
    
}
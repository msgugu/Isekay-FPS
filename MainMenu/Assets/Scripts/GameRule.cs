using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using Isekai.GC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;


public class GameRule : MonoBehaviourPunCallbacks
{
    private DateTime gameStartTime; // 게임이 시작된 시간을 기록합니다.
    [SerializeField] private float gameDuration = 600f; // 게임의 최대 지속 시간을 초 단위로 설정합니다. 여기서는 10분으로 설정했습니다.
    [SerializeField] private float _targetKill = 10;
    private bool gameEnded = false; // 게임이 종료되었는지 여부를 나타내는 플래그입니다.
    private float waitingTime = 10f; // 플레이어가 대기하는 시간을 초 단위로 설정합니다. 여기서는 10초로 설정했습니다.

    private PlayerManager playerManager; // PlayerManager 클래스의 인스턴스에 대한 참조를 저장합니다.
    private bool gameStarted = false; // 게임이 시작되었는지 여부를 나타내는 플래그입니다.
    [SerializeField] GameObject WUi;
    [SerializeField] TMP_Text WUiplayer;

    public TimeSpan RemainingTime
    {
        get
        {
            TimeSpan elapsedTime = DateTime.Now - gameStartTime;
            TimeSpan remainingTime = TimeSpan.FromSeconds(gameDuration) - elapsedTime;
            return remainingTime < TimeSpan.Zero ? TimeSpan.Zero : remainingTime;
        }
    }

    public bool GameEnded => gameEnded;
    void Start()
    {
        // 게임 시작 시간을 현재 시간으로부터 10초 뒤로 설정합니다.
        gameStartTime = DateTime.Now.AddSeconds(waitingTime);

        PhotonNetwork.CurrentRoom.IsOpen = false; // 게임 시작 시 더 이상 새로운 플레이어가 방에 참여하지 못하도록 방을 닫습니다.

        // PlayerManager 컴포넌트를 찾아서 playerManager 변수에 할당합니다.
        playerManager = FindObjectOfType<PlayerManager>();


        // 게임 시작 전에 플레이어의 컨트롤을 비활성화합니다.
        StartCoroutine(DisablePlayerControlsDis());
        //Invoke("DisablePlayerControlsDis", 5f);
        //DisablePlayerControls();
    }

    void Update()
    {
        // 게임이 이미 종료되었다면, 이후 로직을 수행하지 않습니다.
        if (gameEnded) return;

        // 게임이 시작되었는지 여부를 확인하고, 게임 시작 대기 상태에서는 아무 동작도 허용하지 않습니다.
        if (!gameStarted)
        {
            // 게임 시작 시간을 체크하여 게임 시작 대기 시간을 충족하는지 확인합니다.
            if (DateTime.Now >= gameStartTime)
            {
                // 게임 시작 대기 시간이 종료되면 게임을 시작합니다.
                StartGame();
            }
        }
        else
        {
            // 게임이 시작된 후에만 게임 시간 및 종료 조건을 체크합니다.
            CheckGameTime(); // 게임 시간을 체크하여 종료 조건을 충족하는지 확인합니다.
            CheckWinCondition(); // 게임의 승리 조건을 체크합니다.
        }
    }

    void StartGame()
    {
        EnablePlayerControls();
        // 게임이 시작되면 게임 시작 시간을 현재 시간으로 갱신합니다.
        gameStartTime = DateTime.Now;

        // 게임이 시작되면 플레이어들에게 알립니다.
        // 여기에 게임 시작 시 필요한 초기화 작업을 추가할 수 있습니다.

        // 예시: 게임 시작 시 필요한 초기화 작업
        // PhotonNetwork.Instantiate("StartingObjects", Vector3.zero, Quaternion.identity);

        // 게임이 시작되었음을 표시합니다.
        gameStarted = true;

        // 게임이 시작되면 플레이어의 컨트롤을 다시 활성화합니다.
    }

    void CheckGameTime()
    {
        // 현재 시간에서 게임 시작 시간을 빼서, 설정된 게임 지속 시간을 초과했는지 검사합니다.
        TimeSpan elapsedTime = DateTime.Now - gameStartTime;
        if (elapsedTime.TotalSeconds >= gameDuration)
        {
            EndGame(); // 게임을 종료합니다.
        }
    }

    void CheckWinCondition()
    {
        // 모든 플레이어를 순회하며 킬 수를 체크합니다.
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object playerKills;
            // 플레이어의 커스텀 프로퍼티에서 "Kills" 키로 저장된 킬 수를 가져옵니다.
            if (player.CustomProperties.TryGetValue("Kills", out playerKills))
            {
                // 특정 플레이어의 킬 수가 30 이상이면
                // 게임을 종료합니다.
                if ((int)playerKills >= _targetKill)
                {
                    WUiplayer.text = player.NickName.ToString();
                    EndGame();
                    break; // 게임 종료 조건을 충족하므로 더 이상의 검사는 필요 없으므로 반복문을 종료합니다.
                }
            }
        }
    }

    void EndGame()
    {
        gameEnded = true; // 게임이 종료되었음을 표시합니다.
        StartCoroutine(DisablePlayerControlsDis());
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        WUi.SetActive(true);

        /*
        // 모든 플레이어의 킬 수를 출력합니다.
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object playerKills;
            if (player.CustomProperties.TryGetValue("Kills", out playerKills))
            {
                int kills = (int)playerKills;
                // 여기에 케릭터 순위 나오게 하기 
                Debug.Log(player.NickName + ": " + kills + " 킬");
            }
        }
        */
        PhotonNetwork.CurrentRoom.IsOpen = true; // 게임이 종료되면 방을 다시 열어 새 게임을 준비합니다.

        // 플레이어 매니저가 존재하면 킬 수를 초기화합니다.
        if (playerManager != null)
        {
            playerManager.ResetKills();
        }
        // 모든 플레이어를 결과 화면으로 이동시킵니다.
        //PhotonNetwork.LoadLevel("ResultSceneName");
    }

    // 게임 시작 전 플레이어의 컨트롤을 비활성화합니다.
    IEnumerator DisablePlayerControlsDis()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 해당 플레이어의 ActorNumber를 가져옵니다.
            int actorNumber = player.ActorNumber;

            //PlayerManager(Clone)
            //PlayerManager(Clone)(1)
            // 해당 ActorNumber를 사용하여 플레이어 게임 오브젝트를 찾습니다.
            GameObject playerObject = null;//GameObject.Find("unitychan_dynamic(Clone)" + actorNumber); // 예를 들어, "Player1", "Player2" 등의 이름으로 설정되어 있다고 가정합니다.

            List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
            foreach (var pv in players)
            {
                PhotonView p = pv.GetComponent<PhotonView>();
                if (p.IsMine)
                {
                    playerObject = pv;
                    break;
                }
            }

            // PlayerMove 컴포넌트가 있는지 확인한 후, 있으면 비활성화합니다.
            if (playerObject != null)
            {
                PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.enabled = false;
                }
            }
            else
            {
                Debug.Log("playerObject == null" + "unitychan_dynamic(Clone)" + actorNumber);
            }
        }
    }

    // 게임이 시작되면 플레이어의 컨트롤을 활성화합니다.
    void EnablePlayerControls()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 해당 플레이어의 ActorNumber를 가져옵니다.
            int actorNumber = player.ActorNumber;

            //PlayerManager(Clone)
            //PlayerManager(Clone)(1)
            // 해당 ActorNumber를 사용하여 플레이어 게임 오브젝트를 찾습니다.
            GameObject playerObject = null;//GameObject.Find("unitychan_dynamic(Clone)" + actorNumber); // 예를 들어, "Player1", "Player2" 등의 이름으로 설정되어 있다고 가정합니다.

            List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
            foreach (var pv in players)
            {
                PhotonView p = pv.GetComponent<PhotonView>();
                if (p.IsMine)
                {
                    playerObject = pv;
                    break;
                }
            }
            // PlayerMove 컴포넌트가 있는지 확인한 후, 있으면 비활성화합니다.
            if (playerObject != null)
            {
                PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.enabled = true;
                }
            }
            else
            {
                Debug.Log("playerObject == null" + "unitychan_dynamic(Clone)" + actorNumber);
            }
        }
    }
}
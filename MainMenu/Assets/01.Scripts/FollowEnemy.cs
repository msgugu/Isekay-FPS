using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnemy : MonoBehaviour
{
    public GameObject enemyMarkerPrefab; // EnemyMarker 프리팹에 대한 참조

    // 플레이어와 해당 마커를 저장할 딕셔너리
    Dictionary<Transform, GameObject> playerMarkers = new Dictionary<Transform, GameObject>();
    [SerializeField] PhotonView isMine;

    void Start()
    {
        UpdatePlayerList(); // 시작 시 플레이어 리스트를 업데이트합니다.
    }

    void UpdatePlayerList()
    {
        if (!isMine) return;
        // 기존 마커들을 제거합니다.
        foreach (var marker in playerMarkers.Values)
        {
            if (marker != null) Destroy(marker);
        }
        playerMarkers.Clear();

        // 현재 게임에 있는 모든 플레이어를 찾습니다.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Play   er");
        foreach (GameObject player in players)
        {
            PhotonView playerPV = player.GetComponent<PhotonView>();
            Debug.Log(playerPV);
            // 널 체크를 통해 안전하게 PhotonView를 확인합니다.
            if (playerPV != null && !playerPV.IsMine) // 로컬 플레이어가 아니면 마커 생성
            {
                Debug.LogError("로칼");

                // enemyMarkerPrefab이 널이 아닌지 확인합니다.
                if (enemyMarkerPrefab != null)
                {
                    Debug.LogError("생성");

                    GameObject marker = Instantiate(enemyMarkerPrefab, player.transform.position, Quaternion.identity, transform);
                    playerMarkers.Add(player.transform, marker); // 마커를 딕셔너리에 추가
                    Debug.Log(marker);
                }
                else
                {
                    Debug.LogWarning("enemyMarkerPrefab is not assigned in the inspector!");
                }
            }
        }
    }

    void Update()
    {
        if (!isMine) return;
        // 모든 플레이어 마커의 위치를 업데이트합니다.
        foreach (var entry in playerMarkers)
        {
            Transform playerTransform = entry.Key;
            GameObject marker = entry.Value;
            marker.transform.position = playerTransform.position; // 마커 위치를 플레이어 위치로 설정
        }
        Debug.Log("돌긴하니?");
    }
}
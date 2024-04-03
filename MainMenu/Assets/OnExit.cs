using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class OnExit : MonoBehaviourPunCallbacks
{
    public void Exit()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // 마스터 클라이언트가 아닌 모든 플레이어가 룸을 떠났을 때 호출됩니다.
        // 메인 메뉴로 돌아가는 로직을 여기에 배치하세요.
        SceneManager.LoadScene(0);
    }
}

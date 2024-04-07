using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// 플레이어 킬뎃 관리및 리스폰 해주는 관리자
/// </summary>

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    GameRule gameRule;

    int Kills;
    int deaths;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameRule = FindObjectOfType<GameRule>();
    }
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    /// <summary>
    /// 케릭터 생성 하는 로직
    /// </summary>
    void CreateController()
    {
        Transform spawnpint =SpawnManager.instance.GetSpawnpoint();
        controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "unitychan_dynamic"), 
                                                spawnpint.position, spawnpint.rotation,0, new object[] {PV.ViewID});
    }

    /// <summary>
    /// HP에서 피가 0이되면 부르는 로직 
    /// </summary>
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        controller = null;

        StartCoroutine(Respawn());
    }

    /// <summary>
    /// 상대방이 죽으면 실행되는 로직
    /// </summary>
    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    /// <summary>
    /// (네트워크) 킬올리기
    /// </summary>
    [PunRPC]
    void RPC_GetKill()
    {
        Kills++;

        Hashtable hash = new Hashtable();
        hash.Add("Kills", Kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    /// <summary>
    /// 리스폰 대기 주기
    /// </summary>
    /// <returns></returns>
    IEnumerator Respawn()
    {
        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // 사망 후 일정 시간 대기
        yield return new WaitForSeconds(3f);
        if (!gameRule.GameEnded) CreateController();
    }

    /// <summary>
    /// 킬로그 만들기
    /// </summary>
    /// <param name="killerName"> 죽인 사람 </param>
    /// <param name="victimName"> 죽은 사람 </param>
    public void CreateKillLog(string killerName, string victimName)
    {
        Debug.Log("킬로그");
        PV.RPC("RPC_CreateKillLog", RpcTarget.All, killerName, victimName);
    }

    /// <summary>
    /// 킬로그 생성
    /// </summary>
    /// <param name="killerName"> 죽인 사람 </param>
    /// <param name="victimName"> 죽은 사람 </param>
    [PunRPC]
    void RPC_CreateKillLog(string killerName, string victimName)
    {
        // 로컬에서 킬로그 UI 생성
        KillLogManager.Instance.CreateKillLog(killerName, victimName);
    }
    
    /// <summary>
    /// 킬 초기화
    /// </summary>
    public void ResetKills()
    {
        Kills = 0;
    }

    /// <summary>
    /// 자기 자신의 플레이어 매니저 찾는 로직
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}


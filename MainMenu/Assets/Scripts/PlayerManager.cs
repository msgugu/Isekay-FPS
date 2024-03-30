using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    void CreateController()
    {
        Transform spawnpint =SpawnManager.instance.GetSpawnpoint();
        controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "unitychan_dynamic"), 
                                                spawnpint.position, spawnpint.rotation,0, new object[] {PV.ViewID});
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        controller = null;

        StartCoroutine(Respawn());
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        Kills++;

        Hashtable hash = new Hashtable();
        hash.Add("Kills", Kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

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
  
    public void CreateKillLog(string killerName, string victimName)
    {
        Debug.Log("킬로그");
        PV.RPC("RPC_CreateKillLog", RpcTarget.All, killerName, victimName);
    }

    [PunRPC]
    void RPC_CreateKillLog(string killerName, string victimName)
    {
        Debug.Log(killerName + victimName);
        // 로컬에서 킬로그 UI 생성
        KillLogManager.Instance.CreateKillLog(killerName, victimName);
    }
    public void ResetKills()
    {
        Kills = 0;
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}


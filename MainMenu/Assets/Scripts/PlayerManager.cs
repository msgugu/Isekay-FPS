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

    int Kills;
    int deaths;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        Debug.Log("플레이어 매니저 ");

        if (PV.IsMine)
        {
            CreateController();
            Debug.Log("캐릭생성");
        }
    }

    void CreateController()
    {
        Transform spawnpint =SpawnManager.instance.GetSpawnpoint();
        controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), 
                                                spawnpint.position, spawnpint.rotation,0, new object[] {PV.ViewID});
        Debug.Log(controller.name);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
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

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}


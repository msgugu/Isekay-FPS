using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 플레이어 미니맵
/// </summary>
public class MiniMap : MonoBehaviour
{
    [SerializeField] GameObject my;
    [SerializeField] PhotonView isMine;


    /// <summary>
    /// 미니맵 활성화
    /// </summary>
    private void Start()
    {
        if(isMine.IsMine)
        {
            my.SetActive(true);
        }
    }
}

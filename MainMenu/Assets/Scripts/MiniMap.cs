using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniMap : MonoBehaviour
{
    [SerializeField] GameObject my;
    [SerializeField] PhotonView isMine;


    // Start is called before the first frame update
    void Awake()
    {
    }

    private void Start()
    {
        Debug.Log(isMine.IsMine);
        if(isMine.IsMine)
        {
            my.SetActive(true);
        }
    }
}

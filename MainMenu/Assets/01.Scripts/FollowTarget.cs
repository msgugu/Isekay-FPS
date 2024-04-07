using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FollowTarget : MonoBehaviour
{
    
    public Transform target;
    public Vector3 Offset;
    [SerializeField] PhotonView isMine;


    void Start()
    {
    }

    void Update()
    {
        if (!isMine.IsMine) return;
        transform.position = target.transform.position + Offset; 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FollowYRotation : MonoBehaviour
{
    public Transform target;
    [SerializeField] PhotonView isMine;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMine.IsMine) return;
        transform.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);    
    }
}

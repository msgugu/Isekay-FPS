using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Throwing : Grenade
{
    [Header("References")]
    public Transform cam; // 카메라의 위치와 방향을 가져오기 위한 참조
    public Transform attackPoint; // 던질 때 오브젝트가 생성될 위치
    public GameObject objectToThrow; // 던질 오브젝트

    [Header("Settings")]
    //public int totalThrows; // 총 던질 횟수
    public float throwCooldown; // 던지는 간격

    [Header("Throwing")]
    public float throwForce; // 던질 힘
    public float throwUpwardForce; // 위로 던질 힘

    bool readyToThrow; // 던지기 가능한지 여부를 나타내는 플래그
    PhotonView PV;
    private SmokeGrenadeData _data;
    private float countdown; // 폭발까지 남은 시간  
    //private bool hasExploded = false; // 폭발 여부
    private GameObject smokeEffectInstance; // 생성된 폭발 효과 인스턴스


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _data = (SmokeGrenadeData)itemInfo;
        countdown = _data.smokeDelay; // 폭발 딜레이 초기화
    }
    public override void Use()
    {
        PV.RPC("Throw", RpcTarget.All);
    }

    [PunRPC]
    private void Throw()
    {
        readyToThrow = false; // 던지기 불가능하도록 플래그 설정

        // 오브젝트를 던지기 위해 새로운 오브젝트를 생성
        GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmokeGrenade"),
                                attackPoint.position, cam.rotation, 0, new object[] { PV.ViewID });

        // 던질 오브젝트의 Rigidbody 컴포넌트 가져오기
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // 던질 방향 계산
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        // 카메라 방향으로 Raycast를 쏴서 충돌하는 경우가 있으면 해당 지점으로 던짐
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // 던질 힘 계산
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        // 힘을 추가하여 던짐
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        //totalThrows--; // 던진 횟수 감소

        // 던지기 쿨다운 적용
        Invoke(nameof(ResetThrow), countdown);
    }

    private void ResetThrow()
    {
        readyToThrow = true; // 다시 던지기 가능하도록 플래그 설정
    }

}

using UnityEngine;
using Photon.Pun;
using System.IO;

public class SmokeGrenade : Grenade
{
    private float countdown; // 폭발까지 남은 시간  
    private bool hasExploded = false; // 폭발 여부
    private GameObject smokeEffectInstance; // 생성된 폭발 효과 인스턴스
    PhotonView PV;
    private SmokeGrenadeData _data;

    [Header("References")]
    public Transform cam; // 카메라의 위치와 방향을 가져오기 위한 참조
    public Transform attackPoint; // 던질 때 오브젝트가 생성될 위치
    public GameObject objectToThrow; // 던질 오브젝트

    [Header("Settings")]
    public int totalThrows; // 총 던질 횟수
    public float throwCooldown; // 던지는 간격

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0; // 던지기를 위한 키
    public float throwForce; // 던질 힘
    public float throwUpwardForce; // 위로 던질 힘

    bool readyToThrow; // 던지기 가능한지 여부를 나타내는 플래그


    private void Start()
    {
        PV = GetComponent<PhotonView>();    
        _data = (SmokeGrenadeData)itemInfo;
        countdown = _data.smokeDelay; // 폭발 딜레이 초기화
    } 
    void Explode()
    {
        // 폭발 효과 생성
        smokeEffectInstance = PhotonNetwork.Instantiate(_data.smokeEffectPrefab.name, transform.position + _data.smokeParticleOffset, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject); // 발사체 파괴
        Lode();
    }

    void Lode()
    {
        // 폭발 효과가 생성되고 소멸되었는지 검사
        if (smokeEffectInstance != null && !smokeEffectInstance.activeSelf)
        {
            PhotonNetwork.Destroy(smokeEffectInstance); // 폭발 효과 인스턴스 파괴
        }
    }

    [PunRPC]
    void RPC_Explode()
    {
        Explode();
    }

    public override void Use()
    {
        PV.RPC("Throw", RpcTarget.All);
        if(!hasExploded)
        {
            countdown -= Time.deltaTime; // 카운트다운 감소
            if (countdown <= 0f) // 딜레이 종료 시
            {
                hasExploded = true; // 폭발 플래그 설정
                PV.RPC("RPC_Explode", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    private void Throw()
    {
        readyToThrow = false; // 던지기 불가능하도록 플래그 설정

        // 오브젝트를 던지기 위해 새로운 오브젝트를 생성
        GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmokeGrenade"), attackPoint.position, cam.rotation,0, new object[] { PV.ViewID });

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

        totalThrows--; // 던진 횟수 감소

        // 던지기 쿨다운 적용
        Invoke(nameof(ResetThrow), throwCooldown);
    }
    [PunRPC]
    private void ResetThrow()
    {
        readyToThrow = true; // 다시 던지기 가능하도록 플래그 설정
    }
}

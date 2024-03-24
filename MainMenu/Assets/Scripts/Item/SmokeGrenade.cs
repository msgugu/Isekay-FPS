using UnityEngine;
using Photon.Pun;
using System.IO;

public class SmokeGrenade : MonoBehaviour
{
    public GameObject smokeEff;

    private float countdown; // 폭발까지 남은 시간  
    private bool hasExploded = false; // 폭발 여부
    GameObject smokeEffectInstance; // 생성된 폭발 효과 인스턴스
    PhotonView PV;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        //countdown = _data.smokeDelay; // 폭발 딜레이 초기화
    }

    private void Start()
    {
        if (PV.IsMine) // 로컬 플레이어에 의해 생성된 오브젝트인지 확인
        {
            Explode();
        }
    }
    void Explode()
    {
        PV.RPC("RPC_Explode", RpcTarget.All);
        // 폭발 효과 생성
        smokeEffectInstance = PhotonNetwork.Instantiate(smokeEff.name, transform.position + Vector3.up, Quaternion.identity);
    }

    [PunRPC]
    void RPC_Explode()
    {
        Destroy(gameObject, 5f); // 발사체 파괴
        // 폭발 효과가 생성되고 소멸되었는지 검사
        if (smokeEffectInstance != null && !smokeEffectInstance.activeSelf)
        {
            PhotonNetwork.Destroy(smokeEffectInstance); // 폭발 효과 인스턴스 파괴
        }
    }
}

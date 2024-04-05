using UnityEngine;
using System.Collections;
using Photon.Pun;
using System.IO;
using UnityEngine.UIElements;

/// <summary>
/// 연막 수류탄 이펙트 & 사운드
/// </summary>
public class SmokeGrenade : MonoBehaviour
{
    //GameObject smokeEffectInstance; // 생성된 폭발 효과 인스턴스
    
    /// <summary>
    /// 연막 폭발 사운드
    /// </summary>
    public AudioClip explodeSound; 
    
    PhotonView PV;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        //countdown = _data.smokeDelay; // 폭발 딜레이 초기화
    }

    private void Start()
    {
        // 로컬 플레이어에 의해 생성된 오브젝트인지 확인
        if (PV.IsMine) 
        {
            // 2초 후에 폭발 시작
            StartCoroutine(ExplodeAfterDelay(2f)); 
        }
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        PV.RPC("RPC_PlayExplodeSound", RpcTarget.All, transform.position);

        yield return new WaitForSeconds(delay); // 처음 지연

        // 모든 클라이언트에서 폭발 효과를 생성하고 15초 후에 파괴합니다.
        PV.RPC("RPC_Explode", RpcTarget.All, transform.position + Vector3.up);
    }
    
    /// <summary>
    /// 연막 폭발 사운드를 연막탄 오브젝트에서 소리가 멀티 플레이어에게도 들리게 함.
    /// </summary>
    /// <param name="position"> 오브젝트의 위치 </param>
    [PunRPC]
    void RPC_PlayExplodeSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(explodeSound, position);
    }

    /// <summary>
    /// 연막 폭발 이펙트를 오브젝트에서 생성하고 멀티 플레이어에게도 보이게 함.
    /// </summary>
    /// <param name="position"> 오브젝트 위치 </param>
    [PunRPC]
    void RPC_Explode(Vector3 position)
    {
        // 이펙트 생성
        GameObject smokeEffectInstance = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Particle System"), position, Quaternion.identity);

        // 15초 후에 이펙트 파괴
        StartCoroutine(DestroyAfter(smokeEffectInstance, 15f));
    }

    /// <summary>
    /// 일정 시간 뒤에 오브젝트 삭제
    /// </summary>
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
            Destroy(gameObject);
        }
    }
}

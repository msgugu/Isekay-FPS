using UnityEngine;
using System.Collections;
using Photon.Pun;
using System.IO;
using UnityEngine.UIElements;

public class SmokeGrenade : MonoBehaviour
{
    //GameObject smokeEffectInstance; // 생성된 폭발 효과 인스턴스
    public AudioClip explodeSound; // 연막 폭발 사운드
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
            StartCoroutine(ExplodeAfterDelay(2f)); // 3초 후에 폭발 시작
        }
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        AudioSource.PlayClipAtPoint(explodeSound, transform.position);

        yield return new WaitForSeconds(delay); // 처음 지연

        // 모든 클라이언트에서 폭발 효과를 생성하고 15초 후에 파괴합니다.
        PV.RPC("RPC_Explode", RpcTarget.All, transform.position + Vector3.up);
    }

    [PunRPC]
    void RPC_Explode(Vector3 position)
    {
        // 이펙트 생성
        GameObject smokeEffectInstance = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Particle System"), position, Quaternion.identity);

        // 10초 후에 이펙트 파괴
        StartCoroutine(DestroyAfter(smokeEffectInstance, 15f));
    }

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

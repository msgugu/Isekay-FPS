using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    public int Bullet
    {
        get { return _bullet; }
        set { _bullet = value; }
    }
    public float reloadTime;
    public bool isAuto;
    [SerializeField] Camera cam;

    bool reload = false;
    float _reloadTime;
    int _bullet;
    int _maxBullet;
    float _fireRate; // 건 인포에서 받아오기 초당 발사 하는 총알 갯수
    float _lastFireTime;
    PhotonView PV;
    bool _isFiring;
    GameObject fireEffect;
    AudioClip fireAudio;
    Shooter shooter;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _fireRate = ((GunInfo)itemInfo).fireRate;
        _bullet = ((GunInfo)itemInfo).bullet;
        _reloadTime = ((GunInfo)itemInfo).reloadTime;

        _maxBullet = _bullet;
        Debug.Log(_maxBullet);
        // 오디오 불러오기
        //fireAudio = ((GunInfo)itemInfo).firefireAudio;

        // 발사 이펙트 가져오기
        //fireEffect = ((GunInfo)itemInfo).fireEffect;

        _lastFireTime = -1f;
    }
    private void Start()
    {
        shooter = GetComponentInParent<Shooter>();
    }

    public override void Use()
    {
        if (PV.IsMine)
        {
            // 발사 가능한 경우에만 발사하도록 확인
            if (Time.time > _lastFireTime + 1f / _fireRate)
            {
                if(!reload)
                {
                    if (fireMode == FireMode.Auto && !_isFiring)
                    {
                        // 연발 사격 시작
                        StartCoroutine(AutoFire());
                    }
                    else
                    {
                        // 단발 사격 로직
                        Shoot();
                        _lastFireTime = Time.time; // 마지막 발사 시간 업데이트
                    }
                }
            }
        }
    }

    IEnumerator AutoFire()
    {
        _isFiring = true;
        while (Input.GetMouseButton(0)) // 마우스 버튼이 눌려있는 동안
        {
            if (_bullet == 0) break;
            Shoot();
            yield return new WaitForSeconds(1f / _fireRate); // 발사 속도에 따라 대기
        }
        _isFiring = false;
    }

    public IEnumerator ReloadCoroutine()
    {
        reload = true;
        // 리로드 시간 동안 기다립니다.
        yield return new WaitForSeconds(_reloadTime);
        // 사운드 추가 
        _bullet = _maxBullet;
        shooter.UpdateBullets(_bullet);
        reload = false;
    }

    // 단발, 연발 
    public void ToggleFireMode()
    {
        if (isAuto)
        {
            fireMode = fireMode == FireMode.Single ? FireMode.Auto : FireMode.Single;
        }
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).bamage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            
            // 오디오 추가시 활성화
            //PV.RPC("RPC_PlayFireSound", RpcTarget.All);
        }
        _bullet--;
        if (shooter != null)
        {
            // Shooter 스크립트에 있는 UpdateBullets 호출
            shooter.UpdateBullets( _bullet);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        // 이펙트 추가시 넣기
        //if (fireEffect != null)
        //{
           // GameObject effectInstance = Instantiate(fireEffect, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.forward));
            // 필요하다면 이펙트를 정리하기 위한 로직을 추가합니다. 예: 일정 시간 후에 파괴
           // Destroy(effectInstance, 2f);
        //}

        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab,hitPosition + hitNormal *0.001f,Quaternion.LookRotation(hitNormal,Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

    // 오디오 추가시 활성화 하셈
    /*
    [PunRPC]
    void RPC_PlayFireSound()
    {
        // 오디오 소스 컴포넌트가 필요합니다.
        AudioSource audioSource = this.GetComponent<AudioSource>();
        if (!audioSource)
        {
            // 오디오 소스 컴포넌트가 없으면 추가합니다.
            audioSource = this.gameObject.AddComponent<AudioSource>();
        }

        // 오디오 클립을 설정하고 재생합니다.
        audioSource.clip = fireAudio;
        audioSource.Play();
    }
    */
}

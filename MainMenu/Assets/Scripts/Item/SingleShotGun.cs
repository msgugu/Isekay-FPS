using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    public delegate void RecoilDelegate(float amount, Vector3 direction); // 반동 처리를 위한 대리자 선언

    public event RecoilDelegate onRecoil; // 이벤트 선언
    public int Bullet
    {
        get { return _bullet; }
        set { _bullet = value; }
    }
    public bool isAuto;
    [SerializeField] Camera cam;
    [SerializeField] private float recoilAmount = 2.0f; // 반동의 강도
    [SerializeField] private float recoilRecoverySpeed = 1f; // 반동 회복 속도
    [SerializeField] private float maxRecoil = 5f; // 최대 반동 크기

    private Vector3 originalCameraRotation; // 카메라의 원래 회전값 저장
    private Vector3 currentRecoilOffset = Vector3.zero; // 현재 반동에 의한 회전값 오프셋

    bool reload = false;                                                // 재장전 중 일때는 총 발사 안되도록 
    float _reloadTime;                                                  // 건 info에서 받아올 변수
    int _bullet;                                                        // 건 info에서 받아올 기본 총알 갯수
    int count;
    int _maxBullet;                                                     // 맥스 총알 설정
    float _fireRate;                                                    // 건 인포에서 받아오기 초당 발사 하는 총알 갯수
    float _lastFireTime;                                                //
    PhotonView PV;                                                      // 자기 자신 찾기 위한 포톤뷰
    bool _isFiring;                                                     //
    GameObject fireEffect;
    AudioClip fireAudio;
    Shooter shooter;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _fireRate = ((GunInfo)itemInfo).fireRate;
        _bullet = ((GunInfo)itemInfo).bullet;
        _reloadTime = ((GunInfo)itemInfo).reloadTime;
        onRecoil += ApplyRecoil;
        _maxBullet = _bullet;
        originalCameraRotation = cam.transform.localEulerAngles;

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

    private void OnDestroy()
    {
        onRecoil -= ApplyRecoil; // 올바른 이벤트에서 메서드 연결 해제 방법
    }

    public override void Use()
    {
        if (!PV.IsMine) return;

        // 발사 가능한 경우에만 발사하도록 확인
        if (Time.time > _lastFireTime + 1f / _fireRate)
        {
            if (!reload)
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

    IEnumerator AutoFire()
    {
        _isFiring = true;
        while (Input.GetMouseButton(0)) // 마우스 버튼이 눌려있는 동안
        {
            if (_bullet == 0) break;
            count++;
            Shoot();
            yield return new WaitForSeconds(1f / _fireRate); // 발사 속도에 따라 대기
        }
        _isFiring = false;
        count = 0;
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

    IEnumerator RecoilRecovery()
    {
        // 반동 복구 로직
        float time = 0;
        Vector3 startOffset = currentRecoilOffset;
        while (time < 1)
        {
            time += Time.deltaTime * recoilRecoverySpeed;
            currentRecoilOffset = Vector3.Lerp(startOffset, Vector3.zero, time);
            // 매 프레임마다 카메라 회전을 업데이트합니다.
            UpdateCameraRotation(); // 주석 해제 또는 추가
            yield return null;
        }
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
        if(count >= 3) 
        {
            onRecoil?.Invoke(recoilAmount, new Vector3(1, 1, 0)); // 여기서 direction.x는 무시됩니다.
        }
        
        if (shooter != null)
        {
            // Shooter 스크립트에 있는 UpdateBullets 호출
            shooter.UpdateBullets( _bullet);
        }
    }

    void ApplyRecoil(float amount, Vector3 direction)
    {
        // 반동 방향과 크기를 계산
        Vector3 recoilToAdd = new Vector3(-amount * direction.y, Random.Range(-amount, amount) * direction.x, 0);

        // 예상 반동을 현재 반동에 더함
        Vector3 newRecoilOffset = currentRecoilOffset + recoilToAdd;

        // 예상 반동이 최대 반동을 초과하는지 확인
        newRecoilOffset.x = Mathf.Clamp(newRecoilOffset.x, -maxRecoil, maxRecoil);
        newRecoilOffset.y = Mathf.Clamp(newRecoilOffset.y, -maxRecoil, maxRecoil);

        // 최대 반동을 고려하여 반동 오프셋 업데이트
        currentRecoilOffset = newRecoilOffset;

        // 반동 복구 시작
        StartCoroutine(RecoilRecovery());
    }

    void UpdateCameraRotation()
    {
        // 카메라의 원래 회전에 현재 반동 오프셋을 반영하여 회전을 업데이트합니다.
        cam.transform.localEulerAngles = originalCameraRotation + currentRecoilOffset;
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

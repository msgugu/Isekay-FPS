using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SingleShotGun : Gun
{
    public delegate void RecoilDelegate(float amount, Vector3 direction); // 반동 처리를 위한 대리자 선언

    public event RecoilDelegate onRecoil; // 이벤트 선언
    public Crosshair crosshair;
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
    [SerializeField] private Image _reloadImage;

    private Vector3 originalCameraRotation; // 카메라의 원래 회전값 저장
    private Vector3 currentRecoilOffset = Vector3.zero; // 현재 반동에 의한 회전값 오프셋

    bool reload = false;                                                // 재장전 중 일때는 총 발사 안되도록 
    float _reloadTime;                                                  // 건 info에서 받아올 변수
    float _reloading;

    int count;
    int _bullet;                                                        // 건 info에서 받아올 기본 총알 갯수
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
        
        _maxBullet = _bullet;

        originalCameraRotation = cam.transform.localEulerAngles;

        // 발사 이펙트 가져오기
        //fireEffect = ((GunInfo)itemInfo).fireEffect;

        _lastFireTime = -1f;
        onRecoil += ApplyRecoil;
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
                    StartCoroutine(AutoFire());
                }
                else
                {
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
        if (_maxBullet == _bullet) yield break;
        reload = true;
        _reloading = _reloadTime;
        _reloadImage.gameObject.SetActive(true);
        while (_reloading > 0)
        {
            // 경과된 시간만큼 _reloading을 감소시킵니다.
            _reloading -= Time.deltaTime;

            // fillAmount를 업데이트하여 UI를 업데이트합니다.
            _reloadImage.fillAmount = _reloading / _reloadTime;

            // 다음 프레임까지 기다립니다.
            yield return null;
        }

        _reloadImage.gameObject.SetActive(false);
        _bullet = _maxBullet;
        shooter.UpdateBullets(_bullet);
        reload = false;
    }

    // 반동 복구 로직
    IEnumerator RecoilRecovery()
    {
        float time = 0;
        Vector3 startOffset = currentRecoilOffset;
        while (time < 1)
        {
            time += Time.deltaTime * recoilRecoverySpeed;
            currentRecoilOffset = Vector3.Lerp(startOffset, Vector3.zero, time);
            UpdateCameraRotation(); 
            yield return null;
        }
    }

    // 사격모드 변경 
    public void ToggleFireMode()
    {
        // isAuto가 있는 이유는 >> 저격총 같은 총은 오토 모드가 되면 안되기 때문에 
        if (isAuto) fireMode = fireMode == FireMode.Single ? FireMode.Auto : FireMode.Single;
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        
        //ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).bamage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
        _bullet--;
        if(count >= 3) 
        {
            onRecoil?.Invoke(recoilAmount, new Vector3(1, 1, 0)); // 여기서 direction.x는 무시됩니다.
        }

        shooter?.UpdateBullets(_bullet); // Null 조건부 접근 연산자 사용
        crosshair.OnShoot();
        
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
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab,hitPosition + hitNormal *0.001f,Quaternion.LookRotation(hitNormal,Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

    
}

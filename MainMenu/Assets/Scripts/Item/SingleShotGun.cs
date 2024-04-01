using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Cinemachine;
using Unity.VisualScripting;

public class SingleShotGun : Gun
{
    public Crosshair crosshair;
    public string weaponType;
    public int Bullet
    {
        get { return _bullet; }
        set { _bullet = value; }
    }
    public bool isAuto;
    [SerializeField] Camera cam;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
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
    Shooter shooter;
    GunShake gunShake;
    AudioSource audioSource;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _fireRate = ((GunInfo)itemInfo).fireRate;
        _bullet = ((GunInfo)itemInfo).bullet;
        _reloadTime = ((GunInfo)itemInfo).reloadTime;
        _maxBullet = _bullet;

        originalCameraRotation = cam.transform.localEulerAngles;

        _lastFireTime = -1f;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
    }
    private void Start()
    {
        if (virtualCam == null)
        {
            virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        }
        gunShake = GetComponent<GunShake>();
        shooter = GetComponentInParent<Shooter>();
    }

    private void OnDestroy()
    {
        //onRecoil -= ApplyRecoil; // 올바른 이벤트에서 메서드 연결 해제 방법
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
        StartCoroutine(ResetRecoil(virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>()));
        _isFiring = false;
        count = 0;
    }

    public IEnumerator ReloadCoroutine()
    {
        if (_maxBullet == _bullet) yield break;
        reload = true;

        SoundManager.instance.PlayReloadSound(transform.position);

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
        _bullet = _maxBullet;
        _reloadImage.gameObject.SetActive(false);
        shooter.UpdateBullets(_bullet);
        reload = false;
    }

    // 사격모드 변경 
    public void ToggleFireMode()
    {
        // isAuto가 있는 이유는 >> 저격총 같은 총은 오토 모드가 되면 안되기 때문에 
        if (isAuto) fireMode = fireMode == FireMode.Single ? FireMode.Auto : FireMode.Single;

        // 사격 모드 변경 사운드
        SoundManager.instance.PlayFireModeChangeSound(transform.position);
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        gunShake.Fire();
        //ray.origin = cam.transform.position;

        // 탄약이 있는지 체크
        if (_bullet > 0)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).bamage);
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal, hit.collider.gameObject.layer);
            }

            if (count < 3 && isAuto)
            {
                StartCoroutine(ApplyRecoil());
            }
            count++;

            if (PV.IsMine)
            {
                BulletMuzzleEffect.gameObject.layer = LayerMask.NameToLayer("Weapon");
                BulletMuzzleEffect.Play();
            }

            _bullet--; // 탄약 감소

            // 남은 탄약이 더 이상 없음
            if (_bullet == 0)
            {
                // 탄창이 빈 소리가 나옴 (팅)
                SoundManager.instance.PlayEmptyMagazineSound(transform.position);
            }
        }

        else if(_bullet <= 0)
        {
            // 탄약이 남아 있지 않은데 계속 쏘려고 하면 틱틱 거리는 소리 재생
            SoundManager.instance.PlayNoAmmoSound(transform.position);
        }

        PV.RPC("RPC_PlayerShootSound", RpcTarget.All, weaponType); // 무기 타입에 따라 소리 재생
        shooter?.UpdateBullets(_bullet); // Null 조건부 접근 연산자 사용
        crosshair.OnShoot();

    }

    IEnumerator ApplyRecoil()
    {
        var noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise != null)
        {
            float applyTime = 0f;
            // 초기 반동 강도와 빈도 값을 저장합니다.
            float initialAmplitude = noise.m_AmplitudeGain;
            float initialFrequency = noise.m_FrequencyGain;

            // 반동이 적용되는 동안 반복합니다.
            while (applyTime < recoilRecoverySpeed)
            {
                // 반동 강도와 빈도를 점진적으로 증가시킵니다.
                noise.m_AmplitudeGain = Mathf.Lerp(initialAmplitude, maxRecoil, applyTime / recoilRecoverySpeed);
                noise.m_FrequencyGain = Mathf.Lerp(initialFrequency, maxRecoil, applyTime / recoilRecoverySpeed);

                // 시간을 증가시킵니다.
                applyTime += Time.deltaTime;

                // 다음 프레임까지 기다립니다.
                yield return null;
            }

            // 반동 적용 후 바로 반동 복구를 시작합니다.
        }
    }

    IEnumerator ResetRecoil(Cinemachine.CinemachineBasicMultiChannelPerlin noise)
    {
        float startingAmplitude = noise.m_AmplitudeGain;
        float startingFrequency = noise.m_FrequencyGain;
        float elapsedTime = 0;

        while (elapsedTime < recoilRecoverySpeed)
        {
            // 시간에 따라 lerp를 사용하여 AmplitudeGain을 줄입니다.
            noise.m_AmplitudeGain = Mathf.Lerp(startingAmplitude, 0f, elapsedTime / recoilRecoverySpeed);
            noise.m_FrequencyGain = Mathf.Lerp(startingFrequency, 0f, elapsedTime / recoilRecoverySpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 반동 효과를 제거합니다.
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal, int hitLayer)
    {
        // 총구 화염 재생
        BulletMuzzleEffect.Play();

        // 탄흔 생성
        GameObject bulletImpact = bulletImpactPrefab;

        // 레이어에 설정 되어 있는 숫자를 비교해서 탄흔 생성
        switch (hitLayer)
        {
            case int layer when layer == LayerMask.NameToLayer("Player"):
                bulletImpact = bulletImpactFlesh;
                break;
            case int layer when layer == LayerMask.NameToLayer("Ground"):
                bulletImpact = bulletImpactPrefab;
                break;
            case int layer when layer == LayerMask.NameToLayer("Default"):
                bulletImpact = bulletImpactPrefab;
                break;
        }

        // 탄흔 프리팹 생성 위치
        GameObject bulletImpactObj = Instantiate(bulletImpact, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpact.transform.rotation);
        Destroy(bulletImpactObj, 2f); // 생성된 탄흔 일정 시간이 지난 후 삭제
    }

    /// <summary>
    /// 플레이어가 총을 쏘면 재생 되는 사운드 (멀티, 다른 사람도 들을 수 있음)
    /// </summary>
    /// <param name="weaponType"> 무기의 타입 </param>
    [PunRPC]
    public void RPC_PlayerShootSound(string weaponType)
    {
        // transform.position 거리 별 사운드 감소를 위해 게임 오브젝트의 위치에서 생성
        SoundManager.instance.PlayRandomSound(weaponType, transform.position);
    }
}
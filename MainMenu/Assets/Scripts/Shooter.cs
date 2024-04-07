using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// 총 쏘는 로직
/// </summary>
public class Shooter : MonoBehaviourPunCallbacks
{
    public Text Ammo;

    PhotonView PV;
    
    int itemIndex;
    int previousItemIndex = -1;
    int currentBullets;

    [SerializeField] Item[] items;
    [SerializeField] Image[] auto;
    [SerializeField] GameObject[] weaponImage;
    [SerializeField] KeyCode toggleFireModeKey = KeyCode.F;
    [SerializeField] KeyCode reloadKey = KeyCode.R;

    Dictionary<int, int> bulletsPerGun = new Dictionary<int, int>();

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitializeItems();
        if (items.Length > 0)
        {
            items[0].itemGameObject.SetActive(true);
            if(PV.IsMine) items[0].itemGameObject.layer = LayerMask.NameToLayer("Weapon");
            currentBullets = bulletsPerGun.ContainsKey(0) ? bulletsPerGun[0] : 0;
        }
    }

    /// <summary>
    /// 인벤토리와, 발사 가능 한지 체크하고 실행
    /// </summary>
    private void Update()
    {
        if (!PV.IsMine) return;

        #region 인벤토리
        if (Input.GetKeyDown(toggleFireModeKey))
        {
            ToggleGunFireMode();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            Reload();
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                EquipItem(i);
                WeaponImage(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            int nextIndex = itemIndex >= items.Length - 1 ? 0 : itemIndex + 1;
            EquipItem(nextIndex);
            WeaponImage(nextIndex); // 무기 이미지를 업데이트 합니다.
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            int prevIndex = itemIndex <= 0 ? items.Length - 1 : itemIndex - 1;
            EquipItem(prevIndex);
            WeaponImage(prevIndex); // 무기 이미지를 업데이트 합니다.
        }
        #endregion
        #region 발사..
        if (Input.GetMouseButtonDown(0))
        {
            if (items[itemIndex] is SingleShotGun gun)
            {
                currentBullets = gun.Bullet;

            }
            if (currentBullets == 0)
            {
                return;       
            }

            items[itemIndex].Use();


            Ammo.text = currentBullets.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 자동 인지 아닌지 UI업데이트
    /// </summary>
    void ToggleGunFireMode()
    {
        // 배열 추가해서 무기에 현상 저장
        if (items[itemIndex] is SingleShotGun gun)
        {
            gun.ToggleFireMode();
            if(gun.fireMode == 0)
            {
                auto[0].enabled = true;
                auto[1].enabled = false;
            }
            else 
            {
                auto[0].enabled = false;
                auto[1].enabled = true;
            }
        }
    }

    /// <summary>
    /// 무기 체인지
    /// </summary>
    /// <param name="_index"></param>
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;
        foreach (var item in items)
        {
            item.itemGameObject.SetActive(false);
        }

        WeaponImage(_index);
        ChangeWeapon(_index);
        itemIndex = _index;

        // 카메라 웨폰 자신 무기만 랜더 하도록 예외 처리 
        if (PV.IsMine)
        {
            items[itemIndex].itemGameObject.layer = LayerMask.NameToLayer("Weapon");
        }

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    /// <summary>
    /// 무기의 총알을 저장 
    /// </summary>
    private void InitializeItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].itemGameObject.SetActive(true);

            if (items[i] is SingleShotGun gun)
            {
                bulletsPerGun.Add(i, gun.Bullet); // 초기 총알 수 설정
                items[i].itemGameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 무기 UI의 이미지 변경
    /// </summary>
    /// <param name="index"></param>
    void WeaponImage(int index)
    {
        // 이전에 활성화되었던 무기 이미지를 비활성화합니다.
        if (previousItemIndex != -1)
        {
            weaponImage[previousItemIndex].SetActive(false);
        }

        // 현재 선택된 무기 이미지를 활성화합니다.
        weaponImage[index].SetActive(true);
    }

    /// <summary>
    /// 총알 업데이트 
    /// </summary>
    /// <param name="_index"></param>
    #region 총알 
    void ChangeWeapon(int _index)
    {
        if (items[_index] is SingleShotGun gun)
        {

            // 현재 무기에 남은 총알 수 업데이트
            if (bulletsPerGun.ContainsKey(itemIndex))
            {
                bulletsPerGun[itemIndex] = currentBullets;
            }

            // 새로운 무기의 남은 총알 수 가져오기
            if (bulletsPerGun.ContainsKey(_index))
            {
                currentBullets = bulletsPerGun[_index];
                Ammo.text = currentBullets.ToString();
            }
            else
            {
                currentBullets = gun.Bullet; // 딕셔너리에 없는 경우, 초기 총알 수 사용
                Ammo.text = currentBullets.ToString();
            }
        }
    }

    /// <summary>
    /// 재장전
    /// </summary>
    void Reload()
    {
        if (items[itemIndex] is SingleShotGun gun)
        {
            gun.Reload();
            bulletsPerGun[itemIndex] = gun.Bullet;
            currentBullets = bulletsPerGun[itemIndex];
        }
    }
    #endregion

    /// <summary>
    /// 현재 무기의 총알 수를 UI로 나타냄
    /// </summary>
    /// <param name="bulletCount"></param>
    #region 업데이트 확인 하는 함수
    public void UpdateBullets(int bulletCount)
    {
        if (bulletsPerGun.ContainsKey(itemIndex))
        {
            bulletsPerGun[itemIndex] = bulletCount;
        }
        else
        {
            bulletsPerGun.Add(itemIndex, bulletCount);
        }
        currentBullets = bulletCount;
        Ammo.text = currentBullets.ToString();
    }

    /// <summary>
    /// 무기가 바뀌는걸 통보하는 
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
    #endregion 
}

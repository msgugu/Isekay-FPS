using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        for (int i = 0; i< items.Length; i++)
        {
            items[i].itemGameObject.SetActive(true);
        }
    }

    private void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {

            if (items[i] is SingleShotGun gun)
            {
                //bulletsPerGun[i] = gun.Bullet; // GunInfo에서 초기 총알 수를 가져옴
                bulletsPerGun.Add(i, gun.Bullet);// = gun.Bullet; // GunInfo에서 초기 총알 수를 가져옴
                items[i].itemGameObject.SetActive(false);
            }
        }
        items[0].itemGameObject.SetActive(true);
        currentBullets = bulletsPerGun[0];
    }

    private void Update()
    {
        if (!PV.IsMine) return;
        Debug.Log(currentBullets);

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

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;
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

    #region 총알 
    void ChangeWeapon(int _index)
    {
        Debug.Log("무기 바꾸기?");
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
    #endregion 
}

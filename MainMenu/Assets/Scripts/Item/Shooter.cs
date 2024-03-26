using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
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

    Dictionary<int, int> bulletsPerGun = new Dictionary<int, int>();

    [SerializeField] KeyCode toggleFireModeKey = KeyCode.F;
    [SerializeField] KeyCode reloadKey = KeyCode.R;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] is SingleShotGun gun)
            {
                bulletsPerGun[i] = gun.Bullet; // GunInfo에서 초기 총알 수를 가져옴
                Debug.Log("어웨이크");
                Debug.Log(bulletsPerGun[i] + "총");

            }
        }
    }

    private void Update()
    {
        if (!PV.IsMine) return;
        Debug.Log(currentBullets);
        //if (bulletsPerGun == null)
        //{
          //  Bullet();
        //}

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
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(currentBullets == 0)
            {

                return;
            }
            items[itemIndex].Use();
            currentBullets--;
        }

    }

    void Bullet()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] is SingleShotGun gun)
            {
                bulletsPerGun[i] = gun.Bullet; // GunInfo에서 초기 총알 수를 가져옴
                Debug.Log(bulletsPerGun);
            }
        }
    }

    void ChangeWeapon()
    {
        Debug.Log("작동은해?");
        if (items[itemIndex] is SingleShotGun gun)
        {
            // 현재 무기에 남은 총알 수 업데이트
            if (bulletsPerGun.ContainsKey(previousItemIndex))
            {
                bulletsPerGun[previousItemIndex] = currentBullets;
            }

            // 새로운 무기의 남은 총알 수 가져오기
            if (bulletsPerGun.ContainsKey(itemIndex))
            {
                currentBullets = bulletsPerGun[itemIndex];
            }
            else
            {
                currentBullets = gun.Bullet; // 딕셔너리에 없는 경우, 초기 총알 수 사용
            }
        }
    }

    void ToggleGunFireMode()
    {
        if (items[itemIndex] is SingleShotGun gun)
        {
            gun.ToggleFireMode();
        }
    }

    void Reload()
    {
        if (items[itemIndex] is SingleShotGun gun)
        {
            gun.Reload();
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        // 카메라 웨폰 자신 무기만 랜더 하도록 예외 처리 
        // 시이이이이발
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
        ChangeWeapon();
        Ammo.text = currentBullets.ToString();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
}

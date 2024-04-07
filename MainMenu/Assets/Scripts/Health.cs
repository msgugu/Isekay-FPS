using InGame.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 HP 관리
/// </summary>
public class Health : MonoBehaviour, IDamageable
{
    // UI 
    public Test_UIProgressBar healthProgressBar;
    public GameObject HealImage; // 회복하면 나오는 UI Image
    public GameObject BloodImage; // 체력이 없으면 나오는 UI Image

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    PhotonView PV;
    PlayerManager playerManager;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        //UpdateHealthBar();
    }

    void Start()
    {
        UpdateHealthBar();
        HealImage.SetActive(false);
        BloodImage.SetActive(false);
    }

    /// <summary>
    /// 상대방이 호출할 함수 (대미지)
    /// </summary>
    /// <param name="damage"> 무기대미지 </param>
    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    /// <summary>
    /// (네트워크) 데미지 받는 로직
    /// </summary>
    /// <param name="damage"> 무기 대미지 </param>
    /// <param name="info"> 때리는 사람 </param>
    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {

        currentHealth -= damage;
        UpdateHealthBar();
        CheckHealStatus(); 

        //healthbarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            playerManager.CreateKillLog(info.Sender.NickName, PhotonNetwork.NickName);
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }

    }

    /// <summary>
    /// 피 회복하는 로직
    /// </summary>
    /// <param name="hp"> 힐량 </param>
    public void TakeHeal(float hp)
    {
        currentHealth += hp;

        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }

        PV.RPC(nameof(RPC_TakeHeal), PV.Owner, currentHealth); 
    }

    /// <summary>
    /// (네트워크) 피차는걸 알려주는 함수
    /// </summary>
    /// <param name="hp"></param>
    [PunRPC]
    void RPC_TakeHeal(float hp) 
    {
        currentHealth = hp;
        UpdateHealthBar();
        ShowHealImage();
        CheckHealStatus(); // 체력 체크
        Debug.Log(currentHealth);
    }

    /// <summary>
    /// 사망 로직 호출
    /// </summary>
    void Die()
    {
        playerManager.Die();
    }

    /// <summary>
    /// UI갱신
    /// </summary>
    void UpdateHealthBar()
    {
        if (healthProgressBar != null)
        {
            float healthPercent = currentHealth / maxHealth;
            healthProgressBar.SetFillAmount(healthPercent);
        }
    }

    /// <summary>
    /// 체력 체크 하기
    /// </summary>
    void CheckHealStatus()
    {
        if (PV.IsMine)
        {
            if(currentHealth <= maxHealth * 0.3f)
            {
                BloodImage.SetActive(true);
            }
            else
            {
                BloodImage.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 체력킷 먹으면 힐 이펙트
    /// </summary>
    void ShowHealImage()
    {
        if (PV.IsMine)
        {
            HealImage.SetActive(true);
            StartCoroutine(HideHealImageAfterTime(1f));
        }
    }
    
    /// <summary>
    /// 힐 이펙트 끄기
    /// </summary>
    /// <param name="time"> 유지시간 </param>
    /// <returns></returns>
    IEnumerator HideHealImageAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        HealImage.SetActive(false);
    }

}

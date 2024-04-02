using InGame.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        HealImage.SetActive(false); // UI Image 비활성화
        BloodImage.SetActive(false); // UI Image 비활성화
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {

        currentHealth -= damage;
        UpdateHealthBar();
        CheckHealStatus(); // 체력 체크

        //healthbarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            playerManager.CreateKillLog(info.Sender.NickName, PhotonNetwork.NickName);
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }

    }

    public void TakeHeal(float hp)
    {
        currentHealth += hp;

        if (currentHealth > maxHealth) // 조건을 수정하여 체력이 maxHealth를 초과하지 않도록 함
        {
            currentHealth = maxHealth;
        }

        PV.RPC(nameof(RPC_TakeHeal), PV.Owner, currentHealth); // 메소드 이름을 올바르게 수정
    }

    [PunRPC]
    void RPC_TakeHeal(float hp) // 메소드 이름을 올바르게 수정
    {
        currentHealth = hp;
        UpdateHealthBar();
        ShowHealImage(); // 체력 회복 할때 보일 UI Image
        CheckHealStatus(); // 체력 체크
        Debug.Log(currentHealth);
    }

    void Die()
    {
        playerManager.Die();
    }


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
            if(currentHealth <= maxHealth * 0.3f) // 최대 체력의 30% 이하면
            {
                BloodImage.SetActive(true); // UI Image 활성화
            }
            else
            {
                BloodImage.SetActive(false); // 최대 체력 30%보다 높으면 UI Image 비활성화
            }
        }
    }

    /// <summary>
    /// 체력이 회복 될때 나올 UI Image
    /// </summary>
    void ShowHealImage()
    {
        if (PV.IsMine) // 로컬 플레이어 확인
        {
            HealImage.SetActive(true); // 체력 회복 UI Image 활성화
            StartCoroutine(HideHealImageAfterTime(1f)); // 1초뒤 비활성화
        }
    }

    /// <summary>
    /// 일정 시간 뒤에 UI Image 비활성화 시키기.
    /// </summary>
    /// <param name="time"> 시간 정하기. </param>
    /// <returns> 일정 시간 동안 기다리기. </returns>
    IEnumerator HideHealImageAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        HealImage.SetActive(false); // UI Image 비활성화.
    }

}

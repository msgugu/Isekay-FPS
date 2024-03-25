using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    PhotonView PV;
    PlayerManager playerManager;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {

        currentHealth -= damage;

        //healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
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
        Debug.Log(currentHealth);
    }

    void Die()
    {
        playerManager.Die();
    }

   
}

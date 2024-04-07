using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthPack : MonoBehaviour
{
    public float healthAmount = 50f; // 체력 회복 수치
    public float respawnTime = 10.0f; // 힐 팩 리스폰 시간

    private bool isRespawning = false; // 리스폰 시간 다됐는지 체크

    public Image cooldownUI; // 쿨 타임 표시할 UI


    //private bool isCooldown = false; // 힐 팩 리스폰 시간 체크 UI

    /// <summary>
    /// 나중에 추가 할 수 있으면 이펙트랑 사운드 추가
    /// </summary>
    public ParticleSystem pickupEffect; // 힐팩을 플레이어가 먹었을때 나올 이팩트
    //public AudioClip pickupSound;
    //private AudioSource audioSource;

    private void Start()
    {
        if(cooldownUI != null)
        {
            cooldownUI.fillAmount = 0; // UI 숨김
        }
        //audioSource = gameObject.AddComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter called with: " + other.name);
        if (!isRespawning && other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeHeal(healthAmount);
            PlayPickupEffect(other.transform.position);
            StartCoroutine(Respawn()); // 코루틴 Respawn 시작
        }
    }

    private void PlayPickupEffect(Vector3 position)
    {
        if(pickupEffect != null)
        {
            // 파티클 이펙트 플레이어 위치에 생성.
            ParticleSystem effectInstance = Instantiate(pickupEffect, position, Quaternion.identity);
            effectInstance.Play();

            // 파티클 이펙트 효과가 끝나면 오브젝트 삭제.
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }

    IEnumerator Respawn()
    {
        // 리스폰 중
        isRespawning = true;

        // 플레이어가 힐팩 사용 해서 리스폰 중 게임 오브젝트의 랜더러와 콜라이더 비활성화
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        float cooldownRemaining = 0f; // 쿨타임 UI 숨겨져 있는 값

        // 리스폰 시간 만큼 UI 이미지가 점차 생겨남
        while(cooldownRemaining < respawnTime)
        {
            cooldownRemaining += Time.deltaTime;
            cooldownUI.fillAmount = cooldownRemaining / respawnTime;

            yield return null;

        }

        CompleteCooldown();
        
        // 시간 지연 끝나면 렌더러와 콜라이더 다시 활성화
        cooldownUI.fillAmount = 0; // UI 이미지 숨김
        isRespawning = false; // 리스폰 완료

    }

    /// <summary>
    /// 쿨타임 시간이 완료 되면 UI 이미지 다시 숨김
    /// </summary>
    private void CompleteCooldown()
    {
        isRespawning = false;

        if (cooldownUI != null)
        {
            cooldownUI.fillAmount = 0;
        }
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }

}
    // 이거로 적용 시켜 봤는데 힐팩 먹었을때 오브젝트 비활성화는 되는데 다시 활성화가 되지 않음.
    //IEnumerator Respawn()
    //{
    //    // 힐 팩 리스폰
    //    Debug.Log("Respawn started.");
    //    gameObject.SetActive(false);
    //
    //    yield return new WaitForSeconds(respawnTime);
    //
    //    Debug.Log("Respawn complete.");
    //    gameObject.SetActive(true);
    //
    //
    //    // 이펙트 & 사운드
    //    //if(pickupEffect != null)
    //    //{
    //    //    Instantiate(pickupEffect, transform.position, Quaternion.identity);
    //    //}
    //    //
    //    //if(pickupSound != null && audioSource != null)
    //    //{
    //    //    audioSource.PlayOneShot(pickupSound);
    //    //}
    //}


    //private void DeactivateAndRespawn()
    //{
    //    gameObject.SetActive(false);
    //
    //    Invoke("Reactivate", respawnTime);
    //}
    //
    //private void Reactivate()
    //{
    //    gameObject.SetActive(true);
    //}

    
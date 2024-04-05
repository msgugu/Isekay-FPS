using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// 힐 팩
/// </summary>
public class HealthPack : MonoBehaviour
{
    /// <summary>
    /// 체력 회복 수치
    /// </summary>
    public float healthAmount = 50f; 
    
    /// <summary>
    /// 힐 팩 리스폰 시간
    /// </summary>
    public float respawnTime = 10.0f; 

    /// <summary>
    /// 리스폰 시간 다됐는지 체크
    /// </summary>
    private bool isRespawning = false; 

    /// <summary>
    /// 쿨 타임 표시할 UI
    /// </summary>
    public Image cooldownUI; 


    //private bool isCooldown = false; // 힐 팩 리스폰 시간 체크 UI

    /// <summary>
    /// 힐 팩 파티클 이펙트
    /// </summary>
    
    // 힐팩을 플레이어가 먹었을때 나올 이팩트
    public ParticleSystem pickupEffect; 
    
    //public AudioClip pickupSound;
    //private AudioSource audioSource;

    private void Start()
    {
        if(cooldownUI != null)
        {
            // UI 숨김
            cooldownUI.fillAmount = 0; 
        }
        //audioSource = gameObject.AddComponent<AudioSource>();
    }


    /// <summary>
    /// 플레이어가 힐 팩 오브젝트에 콜라이더 범위 안에 들어오면 활성화
    /// </summary>
    /// <param name="other"> 콜라이더 감별 </param>
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter called with: " + other.name);
        // 들어온 콜라이더의 태그가 플레이어 또는 isRespawning이 아니라면
        if (!isRespawning && other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeHeal(healthAmount);
            
            // 파티클 이펙트 재생.
            PlayPickupEffect(other.transform.position); 
            
            // 코루틴 Respawn 시작
            StartCoroutine(Respawn()); 
        }
    }

    /// <summary>
    /// 배치 되어있는 힐 팩을 먹었을때 이펙트
    /// </summary>
    /// <param name="position"> 이펙트 생성 위치 </param>
    private void PlayPickupEffect(Vector3 position)
    {
        if(pickupEffect != null)
        {
            // 이펙트 생성이 힐 팩 위치에서 생성.
            ParticleSystem effectInstance = Instantiate(pickupEffect, position, Quaternion.identity);
            effectInstance.Play();

            // 파티클 이펙트 효과가 끝나면 오브젝트 삭제.
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }

    /// <summary>
    /// 힐 팩 리스폰
    /// </summary>
    /// <returns></returns>
    IEnumerator Respawn()
    {
        // 리스폰 중
        isRespawning = true;

        // 플레이어가 힐팩 사용 해서 리스폰 중 게임 오브젝트의 랜더러와 콜라이더 비활성화
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        // 쿨타임 UI 숨겨져 있는 값
        float cooldownRemaining = 0f; 

        // 리스폰 시간 만큼 UI 이미지가 점차 생겨남
        while(cooldownRemaining < respawnTime)
        {
            cooldownRemaining += Time.deltaTime;
            cooldownUI.fillAmount = cooldownRemaining / respawnTime;

            yield return null;

        }
        
        CompleteCooldown();
        //cooldownUI.fillAmount = 0; // UI 이미지 숨김
        //isRespawning = false; // 리스폰 완료

    }

    /// <summary>
    /// 쿨타임 시간이 완료 되면 UI 이미지 다시 숨기고 렌더러와 콜라이더 활성화.
    /// </summary>
    private void CompleteCooldown()
    {
        // 리스폰 완료.
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

    
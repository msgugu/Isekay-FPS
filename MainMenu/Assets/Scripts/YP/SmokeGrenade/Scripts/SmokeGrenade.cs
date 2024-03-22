using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    public SmokeGrenadeData smokeGrenadeData; // ScriptableObject로 변경

    private float countdown; // 폭발까지 남은 시간  
    private bool hasExploded = false; // 폭발 여부
    private GameObject smokeEffectInstance; // 생성된 폭발 효과 인스턴스

    private void Start()
    {
        countdown = smokeGrenadeData.smokeDelay; // 폭발 딜레이 초기화
    }

    private void Update()
    {
        if (!hasExploded)
        {
            countdown -= Time.deltaTime; // 카운트다운 감소
            if (countdown <= 0f) // 딜레이 종료 시
            {   
                hasExploded = true; // 폭발 플래그 설정
                Explode(); // 폭발 실행
            }
        }
    }

    void Explode()
    {
        // 폭발 효과 생성
        smokeEffectInstance = Instantiate(smokeGrenadeData.smokeEffectPrefab, transform.position + smokeGrenadeData.smokeParticleOffset, Quaternion.identity);
        Destroy(gameObject, 4f); // 발사체 파괴
    }

    private void FixedUpdate()
    {
        // 폭발 효과가 생성되고 소멸되었는지 검사
        if (smokeEffectInstance != null && !smokeEffectInstance.activeSelf)
        {
            Destroy(smokeEffectInstance); // 폭발 효과 인스턴스 파괴
            //Destroy(gameObject); // 발사체 파괴
        }
    }
}

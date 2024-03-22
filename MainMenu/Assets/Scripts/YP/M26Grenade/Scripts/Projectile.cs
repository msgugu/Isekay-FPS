using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileData projectileData; // ScriptableObject로 변경

    private float countdown; // 폭발까지 남은 시간  
    private bool hasExploded = false; // 폭발 여부
    private AudioSource audiosource; // AudioSource 컴포넌트

    private void Start()
    {
        countdown = projectileData.explosionDelay; // 폭발 딜레이 초기화
        audiosource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
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
        GameObject explosionEffect = Instantiate(projectileData.explosionEffectPrefab, transform.position + projectileData.explosionParticleOffset, Quaternion.identity);
        Destroy(explosionEffect, 4f); // 일정 시간 후에 폭발 효과 제거

        NearbyForceApply(); // 폭발 주변에 힘 적용
        ApplyDamage(); // 폭발로 인한 데미지 적용

        Destroy(gameObject); // 발사체 파괴
    }

    void NearbyForceApply()
    {
        // 폭발 범위 내의 콜라이더들 검출
        Collider[] colliders = Physics.OverlapSphere(transform.position, projectileData.explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 폭발 힘 적용
                rb.AddExplosionForce(projectileData.explosionForce, transform.position, projectileData.explosionRadius);
            }
        }
    }

    void ApplyDamage()
    {
        // 폭발 범위 내의 모든 리지드바디들을 검출
        Collider[] colliders = Physics.OverlapSphere(transform.position, projectileData.explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
           
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
               
                ApplyExplosionForce(rb);
            }
        }
    }

    void ApplyExplosionForce(Rigidbody rb)
    {
        // 폭발 힘 적용
        Vector3 explosionDirection = rb.position - transform.position;
        float distance = explosionDirection.magnitude;
        float explosionImpact = (1 - (distance / projectileData.explosionRadius)) * projectileData.explosionForce;

        // 폭발 힘을 리지드바디에 적용
        rb.AddForce(explosionDirection.normalized * explosionImpact, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        audiosource.spatialBlend = 1;
        audiosource.Play(); // 충돌 시 효과음 재생
    }
}
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileData projectileData; // ScriptableObject�� ����

    private float countdown; // ���߱��� ���� �ð�  
    private bool hasExploded = false; // ���� ����
    private AudioSource audiosource; // AudioSource ������Ʈ

    private void Start()
    {
        countdown = projectileData.explosionDelay; // ���� ������ �ʱ�ȭ
        audiosource = GetComponent<AudioSource>(); // AudioSource ������Ʈ ��������
    }

    private void Update()
    {
        if (!hasExploded)
        {
            countdown -= Time.deltaTime; // ī��Ʈ�ٿ� ����
            if (countdown <= 0f) // ������ ���� ��
            {
                hasExploded = true; // ���� �÷��� ����
                Explode(); // ���� ����
            }
        }
    }

    void Explode()
    {
        // ���� ȿ�� ����
        GameObject explosionEffect = Instantiate(projectileData.explosionEffectPrefab, transform.position + projectileData.explosionParticleOffset, Quaternion.identity);
        Destroy(explosionEffect, 4f); // ���� �ð� �Ŀ� ���� ȿ�� ����

        NearbyForceApply(); // ���� �ֺ��� �� ����
        ApplyDamage(); // ���߷� ���� ������ ����

        Destroy(gameObject); // �߻�ü �ı�
    }

    void NearbyForceApply()
    {
        // ���� ���� ���� �ݶ��̴��� ����
        Collider[] colliders = Physics.OverlapSphere(transform.position, projectileData.explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ���� �� ����
                rb.AddExplosionForce(projectileData.explosionForce, transform.position, projectileData.explosionRadius);
            }
        }
    }

    void ApplyDamage()
    {
        // ���� ���� ���� ��� ������ٵ���� ����
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
        // ���� �� ����
        Vector3 explosionDirection = rb.position - transform.position;
        float distance = explosionDirection.magnitude;
        float explosionImpact = (1 - (distance / projectileData.explosionRadius)) * projectileData.explosionForce;

        // ���� ���� ������ٵ� ����
        rb.AddForce(explosionDirection.normalized * explosionImpact, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        audiosource.spatialBlend = 1;
        audiosource.Play(); // �浹 �� ȿ���� ���
    }
}
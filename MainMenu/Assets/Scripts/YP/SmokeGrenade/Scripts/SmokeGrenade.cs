using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    public SmokeGrenadeData smokeGrenadeData; // ScriptableObject�� ����

    private float countdown; // ���߱��� ���� �ð�  
    private bool hasExploded = false; // ���� ����
    private GameObject smokeEffectInstance; // ������ ���� ȿ�� �ν��Ͻ�

    private void Start()
    {
        countdown = smokeGrenadeData.smokeDelay; // ���� ������ �ʱ�ȭ
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
        smokeEffectInstance = Instantiate(smokeGrenadeData.smokeEffectPrefab, transform.position + smokeGrenadeData.smokeParticleOffset, Quaternion.identity);
        Destroy(gameObject, 4f); // �߻�ü �ı�
    }

    private void FixedUpdate()
    {
        // ���� ȿ���� �����ǰ� �Ҹ�Ǿ����� �˻�
        if (smokeEffectInstance != null && !smokeEffectInstance.activeSelf)
        {
            Destroy(smokeEffectInstance); // ���� ȿ�� �ν��Ͻ� �ı�
            //Destroy(gameObject); // �߻�ü �ı�
        }
    }
}

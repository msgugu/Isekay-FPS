using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileData", menuName = "Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Explosion Prefab")]
    public GameObject explosionEffectPrefab; // ���� ȿ���� ������
    public Vector3 explosionParticleOffset = new Vector3(0, 1, 0); // ���� ȿ���� ��ġ ����
    //public GameObject audioSourcePrefab;

    [Header("Explosion Settings")]
    public float explosionDelay = 3f; // ���� ������
    public float explosionForce = 700f; // ���� ��
    public float explosionRadius = 5f; // ���� �ݰ�

    [Header("Damage")]
    public int damage = 10; // ������
}

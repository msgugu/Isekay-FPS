using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileData", menuName = "Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Explosion Prefab")]
    public GameObject explosionEffectPrefab; // Æø¹ß È¿°úÀÇ ÇÁ¸®ÆÕ
    public Vector3 explosionParticleOffset = new Vector3(0, 1, 0); // Æø¹ß È¿°úÀÇ À§Ä¡ Á¶Á¤
    //public GameObject audioSourcePrefab;

    [Header("Explosion Settings")]
    public float explosionDelay = 3f; // Æø¹ß µô·¹ÀÌ
    public float explosionForce = 700f; // Æø¹ß Èû
    public float explosionRadius = 5f; // Æø¹ß ¹Ý°æ

    [Header("Damage")]
    public int damage = 10; // µ¥¹ÌÁö
}

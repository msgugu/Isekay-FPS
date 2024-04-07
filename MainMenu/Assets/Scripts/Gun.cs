using UnityEngine;

/// <summary>
/// 아이템 구분 : 총류
/// </summary>
public abstract class Gun : Item
{
    public enum FireMode { Single, Auto }
    public FireMode fireMode = FireMode.Single;
    public abstract override void Use();

    [Header("Muzzle")]
    public Transform BulletMuzzlePosition;
    public ParticleSystem BulletMuzzleEffect;

    [Header("Impact")]
    public GameObject bulletImpactPrefab;
    public GameObject bulletImpactFlesh;
}

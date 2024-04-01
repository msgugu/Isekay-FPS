using UnityEngine;

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

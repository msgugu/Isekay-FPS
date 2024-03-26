using UnityEngine;

public abstract class Gun : Item
{
    public enum FireMode { Single, Auto }
    public FireMode fireMode = FireMode.Single;
    public abstract override void Use();

    public GameObject bulletImpactPrefab;
}

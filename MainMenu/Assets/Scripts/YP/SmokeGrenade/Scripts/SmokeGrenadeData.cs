using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSmokeGrenadeData", menuName = "SmokeGrenade Data")]
public class SmokeGrenadeData : ItemInfo
{ 
    [Header("Smoke Prefab")]
    public GameObject smokeEffectPrefab; // smokegrenade 효과 프리팹
    public Vector3 smokeParticleOffset = new Vector3(0, 1, 0); // smoke 효과 위치 조정
   // public GameObject audioSourcePrefab;

    [Header("Smoke Settings")]
    public float smokeDelay = 1f;
    

}

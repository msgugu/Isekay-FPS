using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSmokeGrenadeData", menuName = "SmokeGrenade Data")]
public class SmokeGrenadeData : ScriptableObject
{ 
    [Header("Smoke Prefab")]
    public GameObject smokeEffectPrefab; // smokegrenade ȿ�� ������
    public Vector3 smokeParticleOffset = new Vector3(0, 1, 0); // smoke ȿ�� ��ġ ����
   // public GameObject audioSourcePrefab;

    [Header("Smoke Settings")]
    public float smokeDelay = 1f;
    

}

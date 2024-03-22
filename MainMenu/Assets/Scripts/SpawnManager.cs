using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public Spawnpoint[] spawnpoints;

    void Awake()
    {
        instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint()
    {
        if (spawnpoints.Length == 0)
        {
            Debug.LogError("Spawnpoints array is empty.");
            return null; // 또는 기본 Transform 반환
        }
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 총 데이터 
/// </summary>
[CreateAssetMenu(menuName ="FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float bamage;
    public float fireRate;
    public float nextFireTime;
    public int bullet;
    public float reloadTime;

    //public GameObject fireEffect;
    //public AudioClip fireAudio;

}

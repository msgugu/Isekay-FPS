using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 Offset;


    void Start()
    {
        
    }

    void Update()
    {
        transform.position = target.transform.position + Offset; 
    }
}

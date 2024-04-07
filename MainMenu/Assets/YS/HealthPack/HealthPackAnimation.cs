using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackAnimation : MonoBehaviour
{
    public float rotationSpeed = 50.0f;

    //private Vector3 startPosition;

    void Start()
    {
        //startPosition = transform.position;
    }

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}

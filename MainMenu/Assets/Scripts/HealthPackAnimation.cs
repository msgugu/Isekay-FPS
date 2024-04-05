using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 힐 팩 오브젝트 Y축 기준으로 회전.
/// </summary>
public class HealthPackAnimation : MonoBehaviour
{
    /// <summary>
    /// 회전 속도.
    /// </summary>
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

    /// <summary>
    /// 회전 함수
    /// </summary>
    private void Rotate()
    {
        // 월드 좌표에서 Y축 기준으로 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Throwing_L : MonoBehaviour
{
    [Header("References")]
    public Transform cam; // 카메라의 위치와 방향을 가져오기 위한 참조
    public Transform attackPoint; // 던질 때 오브젝트가 생성될 위치
    public GameObject objectToThrow; // 던질 오브젝트

    [Header("Settings")]
    public int totalThrows; // 총 던질 횟수
    public float throwCooldown; // 던지는 간격

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0; // 던지기를 위한 키
    public float throwForce; // 던질 힘
    public float throwUpwardForce; // 위로 던질 힘

    public LineRenderer trajectoryLine; // 수류탄 궤적을 그리기 위한 라인 렌더러

    bool readyToThrow; // 던지기 가능한지 여부를 나타내는 플래그

    private void Start()
    {
        readyToThrow = true; // 시작할 때 던지기 가능하도록 플래그 설정
    }

    private void Update()
    {
        // 던지기 키를 눌렀고, 던질 수 있으며, 던질 횟수가 남았을 때
        if (Input.GetKeyUp(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw(); // 던지기 함수 호출
        }

        // 수류탄 궤적을 업데이트
        UpdateTrajectory();
    }

    private void Throw()
    {
        readyToThrow = false; // 던지기 불가능하도록 플래그 설정

        // 오브젝트를 던지기 위해 새로운 오브젝트를 생성
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // 던질 오브젝트의 Rigidbody 컴포넌트 가져오기
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // 던질 방향 계산
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        // 카메라 방향으로 Raycast를 쏴서 충돌하는 경우가 있으면 해당 지점으로 던짐
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // 던질 힘 계산
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        // 힘을 추가하여 던짐
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--; // 던진 횟수 감소

        // 던지기 쿨다운 적용
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true; // 다시 던지기 가능하도록 플래그 설정
    }

    private void UpdateTrajectory()
    {
        // 수류탄 궤적을 계산하여 Line Renderer로 그립니다.
        Vector3 initialPosition = attackPoint.position;
        Vector3 initialVelocity = cam.transform.forward * throwForce + transform.up * throwUpwardForce;
        int numPoints = 30; // 궤적을 그릴 점의 수

        Vector3[] positions = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            // 현재 시간에 따른 위치 계산
            float time = i / (float)numPoints;
            positions[i] = initialPosition + initialVelocity * time + Physics.gravity * time * time / 2f;
        }

        // Line Renderer에 궤적 설정
        trajectoryLine.positionCount = numPoints;
        trajectoryLine.SetPositions(positions);
    }
}

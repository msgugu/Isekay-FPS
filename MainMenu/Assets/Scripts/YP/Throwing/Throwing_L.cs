using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Throwing_L : MonoBehaviour
{
    [Header("References")]
    public Transform cam; // ī�޶��� ��ġ�� ������ �������� ���� ����
    public Transform attackPoint; // ���� �� ������Ʈ�� ������ ��ġ
    public GameObject objectToThrow; // ���� ������Ʈ

    [Header("Settings")]
    public int totalThrows; // �� ���� Ƚ��
    public float throwCooldown; // ������ ����

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0; // �����⸦ ���� Ű
    public float throwForce; // ���� ��
    public float throwUpwardForce; // ���� ���� ��

    public LineRenderer trajectoryLine; // ����ź ������ �׸��� ���� ���� ������

    bool readyToThrow; // ������ �������� ���θ� ��Ÿ���� �÷���

    private void Start()
    {
        readyToThrow = true; // ������ �� ������ �����ϵ��� �÷��� ����
    }

    private void Update()
    {
        // ������ Ű�� ������, ���� �� ������, ���� Ƚ���� ������ ��
        if (Input.GetKeyUp(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw(); // ������ �Լ� ȣ��
        }

        // ����ź ������ ������Ʈ
        UpdateTrajectory();
    }

    private void Throw()
    {
        readyToThrow = false; // ������ �Ұ����ϵ��� �÷��� ����

        // ������Ʈ�� ������ ���� ���ο� ������Ʈ�� ����
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // ���� ������Ʈ�� Rigidbody ������Ʈ ��������
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // ���� ���� ���
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        // ī�޶� �������� Raycast�� ���� �浹�ϴ� ��찡 ������ �ش� �������� ����
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // ���� �� ���
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        // ���� �߰��Ͽ� ����
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--; // ���� Ƚ�� ����

        // ������ ��ٿ� ����
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true; // �ٽ� ������ �����ϵ��� �÷��� ����
    }

    private void UpdateTrajectory()
    {
        // ����ź ������ ����Ͽ� Line Renderer�� �׸��ϴ�.
        Vector3 initialPosition = attackPoint.position;
        Vector3 initialVelocity = cam.transform.forward * throwForce + transform.up * throwUpwardForce;
        int numPoints = 30; // ������ �׸� ���� ��

        Vector3[] positions = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            // ���� �ð��� ���� ��ġ ���
            float time = i / (float)numPoints;
            positions[i] = initialPosition + initialVelocity * time + Physics.gravity * time * time / 2f;
        }

        // Line Renderer�� ���� ����
        trajectoryLine.positionCount = numPoints;
        trajectoryLine.SetPositions(positions);
    }
}

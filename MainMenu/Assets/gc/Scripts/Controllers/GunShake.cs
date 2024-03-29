using Isekai.GC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShake : MonoBehaviour
{
    public float recoilForce = 1f; // 총 반동 힘의 크기
    public float recoilDuration = 0.1f; // 반동 지속시간

    private bool isRecoiling = false;
    private Vector3 originalPosition;

    private void Update()
    {
        if (Input.GetMouseButton(0)) // 마우스 버튼을 누르는 순간에만 발사 이벤트 발생
        {
            Fire();
        }
    }

    // 총 발사시 호출되는 함수
    public void Fire()
    {
        if (!isRecoiling)
        {
            // 일시적으로 총을 앞으로 향하게 이동시킴
            originalPosition = transform.localPosition;
            Vector3 recoilPosition = originalPosition + new Vector3(0f, 0f, -recoilForce);
            StartCoroutine(Recoil(recoilPosition));
        }
    }

    // 반동 코루틴
    IEnumerator Recoil(Vector3 targetPosition)
    {
        isRecoiling = true;
        float elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 반동이 완료된 후에는 원래 위치로 돌아감
        transform.localPosition = originalPosition;
        isRecoiling = false;
    }
}
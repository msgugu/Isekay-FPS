using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Range(0, 100)]              // 0부터 100 사이의 범위로 Value 변수를 조절하는 Range 속성
    public float Croshairvalue;  // croshair의 확장 값을 나타내는 변수
    public float Croshairspeed;  // croshair 부분이 목표 위치로 이동하는 속도

    public float Margin;         // croshair 각 부분과 중심 사이의 default 거리

    // 크로스헤어 각 부분의 RectTransform에 대한 참조(UI_image)
    public RectTransform Top, Bottom, Left, Right, Center;

    public void OnShoot()
    {
        Croshairvalue += 10; // 또는 적당한 값을 증가시킵니다.
        Croshairvalue = Mathf.Clamp(Croshairvalue, 0, 100); // 값의 상한을 제한합니다.
        StopAllCoroutines(); // 현재 진행 중인 모든 코루틴을 멈춥니다.
        StartCoroutine(ResetCrosshairAfterShoot()); // 크로스헤어 리셋 코루틴을 시작합니다.
    }

    IEnumerator ResetCrosshairAfterShoot()
    {
        float startValue = Croshairvalue;
        float duration = 1.0f / Croshairspeed;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Croshairvalue = Mathf.Lerp(startValue, 0, elapsed / duration);
            UpdateCrosshairPositions();
            yield return null;
        }

        Croshairvalue = 0;
        UpdateCrosshairPositions();
    }

    public void UpdateCrosshairPositions()
    {
        Top.position = new Vector2(Center.position.x, Center.position.y + Margin + Croshairvalue);
        Bottom.position = new Vector2(Center.position.x, Center.position.y - Margin - Croshairvalue);

        Left.position = new Vector2(Center.position.x - Margin - Croshairvalue, Center.position.y);
        Right.position = new Vector2(Center.position.x + Margin + Croshairvalue, Center.position.y);
    }

}

using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed = 5f; // 이동 속도

    void Update()
    {
        // 사용자 입력을 받아 움직임 처리
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed * Time.deltaTime;

        // 현재 위치에 이동량을 더하여 새 위치 계산
        transform.Translate(movement);
    }
}

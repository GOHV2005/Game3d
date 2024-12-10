using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catch : MonoBehaviour
{
    public float moveSpeed = 250f; // Tốc độ di chuyển
    public float maxLeft = -250f; // Giới hạn bên trái
    public float maxRight = 250f; // Giới hạn bên phải

    void Update()
    {
        // Lấy giá trị đầu vào từ trục ngang (A/D hoặc phím mũi tên trái/phải)
        float moveInput = Input.GetAxis("Horizontal");

        // Nếu có input, thực hiện di chuyển
        if (moveInput != 0)
        {
            MoveCatcher(moveInput);
        }
    }

    private void MoveCatcher(float moveInput)
    {
        // Tính toán di chuyển
        Vector3 movement = Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        // Tính toán vị trí mới
        Vector3 newPosition = transform.localPosition + movement;

        // Giới hạn vị trí trong khoảng maxLeft và maxRight
        newPosition.x = Mathf.Clamp(newPosition.x, maxLeft, maxRight);

        // Cập nhật vị trí
        transform.localPosition = newPosition;
    }
}

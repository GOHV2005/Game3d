using UnityEngine;

public class FishMove : MonoBehaviour
{
    public float maxLeft = -250f; // Giới hạn bên trái
    public float maxRight = 250f; // Giới hạn bên phải
    public float moveSpeed = 250f; // Tốc độ di chuyển
    public float changeFrequency = 0.03f; // Tần suất thay đổi hướng ngẫu nhiên

    private float targetPosition; // Vị trí mục tiêu
    private bool movingRight; // Đang di chuyển về bên phải?

    void Start()
    {
        // Chọn mục tiêu ngẫu nhiên ban đầu
        targetPosition = Random.Range(maxLeft, maxRight);
        movingRight = targetPosition > transform.position.x; // Xác định hướng ban đầu
    }

    void Update()
    {
        // Di chuyển cá tới vị trí mục tiêu
        transform.localPosition = Vector3.MoveTowards(transform.localPosition,
            new Vector3(targetPosition, transform.localPosition.y, transform.localPosition.z),
            moveSpeed * Time.deltaTime
        );

        // Kiểm tra nếu cá đã gần đến vị trí mục tiêu
        if (Mathf.Approximately(transform.localPosition.x, targetPosition))
        {
            // Chọn mục tiêu mới khi đến gần mục tiêu
            targetPosition = Random.Range(maxLeft, maxRight);
        }

        // Thay đổi hướng di chuyển ngẫu nhiên
        if (Random.value < changeFrequency)
        {
            movingRight = !movingRight; // Đảo hướng
            targetPosition = movingRight ? maxRight : maxLeft; // Cập nhật vị trí mục tiêu theo hướng mới
        }
    }
}

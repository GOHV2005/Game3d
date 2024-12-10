using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển
    public float turnSmoothTime = 0.1f; // Thời gian mượt mà khi xoay
    private float turnSmoothVelocity; // Tốc độ xoay
    public float jumpForce = 5f; // Lực nhảy
    public Transform cameraTransform; // Camera theo dõi nhân vật

    private Rigidbody rb;
    private bool isGrounded;

    private Vector3 movementDirection; // Hướng di chuyển cuối cùng
    private float currentRotationAngle;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Lấy Rigidbody của nhân vật
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Lấy camera chính nếu chưa gán
        }
    }

    void Update()
    {
        // Nhận đầu vào di chuyển
        float horizontal = Input.GetAxis("Horizontal"); // A/D hoặc phím mũi tên trái/phải
        float vertical = Input.GetAxis("Vertical"); // W/S hoặc phím mũi tên lên/xuống

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Tính góc xoay theo hướng camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Xoay nhân vật
            transform.rotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

            // Tính toán hướng di chuyển cuối cùng
            movementDirection = Quaternion.Euler(0f, currentRotationAngle, 0f) * Vector3.forward;
        }
        else
        {
            movementDirection = Vector3.zero; // Dừng khi không có input
        }

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật bằng Rigidbody
        if (movementDirection.magnitude >= 0.1f)
        {
            rb.MovePosition(rb.position + movementDirection.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Kiểm tra khi nhân vật chạm đất
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

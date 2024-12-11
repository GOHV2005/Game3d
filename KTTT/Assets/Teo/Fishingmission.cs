using UnityEngine;
using TMPro;

public class FishingTaskTracker : MonoBehaviour
{
    public TextMeshProUGUI taskProgressText; // UI hiển thị tiến trình nhiệm vụ

    private int fishCaught = 0; // Số lượng cá đã câu được
    private const int targetFishCount = 3; // Mục tiêu số lượng cá cần để hoàn thành nhiệm vụ

    public void AddFishCount()
    {
        // Tăng số lượng cá đã câu được
        fishCaught++;
        UpdateTaskProgressUI();

        // Kiểm tra nếu đạt mục tiêu
        if (fishCaught >= targetFishCount)
        {
            CompleteTask();
        }
    }

    private void UpdateTaskProgressUI()
    {
        // Cập nhật UI hiển thị số lượng cá
        taskProgressText.text = $"Cá đã câu: {fishCaught}/{targetFishCount}";
    }

    private void CompleteTask()
    {
        // Thông báo nhiệm vụ đã hoàn thành
        Debug.Log("Nhiệm vụ câu cá hoàn thành!");
        taskProgressText.text = "Nhiệm vụ đã hoàn thành!";

        // Thực hiện các hành động khác khi hoàn thành nhiệm vụ nếu cần
    }

    public void ResetTask()
    {
        // Đặt lại tiến trình nhiệm vụ
        fishCaught = 0;
        UpdateTaskProgressUI();
    }
}

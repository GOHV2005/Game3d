using System.Collections;
using TMPro;
using UnityEngine;

public class FishingMission : MonoBehaviour
{
    public NPC npcScript; // Tham chiếu đến script NPC để cập nhật nhiệm vụ
    public TextMeshProUGUI fishCountText; // UI hiển thị số lượng cá đã câu được

    private int fishCaught = 0; // Số lượng cá đã câu được
    private int fishTarget = 3; // Mục tiêu số lượng cá cần để hoàn thành nhiệm vụ

    private void Start()
    {
        UpdateFishCountUI(); // Cập nhật UI ban đầu
    }

    // Hàm gọi khi người chơi câu được cá
    public void OnFishCaught()
    {
        fishCaught++;
        UpdateFishCountUI();

        // Kiểm tra nếu người chơi đạt mục tiêu
        if (fishCaught >= fishTarget)
        {
            CompleteFishingMission();
        }
    }

    // Hàm để cập nhật số lượng cá trên UI
    private void UpdateFishCountUI()
    {
        fishCountText.text = $"Cá đã câu: {fishCaught}/{fishTarget}";
    }

    // Hàm để đánh dấu nhiệm vụ hoàn thành
    private void CompleteFishingMission()
    {
        Debug.Log("Nhiệm vụ câu cá hoàn thành!");

        // Gọi hàm CompleteMission trong script NPC
        if (npcScript != null)
        {
            npcScript.CompleteMission();
        }

        // Cập nhật UI hoặc các hành động khác nếu cần
        fishCountText.text = "Nhiệm vụ đã hoàn thành!";
    }

    // Hàm để reset số lượng cá nếu cần thiết (cho mục đích phát triển hoặc tái sử dụng nhiệm vụ)
    public void ResetFishingMission()
    {
        fishCaught = 0;
        UpdateFishCountUI();

        if (npcScript != null)
        {
            npcScript.ResetMission();
        }
    }
}

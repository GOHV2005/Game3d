using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public FishingMinigame fishingMinigame;  // Tham chiếu đến FishingMinigame
    private bool isInFishingArea = false;     // Kiểm tra xem có trong vùng fishing hay không
    private bool isFishingMinigameActive = false; // Kiểm tra trạng thái FishingMinigameController

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fishing"))
        {
            isInFishingArea = true; // Đặt trạng thái là đang trong vùng fishing
            StartCoroutine(DelayedDebugLog());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fishing"))
        {
            isInFishingArea = false; // Đặt trạng thái là không còn trong vùng fishing
        }
    }


    private IEnumerator DelayedDebugLog()
    {
        // Thời gian chờ ngẫu nhiên từ 1 đến 5 giây
        float delayTime = Random.Range(1f, 10f);
        yield return new WaitForSeconds(delayTime);

        // Kiểm tra nếu đối tượng vẫn trong vùng fishing trước khi kích hoạt minigame
        if (isInFishingArea)
        {
            Debug.Log("Fishing");

            // Kích hoạt minigame
            fishingMinigame.ActivateMinigame();
        }
        else
        {
            Debug.Log("Exited fishing area before activation.");
        }
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    // Các tham chiếu đến UI
    public RectTransform fishingBar;   // Thanh điều khiển
    public RectTransform catchBar;    // Khu vực bắt
    public RectTransform sweetSpot;   // Vùng "Sweet Spot"
    public Slider progressBar;        // Thanh tiến trình

    // Các tham số điều chỉnh tốc độ
    public float fishingBarSpeed = 200f;       // Tốc độ thanh điều khiển
    public float sweetSpotSpeed = 50f;         // Tốc độ di chuyển Sweet Spot
    public float sweetSpotSpeedIncreaseRate = 5f; // Tốc độ tăng dần của Sweet Spot

    // Tham chiếu đến UI
    public GameObject minigameUI;      // Giao diện minigame
    public GameObject fishingResultUI; // Giao diện kết quả
    public Image fishingWinImage;      // Hình ảnh khi thắng
    public Image fishingFailImage;     // Hình ảnh khi thua

    // Thêm một mảng ảnh để hiển thị khi thắng
    public Sprite[] fishingWinImages;  // Mảng ảnh thắng

    // Các biến điều khiển
    private bool isFishingBarInSweetSpot = false;  // Kiểm tra thanh điều khiển có trong Sweet Spot không
    private bool isPlayerControlling = false;      // Kiểm tra người chơi có đang điều khiển không
    private float progressBarSpeed = 0.1f;         // Tốc độ tăng/giảm của thanh tiến trình

    private bool isGameActive = true; // Trạng thái trò chơi đang hoạt động

    private void Start()
    {
        fishingResultUI.SetActive(false); // Ẩn giao diện kết quả khi bắt đầu
    }

    private void Update()
    {
        if (isGameActive)
        {
            HandleFishingBar();        // Xử lý thanh điều khiển
            HandleSweetSpot();         // Xử lý vùng Sweet Spot
            CheckFishingBarInSweetSpot(); // Kiểm tra vị trí thanh điều khiển
            UpdateProgressBar();       // Cập nhật thanh tiến trình
        }
    }

    public void ActivateMinigame()
    {
        // Bắt đầu minigame từ trạng thái ban đầu
        minigameUI.SetActive(true);   // Hiển thị giao diện minigame

        // Reset giá trị cho lần chơi
        fishingBar.anchoredPosition = Vector2.zero;  // Đặt thanh điều khiển về vị trí gốc
        progressBar.value = progressBar.minValue;    // Đặt lại thanh tiến trình về 0
        sweetSpot.anchoredPosition = new Vector2(Random.Range(-100f, 100f), 0f); // Đặt vị trí Sweet Spot ngẫu nhiên

        isGameActive = true; // Kích hoạt trò chơi
    }

    void HandleFishingBar()
    {
        // Điều khiển thanh câu bằng phím Q và E
        if (Input.GetKey(KeyCode.Q))
        {
            isPlayerControlling = true;
            fishingBar.anchoredPosition += new Vector2(-fishingBarSpeed * Time.deltaTime, 0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            isPlayerControlling = true;
            fishingBar.anchoredPosition += new Vector2(fishingBarSpeed * Time.deltaTime, 0f);
        }
        else
        {
            isPlayerControlling = false;
        }

        // Giới hạn phạm vi di chuyển của thanh điều khiển
        float catchBarLeftLimit = -catchBar.rect.width / 2 + fishingBar.rect.width / 2;
        float catchBarRightLimit = catchBar.rect.width / 2 - fishingBar.rect.width / 2;

        if (fishingBar.anchoredPosition.x > catchBarRightLimit)
        {
            fishingBar.anchoredPosition = new Vector2(catchBarRightLimit, fishingBar.anchoredPosition.y);
        }
        else if (fishingBar.anchoredPosition.x < catchBarLeftLimit)
        {
            fishingBar.anchoredPosition = new Vector2(catchBarLeftLimit, fishingBar.anchoredPosition.y);
        }

        // Nếu không điều khiển, thanh tự động di chuyển
        if (!isPlayerControlling)
        {
            fishingBar.anchoredPosition += new Vector2(fishingBarSpeed * Time.deltaTime, 0f);

            if (fishingBar.anchoredPosition.x > catchBarRightLimit || fishingBar.anchoredPosition.x < catchBarLeftLimit)
            {
                fishingBarSpeed = -fishingBarSpeed;
            }
        }
    }

    void HandleSweetSpot()
    {
        // Tăng dần tốc độ Sweet Spot
        sweetSpotSpeed += sweetSpotSpeedIncreaseRate * Time.deltaTime;

        Vector2 position = sweetSpot.anchoredPosition;
        position.x += sweetSpotSpeed * Time.deltaTime;

        float catchBarLeftLimit = -catchBar.rect.width / 2 + sweetSpot.rect.width / 2;
        float catchBarRightLimit = catchBar.rect.width / 2 - sweetSpot.rect.width / 2;

        if (position.x > catchBarRightLimit || position.x < catchBarLeftLimit)
        {
            sweetSpotSpeed = -sweetSpotSpeed;
        }

        sweetSpot.anchoredPosition = position;
    }

    void CheckFishingBarInSweetSpot()
    {
        float fishingBarLeft = fishingBar.anchoredPosition.x - fishingBar.rect.width / 2;
        float fishingBarRight = fishingBar.anchoredPosition.x + fishingBar.rect.width / 2;
        float sweetSpotLeft = sweetSpot.anchoredPosition.x - sweetSpot.rect.width / 2;
        float sweetSpotRight = sweetSpot.anchoredPosition.x + sweetSpot.rect.width / 2;

        isFishingBarInSweetSpot = fishingBarRight > sweetSpotLeft && fishingBarLeft < sweetSpotRight;
    }

    void UpdateProgressBar()
    {
        if (isFishingBarInSweetSpot)
        {
            progressBar.value += progressBarSpeed * Time.deltaTime;

            if (progressBar.value >= progressBar.maxValue)
            {
                Debug.Log("Fishing Completed!");
                ShowResult(true);
            }
        }
        else
        {
            progressBar.value -= progressBarSpeed * Time.deltaTime;

            if (progressBar.value <= progressBar.minValue)
            {
                Debug.Log("Fishing Failed!");
                ShowResult(false);
            }
        }
    }

    void ShowResult(bool isWin)
    {
        minigameUI.SetActive(false);
        fishingResultUI.SetActive(true);

        if (isWin)
        {
            DisplayRandomWinImage();
            fishingFailImage.gameObject.SetActive(false);
        }
        else
        {
            fishingFailImage.gameObject.SetActive(true);
            fishingWinImage.gameObject.SetActive(false);
        }

        isGameActive = false; // Dừng trò chơi
        StartCoroutine(HideResultAfterDelay());
    }

    void DisplayRandomWinImage()
    {
        if (fishingWinImages.Length > 0)
        {
            int randomIndex = Random.Range(0, fishingWinImages.Length);
            fishingWinImage.sprite = fishingWinImages[randomIndex];
            fishingWinImage.gameObject.SetActive(true);
        }
    }

    IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        // Ẩn kết quả sau 3 giây
        fishingResultUI.SetActive(false);
        fishingWinImage.gameObject.SetActive(false);
        fishingFailImage.gameObject.SetActive(false);

        Debug.Log("Minigame Ended.");
    }
}

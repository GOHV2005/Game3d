using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    // Các tham chiếu đến UI
    public RectTransform fishingBar;
    public RectTransform catchBar;
    public RectTransform sweetSpot;
    public Slider progressBar;
    public static int fishCaught = 0;

    // Các tham số điều chỉnh tốc độ
    public float fishingBarSpeed = 200f;
    public float sweetSpotSpeed = 50f;
    public float sweetSpotSpeedIncreaseRate = 5f;

    // Tham chiếu đến UI
    public GameObject minigameUI;
    public GameObject fishingResultUI;
    public Image fishingWinImage;
    public Image fishingFailImage;
    public Button retryButton;

    // Thêm một mảng ảnh để hiển thị khi thắng
    public Sprite[] fishingWinImages; // Mảng ảnh thắng
    private Image currentWinImage; // Biến để lưu ảnh thắng hiện tại

    // Các biến điều khiển
    private bool isFishingBarInSweetSpot = false;
    private bool isPlayerControlling = false;
    private float progressBarSpeed = 0.1f;

    public bool isGameActive = true; // Kiểm tra trò chơi có đang hoạt động không

    private void Start()
    {
        fishingResultUI.SetActive(false);
        retryButton.gameObject.SetActive(false);
        retryButton.onClick.AddListener(RetryMinigame);
    }

    private void Update()
    {
        if (isGameActive)
        {
            HandleFishingBar();
            HandleSweetSpot();
            CheckFishingBarInSweetSpot();
            UpdateProgressBar();
        }
    }

    public void ActivateMinigame()
    {
        // Đảm bảo rằng minigame bắt đầu lại từ đầu và không hiển thị kết quả trước đó
        minigameUI.SetActive(true);

        // Reset các giá trị cần thiết cho vòng chơi mới
        fishingBar.anchoredPosition = Vector2.zero;  // Đặt lại vị trí của thanh câu
        progressBar.value = progressBar.minValue;    // Đặt lại thanh tiến trình về 0
        sweetSpot.anchoredPosition = new Vector2(Random.Range(-100f, 100f), 0f); // Đặt lại vị trí sweet spot

        // Cần đảm bảo là minigame không bị giữ lại trạng thái từ lần trước
        isGameActive = true;
    }

    void HandleFishingBar()
    {
        if (Input.GetKey(KeyCode.E))
        {
            isPlayerControlling = true;
            fishingBar.anchoredPosition += new Vector2(-fishingBarSpeed * Time.deltaTime, 0f);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            isPlayerControlling = true;
            fishingBar.anchoredPosition += new Vector2(fishingBarSpeed * Time.deltaTime, 0f);
        }
        else
        {
            isPlayerControlling = false;
        }

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

        if (!isPlayerControlling)
        {
            fishingBar.anchoredPosition += new Vector2(fishingBarSpeed * Time.deltaTime, 0f);

            if (fishingBar.anchoredPosition.x > catchBarRightLimit)
            {
                fishingBar.anchoredPosition = new Vector2(catchBarRightLimit, fishingBar.anchoredPosition.y);
                fishingBarSpeed = -fishingBarSpeed;
            }
            else if (fishingBar.anchoredPosition.x < catchBarLeftLimit)
            {
                fishingBar.anchoredPosition = new Vector2(catchBarLeftLimit, fishingBar.anchoredPosition.y);
                fishingBarSpeed = -fishingBarSpeed;
            }
        }
    }

    void HandleSweetSpot()
    {
        Vector2 position = sweetSpot.anchoredPosition;

        sweetSpotSpeed += sweetSpotSpeedIncreaseRate * Time.deltaTime;

        float catchBarLeftLimit = -catchBar.rect.width / 2 + sweetSpot.rect.width / 2;
        float catchBarRightLimit = catchBar.rect.width / 2 - sweetSpot.rect.width / 2;

        if (Random.Range(0f, 1f) < 0.01f)
        {
            sweetSpotSpeed = -sweetSpotSpeed;
        }

        position.x += sweetSpotSpeed * Time.deltaTime;

        if (position.x > catchBarRightLimit)
        {
            position.x = catchBarRightLimit;
            sweetSpotSpeed = -Mathf.Abs(sweetSpotSpeed);
        }
        else if (position.x < catchBarLeftLimit)
        {
            position.x = catchBarLeftLimit;
            sweetSpotSpeed = Mathf.Abs(sweetSpotSpeed);
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

                minigameUI.SetActive(false);
                fishingResultUI.SetActive(true);
                fishCaught++;
                // Chọn ngẫu nhiên ảnh thắng
                DisplayRandomWinImage();

                fishingFailImage.gameObject.SetActive(false);
                retryButton.gameObject.SetActive(true);  // Hiển thị nút Retry sau khi thắng
                isGameActive = false;  // Ngừng trò chơi sau khi thắng
            }
        }
        else
        {
            progressBar.value -= progressBarSpeed * Time.deltaTime;

            if (progressBar.value < progressBar.minValue)
            {
                progressBar.value = progressBar.minValue;
            }

            if (progressBar.value <= progressBar.minValue)
            {
                Debug.Log("Fishing Failed!");

                minigameUI.SetActive(false);
                fishingResultUI.SetActive(true);

                fishingFailImage.gameObject.SetActive(true);
                fishingWinImage.gameObject.SetActive(false);

                retryButton.gameObject.SetActive(true);  // Hiển thị nút Retry sau khi thua
                isGameActive = false;  // Ngừng trò chơi sau khi thua
            }
        }
    }

    void DisplayRandomWinImage()
    {
        if (fishingWinImages.Length > 0)
        {
            int randomIndex = Random.Range(0, fishingWinImages.Length);
            fishingWinImage.sprite = fishingWinImages[randomIndex];
            fishingWinImage.gameObject.SetActive(true);  // Hiển thị ảnh thắng
        }
    }

    void RetryMinigame()
    {
        // Ẩn nút Retry ngay lập tức sau khi bấm
        retryButton.gameObject.SetActive(false);

        // Ẩn kết quả thắng/thua
        fishingResultUI.SetActive(false);
        fishingWinImage.gameObject.SetActive(false);
        fishingFailImage.gameObject.SetActive(false);

        // Reset lại trạng thái minigame
        progressBar.value = progressBar.minValue; // Đặt lại giá trị thanh tiến trình
        isGameActive = true;  // Đảm bảo trò chơi có thể bắt đầu lại

        // Bạn có thể thêm các thao tác khác nếu cần
    }

    IEnumerator ShowMinigameAfterDelay()
    {
        // Đợi 1 frame hoặc thời gian đủ dài
        yield return null;  // Đợi đến frame kế tiếp

        // Hiển thị lại Minigame UI sau khi đã ẩn kết quả và nút retry
        minigameUI.SetActive(true);

        // Khởi động lại trò chơi và cho phép điều khiển lại
        isGameActive = true;
    }
}

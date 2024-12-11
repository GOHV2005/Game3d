using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject NPCPanel; // Panel hiển thị
    public GameObject Mission;
    public TextMeshProUGUI MissionContent;
    public GameObject Missioncomplet;
    public TextMeshProUGUI MissioncompletContent;
    public GameObject NPCcomplet;
    public TextMeshProUGUI NPCcompletContent;

    public TextMeshProUGUI NPCContent; // Nội dung NPC

    public string[] content; // Nội dung đối thoại khi nhiệm vụ chưa hoàn thành
    public string[] NPCcompletcontent; // Nội dung khi NPC hoàn thành nhiệm vụ

    private Coroutine coroutine;
    private bool isMissionComplete = false; // Kiểm tra trạng thái hoàn thành nhiệm vụ

    void Start()
    {
        NPCPanel.SetActive(false);
        NPCcomplet.SetActive(false);
        Mission.SetActive(false);
        Missioncomplet.SetActive(false);
        MissionContent.text = "Nhiệm vụ: Chưa hoàn thành";
        MissioncompletContent.text = "Nhiệm vụ: Hoàn thành!";
        NPCcompletContent.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Nếu nhiệm vụ chưa hoàn thành, hiển thị nội dung đối thoại NPCContent
            if (!isMissionComplete)
            {
                NPCPanel.SetActive(true); // Hiển thị bảng đối thoại NPC
                coroutine = StartCoroutine(ReadContent());
                NPCcomplet.SetActive(false);
            }
            // Nếu nhiệm vụ đã hoàn thành, hiển thị đối thoại NPC hoàn thành nhiệm vụ
            else
            {
                Mission.SetActive(false); // Tắt bảng nhiệm vụ
                Missioncomplet.SetActive(true); // Hiển thị bảng nhiệm vụ hoàn thành
                NPCcomplet.SetActive(true); // Hiển thị NPC hoàn thành nhiệm vụ
                coroutine = StartCoroutine(NPCcompletconten());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NPCPanel.SetActive(false);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            NPCcomplet.SetActive(false);
        }
    }

    // Coroutine hiển thị nội dung đối thoại NPC khi nhiệm vụ chưa hoàn thành
    private IEnumerator ReadContent()
    {
        foreach (var line in content)
        {
            NPCContent.text = "";
            foreach (var item in line)
            {
                NPCContent.text += item;
                yield return new WaitForSeconds(0.1f); // Thời gian hiển thị mỗi ký tự
            }
            yield return new WaitForSeconds(0.5f); // Thời gian nghỉ giữa các câu
        }
    }

    // Coroutine hiển thị nội dung khi NPC hoàn thành nhiệm vụ
    private IEnumerator NPCcompletconten()
    {
        foreach (var line in NPCcompletcontent)
        {
            NPCcompletContent.text = "";
            foreach (var item in line)
            {
                NPCcompletContent.text += item;
                yield return new WaitForSeconds(0.2f); // Thời gian hiển thị mỗi ký tự
            }
            yield return new WaitForSeconds(0.5f); // Thời gian nghỉ giữa các câu
        }
    }

    // Hàm để kết thúc cuộc trò chuyện
    public void endContent()
    {
        // Ẩn bảng đối thoại NPC
        NPCPanel.SetActive(false);
        Mission.SetActive(true); // Hiển thị bảng nhiệm vụ khi nhấn nút kết thúc
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    // Hàm để đánh dấu nhiệm vụ hoàn thành
    public void CompleteMission()
    {
        isMissionComplete = true;

        // Cập nhật lại nội dung bảng nhiệm vụ
        MissionContent.text = "Nhiệm vụ: Đã hoàn thành!";
    }

    // Hàm để reset nhiệm vụ nếu cần thiết
    public void ResetMission()
    {
        isMissionComplete = false;

        // Tắt bảng nhiệm vụ hoàn thành
        Missioncomplet.SetActive(false);
        NPCcomplet.SetActive(false);

        // Cập nhật lại nội dung bảng nhiệm vụ
        MissionContent.text = "Nhiệm vụ: Chưa hoàn thành";
    }
}
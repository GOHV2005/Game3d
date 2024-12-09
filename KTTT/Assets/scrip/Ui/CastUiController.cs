using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastUiController : MonoBehaviour
{
    [SerializeField] private GameObject sliderGO; // Thanh trượt UI
    [SerializeField] private Slider slider;       // Thanh trượt Slider
    [SerializeField] private Text text;           // Văn bản hiển thị giá trị

    private bool isSliderActive = false;          // Kiểm tra xem thanh trượt có đang hiển thị không

    private void Start()
    {
        sliderGO.SetActive(false);  // Ban đầu ẩn thanh trượt
    }

    // Hiển thị hoặc ẩn thanh trượt khi cần
    public void SetCastSliderActive(bool isActive)
    {
        if (isActive)
        {
            sliderGO.SetActive(true);  // Hiển thị thanh trượt
        }
        else
        {
            sliderGO.SetActive(false); // Ẩn thanh trượt
            slider.value = 0f;          // Đặt lại giá trị thanh trượt
            text.text = "0";            // Đặt lại giá trị văn bản
        }
    }

    // Cập nhật giá trị của thanh trượt
    public void SetCastPowerSlider(float val)
    {
        // Cập nhật giá trị của thanh trượt và văn bản
        slider.value = val / 100f;
        text.text = ((int)(val)).ToString();  // Hiển thị giá trị dưới dạng số nguyên
    }

    // Lấy giá trị của thanh trượt
    public float GetCastPowerSlider()
    {
        return slider.value * 100f;  // Trả về giá trị dưới dạng phần trăm
    }
}

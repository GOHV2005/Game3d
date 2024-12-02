using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [SerializeField] private List<Floaters> floaters = new List<Floaters>();

    [SerializeField] private GameObject waterObject; // GameObject làm nước

    [SerializeField] private float underWaterDrag = 3f;
    [SerializeField] private float underWaterAngularDrag = 1f;
    [SerializeField] private float defaultDrag = 0f;
    [SerializeField] private float defaultAngularDrag = 0.05f;

    private bool isUnderWater = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool isUnderWater = false;

        // Xác định vị trí mặt nước (trục Y của GameObject)
        float waterLine = waterObject.transform.position.y;

        // Kiểm tra trạng thái của từng floater
        foreach (var floater in floaters)
        {
            if (floater.FloaterUpdate(rb, waterLine))
                isUnderWater = true; // Nếu bất kỳ floater nào chìm, thì cả đối tượng được coi là chìm
        }

        // Thiết lập trạng thái vật lý
        SetState(isUnderWater);
    }

    private void SetState(bool isUnderWater)
    {
        if (isUnderWater)
        {
            rb.drag = underWaterDrag;
            rb.angularDrag = underWaterAngularDrag;
        }
        else
        {
            rb.drag = defaultDrag;
            rb.angularDrag = defaultAngularDrag;
        }
    }

    public bool GetIsUnderWater()
    {
        return isUnderWater;
    }
}

[System.Serializable]
public class Floaters
{
    [SerializeField] private float floatingPower = 20f; // Lực đẩy
    [SerializeField] private Transform floater;         // Vị trí floater

    public bool FloaterUpdate(Rigidbody rb, float waterLine)
    {
        // Tính sự chênh lệch giữa vị trí của floater và mặt nước
        float difference = floater.position.y - waterLine;

        // Nếu floater chìm dưới mặt nước
        if (difference < 0)
        {
            // Áp dụng lực đẩy lên
            rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floater.position, ForceMode.Force);
            return true; // Floater đang chìm
        }

        return false; // Floater không chìm
    }
}

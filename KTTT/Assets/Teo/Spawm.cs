using UnityEngine;

public class RandomSpawnController : MonoBehaviour
{
    public GameObject prefab;          // Prefab đối tượng sẽ xuất hiện
    public Vector2 mapSize = new Vector2(10, 10); // Kích thước mặt phẳng
    public float spawnInterval = 3f;  // Thời gian giữa mỗi lần xuất hiện
    public float objectLifetime = 2f; // Thời gian tồn tại của đối tượng

    private void Start()
    {
        InvokeRepeating("SpawnRandomObject", 0, spawnInterval); // Gọi hàm liên tục
    }

    void SpawnRandomObject()
    {
        // Tạo vị trí ngẫu nhiên trên mặt phẳng
        Vector3 randomPosition = new Vector3(
            Random.Range(-mapSize.x / 2, mapSize.x / 2),
            0.5f, // Cao hơn mặt phẳng để hiện rõ
            Random.Range(-mapSize.y / 2, mapSize.y / 2)
        );

        // Tạo đối tượng tại vị trí ngẫu nhiên
        GameObject spawnedObject = Instantiate(prefab, randomPosition, Quaternion.identity);

        // Xóa đối tượng sau một khoảng thời gian
        Destroy(spawnedObject, objectLifetime);
    }
}

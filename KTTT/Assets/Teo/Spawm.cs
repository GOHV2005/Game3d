using UnityEngine;

public class RandomSpawnController : MonoBehaviour
{
    public float docao;
    public GameObject prefab;          // Prefab đối tượng sẽ xuất hiện
    public Vector2 mapSize = new Vector2(10, 10); // Kích thước mặt phẳng (x, z)
    public float spawnInterval = 3f;  // Thời gian giữa mỗi lần xuất hiện
    public float objectLifetime = 2f; // Thời gian tồn tại của đối tượng

    private void Start()
    {
        // Gọi hàm liên tục để tạo đối tượng
        InvokeRepeating("SpawnRandomObject", 0, spawnInterval);
    }

    void SpawnRandomObject()
    {
        // Tạo vị trí ngẫu nhiên trên mặt phẳng (y = 0)
        Vector3 randomPosition = new Vector3(
            Random.Range(-mapSize.x / 2, mapSize.x / 2), // Trục x
            docao,                                          // Trục y
            Random.Range(-mapSize.y / 2, mapSize.y / 2) // Trục z
        );

        // Tạo đối tượng tại vị trí ngẫu nhiên
        GameObject spawnedObject = Instantiate(prefab, randomPosition, Quaternion.identity);

        // Xóa đối tượng sau một khoảng thời gian
        Destroy(spawnedObject, objectLifetime);
    }
}

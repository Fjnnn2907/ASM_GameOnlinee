using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSpawner : MonoBehaviour
{
    public GameObject lavaPrefab;
    public float spawnRadius = 20f;
    //public float spawnInterval = 3f; //Thoi gian spamspam
    public int numberOfLavaToSpawn = 10;
    public float lavaLifetime = 2f;
    private bool hasSpawned = false;
    private void Start()
    {
        if (!hasSpawned)
        {
            SpawnLava();
            hasSpawned = true;
        }
        //StartCoroutine(SpawnLava());
    }
    private void SpawnLava()
    {
        for (int i = 0; i < numberOfLavaToSpawn; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle * spawnRadius;
            Vector2 randomPosition = (Vector2)transform.position + randomDirection;
            if (lavaPrefab != null)
            {
                GameObject lava = Instantiate(lavaPrefab, randomPosition, Quaternion.identity);
                Destroy(lava, lavaLifetime);
            }
        }
    }
    // private IEnumerator SpawnLava()
    // {
    //     while (true)
    //     {
    //         for (int i = 0; i < numberOfLavaToSpawn; i++)
    //         {
    //             // Tạo vị trí ngẫu nhiên trong phạm vi spawnRadius (chỉ sử dụng trục X và Y)
    //             Vector2 randomDirection = Random.insideUnitCircle * spawnRadius;

    //             // Tạo vị trí spawn bằng cách cộng vị trí gốc (this.transform.position) với vector ngẫu nhiên
    //             Vector2 randomPosition = (Vector2)transform.position + randomDirection;

    //             // Tạo đối tượng lava tại vị trí ngẫu nhiên với rotation mặc định
    //             if (lavaPrefab != null)
    //             {
    //                 GameObject lava = Instantiate(lavaPrefab, randomPosition, Quaternion.identity);
    //                 Destroy(lava, lavaLifetime);
    //             }
    //         }
    //         yield return new WaitForSeconds(spawnInterval);
    //     }
    // }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

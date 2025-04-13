using System.Collections;
using Photon.Pun;
using UnityEngine;

public class IceSpikeSpawner : MonoBehaviourPun
{
    public GameObject iceSpikePrefab;
    public float spacing = 0.01f; // khoảng cách giữa các cục băng
    public float spawnDelay = 0.1f; // thời gian delay giữa mỗi lần mọc
    public LayerMask enemyLayer;

    public void SpawnIceSpikes(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(SpawnSequence(startPos, targetPos));
    }

    IEnumerator SpawnSequence(Vector2 startPos, Vector2 targetPos)
    {
        Vector2 dir = (targetPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, targetPos);
        int spikeCount = Mathf.FloorToInt(distance / spacing);

        for (int i = 0; i <= spikeCount; i++)
        {
            Vector2 spawnPos = startPos + dir * i * spacing;

            // Giữ nguyên rotation mặc định của prefab
            GameObject spike = PhotonNetwork.Instantiate(iceSpikePrefab.name, spawnPos, Quaternion.identity);
            // Nếu muốn xử lý hướng animation mà không xoay transform:
            // Có thể đổi hướng animation trong Animator hoặc scale.X nếu cần

            // Gây damage nếu enemy trong vùng
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.3f, enemyLayer);
            if (hit != null)
            {
                // Gây damage tại đây
                Debug.Log("Enemy trúng đòn!");
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}

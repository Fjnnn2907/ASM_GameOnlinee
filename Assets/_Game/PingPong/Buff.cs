using Photon.Pun;
using UnityEngine;

public class BuffSpawner : MonoBehaviourPun
{
    public GameObject[] buffPrefabs;
    public float spawnInterval = 10f;
    public Transform spawnAreaTopLeft;
    public Transform spawnAreaBottomRight;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            InvokeRepeating(nameof(SpawnBuff), 5f, spawnInterval);
    }

    void SpawnBuff()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaTopLeft.position.x, spawnAreaBottomRight.position.x),
            spawnAreaTopLeft.position.y
        );

        int index = Random.Range(0, buffPrefabs.Length);
        PhotonNetwork.Instantiate(buffPrefabs[index].name, spawnPos, Quaternion.identity);
    }
}

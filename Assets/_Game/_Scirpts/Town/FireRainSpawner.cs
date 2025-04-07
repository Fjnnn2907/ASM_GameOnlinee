using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRainSpawner : MonoBehaviour
{
    public GameObject fireRainPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 3f;
    private bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnFireRain());
    }

    private IEnumerator SpawnFireRain()
    {
        while (isSpawning)
        {
            Vector2 randomDirection = Random.insideUnitCircle * spawnRadius;

            Vector2 randomPosition = (Vector2)transform.position + randomDirection;

            if (fireRainPrefab != null)
            {
                var spawnRain = Instantiate(fireRainPrefab, randomPosition, Quaternion.identity);
                Destroy(spawnRain, 2f);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

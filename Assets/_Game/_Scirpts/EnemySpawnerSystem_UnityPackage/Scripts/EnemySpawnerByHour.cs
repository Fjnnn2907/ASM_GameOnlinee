using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerByHour : MonoBehaviour
{
    [Header("Thời gian spawn")]
    public int startHour = 18;
    public int endHour = 6;
    private bool spawningActive = false;

    [Header("Vùng spawn")]
    public float spawnRadius = 10f;
    public Color circleColor = Color.red;

    [Header("Cài đặt wave")]
    public GameObject[] enemyPrefabs;
    public int baseEnemyCount = 3;
    public float spawnDelay = 1f;

    [Header("Độ khó")]
    public float waveMultiplier = 1.2f;
    public float statMultiplierPerWave = 0.1f;

    public int currentWave = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();

    private void Start()
    {
        WorldTime.OnHourChanged += CheckSpawnTime;
    }

    private void OnDestroy()
    {
        WorldTime.OnHourChanged -= CheckSpawnTime;
    }

    private void CheckSpawnTime(int hour)
    {
        bool activeTime = IsWithinSpawnTime(hour);

        if (activeTime && !spawningActive)
        {
            spawningActive = true;
            //currentWave = 0;
            SpawnNextWave();
        }
        else if (!activeTime && spawningActive)
        {
            spawningActive = false;
        }
    }

    private bool IsWithinSpawnTime(int hour)
    {
        if (startHour < endHour)
            return hour >= startHour && hour < endHour;
        else
            return hour >= startHour || hour < endHour;
    }

    private void Update()
    {
        if (!spawningActive) return;

        currentEnemies.RemoveAll(e => e == null);

        if (currentEnemies.Count == 0)
        {
            SpawnNextWave();
        }
    }

    private void SpawnNextWave()
    {
        currentWave++;
        int enemyCount = Mathf.FloorToInt(baseEnemyCount * currentWave * waveMultiplier);
        StartCoroutine(SpawnEnemies(enemyCount));
    }

    private IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
            currentEnemies.Add(enemy);

            var stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                float multiplier = 1f + statMultiplierPerWave * currentWave;
                //stats.ApplyDifficultyScaling(multiplier);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector2 spawnPosition = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class EnemyTypeData
{
    public GameObject prefab;
    public int cost = 1;
    public int weight = 1;
}

[System.Serializable]
public class BossData
{
    public GameObject prefab;
    public int spawnEveryXWave = 2;
}

[System.Serializable]
public class TrueMiniBossData
{
    public GameObject prefab;
    public int spawnEveryXEnemies = 20;
}

public class EnemySpawnerByHour : MonoBehaviourPunCallbacks
{
    [Header("Thời gian spawn")]
    public int startHour = 18;
    public int endHour = 6;
    private bool spawningActive = false;

    [Header("Vùng spawn")]
    public float spawnRadius = 10f;
    public Color circleColor = Color.red;

    [Header("Cài đặt wave")]
    public List<EnemyTypeData> enemyTypes;
    public List<BossData> bosses;
    public List<TrueMiniBossData> trueMiniBosses;

    public int maxEnemyPerWave = 12;
    public float autoWaveDelay = 10f;

    [Header("Số wave mỗi ngày")]
    public int baseWavePerDay = 3;
    private int currentDay = 1;
    private int waveLimitToday;
    private int waveSpawnedToday = 0;

    //[Header("Độ khó")]
    //public float statMultiplierPerWave = 0.1f;

    [Header("Độ khó tăng theo wave")]
    public float enemyPerWaveMultiplier = 0.1f; 

    private List<GameObject> currentEnemies = new List<GameObject>();
    private float autoWaveTimer;
    public int currentWave = 0;
    private int totalEnemySpawned = 0;

    private void Start()
    {
        WorldTime.OnHourChanged += CheckSpawnTime;
        WorldTime.OnDayChanged += ResetWavePerDay;
        waveLimitToday = baseWavePerDay;

        CheckSpawnTime(WorldTime.Instance.Hour);
    }

    private void OnDestroy()
    {
        WorldTime.OnHourChanged -= CheckSpawnTime;
        WorldTime.OnDayChanged -= ResetWavePerDay;
    }

    private void ResetWavePerDay(int day)
    {
        currentDay = day;
        waveSpawnedToday = 0;
        waveLimitToday = baseWavePerDay + (day - 1);
    }

    private void CheckSpawnTime(int hour)
    {
        bool activeTime = IsWithinSpawnTime(hour);
        if (activeTime && !spawningActive)
        {
            spawningActive = true;
            autoWaveTimer = autoWaveDelay;
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
        if (!PhotonNetwork.IsMasterClient) return; // Chỉ host mới spawn

        if (!spawningActive) return;

        if (waveSpawnedToday >= waveLimitToday) return;

        autoWaveTimer -= Time.deltaTime;

        if (autoWaveTimer <= 0f)
        {
            autoWaveTimer = autoWaveDelay;
            SpawnNextWave(); // Gọi spawn
        }
    }


    private void SpawnNextWave()
    {
        currentWave++;
        waveSpawnedToday++;

        List<GameObject> waveEnemies = GenerateOptimizedEnemyList();
        List<string> enemyNames = new List<string>();

        foreach (var enemy in waveEnemies)
        {
            enemyNames.Add(enemy.name); // Gửi tên prefab
        }

        photonView.RPC("RPC_SpawnEnemies", RpcTarget.All, enemyNames.ToArray(), currentWave);

        // Gọi boss & miniboss
        foreach (var boss in bosses)
        {
            if (currentWave % boss.spawnEveryXWave == 0)
            {
                photonView.RPC("RPC_SpawnBoss", RpcTarget.All, boss.prefab.name);
            }
        }

        foreach (var mini in trueMiniBosses)
        {
            if (waveEnemies.Count % mini.spawnEveryXEnemies == 0)
            {
                photonView.RPC("RPC_SpawnTrueMiniBoss", RpcTarget.All, mini.prefab.name);
            }
        }
    }

    [PunRPC]
    void RPC_SpawnEnemies(string[] enemyNames, int syncedWave)
    {
        currentWave = syncedWave;

        StartCoroutine(SpawnEnemiesByName(enemyNames));
    }

    IEnumerator SpawnEnemiesByName(string[] names)
    {
        foreach (string name in names)
        {
            GameObject prefab = enemyTypes.Find(e => e.prefab.name == name)?.prefab;
            if (prefab != null)
            {
                Vector2 spawnPos = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                currentEnemies.Add(enemy);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    [PunRPC]
    void RPC_SpawnBoss(string bossName)
    {
        var prefab = bosses.Find(b => b.prefab.name == bossName)?.prefab;
        if (prefab != null)
        {
            Vector2 spawnPos = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    [PunRPC]
    void RPC_SpawnTrueMiniBoss(string miniBossName)
    {
        var prefab = trueMiniBosses.Find(m => m.prefab.name == miniBossName)?.prefab;
        if (prefab != null)
        {
            Vector2 spawnPos = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    private List<GameObject> GenerateOptimizedEnemyList()
    {
        int totalCost = Mathf.FloorToInt(maxEnemyPerWave * Mathf.Pow(enemyPerWaveMultiplier, currentWave)); // wave multiplier
        List<GameObject> selectedEnemies = new List<GameObject>();

        // Tính tổng trọng số
        int totalWeight = 0;
        foreach (var enemy in enemyTypes)
        {
            totalWeight += enemy.weight;
        }

        // Gacha enemy theo trọng số đến khi hết cost
        while (totalCost > 0)
        {
            int rand = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var enemy in enemyTypes)
            {
                cumulative += enemy.weight;
                if (rand < cumulative)
                {
                    if (enemy.cost <= totalCost)
                    {
                        selectedEnemies.Add(enemy.prefab);
                        totalCost -= enemy.cost;
                    }
                    break;
                }
            }
        }

        return selectedEnemies;
    }



    private IEnumerator SpawnEnemies(List<GameObject> enemies)
    {
        foreach (var enemyPrefab in enemies)
        {
            Vector2 spawnPosition = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemies.Add(enemy);

            totalEnemySpawned++; // Tăng tổng số enemy thường

            // Spawn miniboss thật dựa trên số enemy đã spawn
            foreach (var mini in trueMiniBosses)
            {
                if (mini.prefab != null && totalEnemySpawned % mini.spawnEveryXEnemies == 0)
                {
                    Vector2 miniBossPos = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnRadius);
                    Instantiate(mini.prefab, miniBossPos, Quaternion.identity);
                }
            }

            //var stats = enemy.GetComponent<EnemyStats>();
            //if (stats != null)
            //{
            //    float multiplier = 1f + statMultiplierPerWave * currentWave;
            //    stats.ApplyDifficultyScaling(multiplier);
            //}

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

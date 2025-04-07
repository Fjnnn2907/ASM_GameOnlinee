using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;    
    public Transform spawnPoint;
    public float speed = 10f;
    public float fireRate = 3f;
    public float detectRange = 10f;

    private float fireTimer;

    private TownHealth townHealth;
    void Start()
    {
        townHealth = GetComponent<TownHealth>();
    }
    void Update()
    {
        if (townHealth.GetHealth() <= 0) return;
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            SpawnFireballsToEnemiesInRange();
        }
    }

    void SpawnFireballsToEnemiesInRange()
    {
        if (townHealth.GetHealth() <= 0) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance <= detectRange)
            {
                SpawnFireball(enemy.transform);
            }
        }
    }

    void SpawnFireball(Transform target)
    {
        GameObject fireball = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);

        FireballMovement movement = fireball.AddComponent<FireballMovement>();
        movement.target = target;
        movement.speed = speed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBallHitPet : MonoBehaviour
{
    [SerializeField] int minDamage = 0;
    [SerializeField] int maxDamage = 100;
    [SerializeField] float critChance = 0.2f; // 20% chí mạng
    [SerializeField] float critMultiplier = 2f; // x2 dame
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy"))
        {
            int damage = Random.Range(minDamage, maxDamage + 1);
            bool isCritical = Random.value <= critChance;
            if (isCritical)
            {
                damage = Mathf.RoundToInt(damage * critMultiplier);
                Debug.Log("Crit Damage = " + damage);
            }
            var enemyStats = other.gameObject.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }
        }
    }
}

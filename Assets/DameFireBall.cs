using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DameFireBall : MonoBehaviour
{   
    [SerializeField] int minDamage = 0;
    [SerializeField] int maxDamage = 100;
    [SerializeField] float critChance = 0.2f; // 20% chí mạng
    [SerializeField] float critMultiplier = 2f; // x2 dame
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            int damage = Random.Range(minDamage, maxDamage + 1);
            bool isCritical = Random.value <= critChance;
            if (isCritical)
            {
                damage = Mathf.RoundToInt(damage * critMultiplier);
                Debug.Log("Crit Damage = " + damage);
            }
            var playerHealth = collision.gameObject.GetComponent<PlayerStats>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}

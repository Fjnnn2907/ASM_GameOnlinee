using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostbite : MonoBehaviour
{
    [SerializeField] int minDamage = 0;
    [SerializeField] int maxDamage = 100;
    [SerializeField] float critChance = 0.2f;
    [SerializeField] float critMultiplier = 2f;

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            int damage = Random.Range(minDamage, maxDamage + 1);
            bool isCritical = Random.value <= critChance;
            if (isCritical)
            {
                damage = Mathf.RoundToInt(damage * critMultiplier);
                Debug.Log("Crit Damage = " + damage);
            }

            var enemyHealth = collision.gameObject.GetComponent<EnemyStats>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}

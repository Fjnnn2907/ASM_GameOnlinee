using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FireballMovement : MonoBehaviourPun
{
    public Transform target;
    public float speed = 10f;
    private Vector3 direction;
    private bool lostTarget = false;

    void Start()
    {
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            lostTarget = true;
            Destroy(gameObject, 4f);
        }
    }
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            // Quay đầu đạn
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Di chuyển theo hướng transform.right
            transform.position += transform.right * speed * Time.deltaTime;
        }
        else if (!lostTarget)
        {
            lostTarget = true;
            Destroy(gameObject, 4f);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
            Destroy(gameObject);
        }
    }
}

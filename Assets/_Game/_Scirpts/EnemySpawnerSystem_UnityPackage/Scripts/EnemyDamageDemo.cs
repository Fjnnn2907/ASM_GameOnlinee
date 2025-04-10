using UnityEngine;

public class EnemyDamageTester : MonoBehaviour
{
    public KeyCode damageKey = KeyCode.Space; // Bấm phím này để trừ máu
    public float testDamage = 25f;

    private EnemyStats stats;

    private void Start()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(damageKey) && stats != null)
        {
            stats.TakeDamage(testDamage);
        }
    }
}

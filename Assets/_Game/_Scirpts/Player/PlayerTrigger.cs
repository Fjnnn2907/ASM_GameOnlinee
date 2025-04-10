using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PlayerCtrl playerCtrl;

    private void Start()
    {
        playerCtrl = GetComponentInParent<PlayerCtrl>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemSO selectedItem = HotBarManager.instance.GetSelectedItem();
        if (selectedItem == null) return;

        if (selectedItem.action == ActionType.Attack)
        {
            var enemy = collision.GetComponent<EnemyStats>();
            if (enemy != null)
            {   
                enemy.TakeDamage(400);
            }
        }
    }
}

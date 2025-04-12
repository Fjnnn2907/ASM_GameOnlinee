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

        if (selectedItem.action == ActionType.Attack && collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyStats>();
            if (enemy != null)
            {   
                enemy.TakeDamage(400);
            }
        }
        if(selectedItem.action == ActionType.Axe && collision.CompareTag("Tree"))
        {
            var tree = collision.GetComponent<Tree>();
            if (tree != null)
                tree.OnHit();
        }
    }
}

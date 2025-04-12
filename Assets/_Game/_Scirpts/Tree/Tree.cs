using Photon.Pun;
using UnityEngine;

public class Tree : MonoBehaviourPun
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int currentHealth;
    public int maxHealth = 5;

    private bool isDead;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void OnHit()
    {
        if (isDead) return;

        if (currentHealth > 1)
        {
            currentHealth--;
            anim.SetTrigger("Hit");
        }
        else
        {
            isDead = true;
            PhotonNetwork.Instantiate(itemPrefab.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

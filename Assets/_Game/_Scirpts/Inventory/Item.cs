using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviourPun
{
    public ItemSO itemSO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tag.PLAYER))
            PhotonNetwork.Destroy(gameObject);
    }
}

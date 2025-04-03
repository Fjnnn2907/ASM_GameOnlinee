using Photon.Pun;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Start()
    {
        Vector2 posSpawn = new Vector2(Random.Range(-2, 2), 0);
        GameObject player = PhotonNetwork.Instantiate("Player", posSpawn, Quaternion.identity);
    }
}

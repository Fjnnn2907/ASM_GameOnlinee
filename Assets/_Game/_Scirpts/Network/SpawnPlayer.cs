using Photon.Pun;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Start()
    {
        string selectedHero = PlayerPrefs.GetString("SelectedHero", "Player");
        Vector2 posSpawn = new Vector2(Random.Range(-2, 2), 0);
        GameObject player = PhotonNetwork.Instantiate(selectedHero, posSpawn, Quaternion.identity);
    }
}

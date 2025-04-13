using Photon.Pun;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Start()
    {
        string selectedHero = PlayerPrefs.GetString("SelectedHero", "Player");
        Vector2 posSpawn = new Vector2(Random.Range(4, 6), 4);
        GameObject player = PhotonNetwork.Instantiate(selectedHero, posSpawn, Quaternion.identity);
        PhotonNetwork.LocalPlayer.TagObject = player.transform;
    }
}

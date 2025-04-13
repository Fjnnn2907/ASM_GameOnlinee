using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPet : MonoBehaviour
{
    private void Start()
    {
        string selectedPet = PlayerPrefs.GetString("SelectedPet", "");
        if (string.IsNullOrEmpty(selectedPet))
        {
            return;
        }
        Vector2 posSpawn = new Vector2(Random.Range(4, 6), 4);

        GameObject pet = PhotonNetwork.Instantiate(selectedPet, posSpawn, Quaternion.identity);

        StartCoroutine(AssignOwnerAfterDelay(pet));
    }

    private IEnumerator AssignOwnerAfterDelay(GameObject pet)
    {
        yield return new WaitForSeconds(0.1f);

        if (pet != null)
        {
            if (pet.TryGetComponent(out PhotonView petPV) && petPV.IsMine)
            {
                if (PhotonNetwork.LocalPlayer.TagObject != null)
                {
                    Transform playerTransform = PhotonNetwork.LocalPlayer.TagObject as Transform;
                    if (playerTransform != null)
                    {
                        if (pet.TryGetComponent<SlimzBlue>(out var slimzBlue))
                        {
                            slimzBlue.owner = playerTransform;
                        }

                        if (pet.TryGetComponent<SlimzPurl>(out var slimzPurl))
                        {
                            slimzPurl.owner = playerTransform;
                        }
                        if(pet.TryGetComponent<Unicorn>(out var unicorn))
                        {
                            unicorn.owner = playerTransform;
                        }
                    }
                }
            }
        }
    }
}

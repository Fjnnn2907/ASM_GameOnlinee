using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaAnimationHandler : MonoBehaviour
{
   //public Animator fireAnimator;
    public Animator lavaAnimator;
    public GameObject firePrefab;
    public Transform spawnPoint;
    private bool lavaPlayed = false;

    public void OnLavaAnimationEnd()
    {
        if (!lavaPlayed)
        {
            Destroy(lavaAnimator.gameObject);

            // Spawn đối tượng fire
            if (firePrefab != null && spawnPoint != null)
            {
                Instantiate(firePrefab, spawnPoint.position, Quaternion.identity);
            }
            lavaPlayed = true;
        }
    }
}

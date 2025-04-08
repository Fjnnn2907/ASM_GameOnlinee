using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballFall : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float fallDuration = 2f;
    public Animator fireballAnimator;
    public GameObject fireHitPrefab;
    public Transform spawnPoint;
    private bool firePlayed = false;
    private void Start()
    {
        StartCoroutine(FallAndDestroy());
    }

    private IEnumerator FallAndDestroy()
    {
        float elapsedTime = 0f;  // Timeime

        // Roi xuong theo phuong thang dungdung
        while (elapsedTime < fallDuration)
        {
            // Di chuyen fireball xuong theo truc YY
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        OnLavaAnimationEnd();
        Destroy(gameObject, 1f);
    }
    public void OnLavaAnimationEnd()
    {
        if (!firePlayed)
        {
            Destroy(fireballAnimator.gameObject);

            if (fireHitPrefab != null && spawnPoint != null)
            {
                var fireHitIn = Instantiate(fireHitPrefab, spawnPoint.position, Quaternion.identity);
                Destroy(fireHitIn, 1f);
            }
            firePlayed = true;
        }
    }
}

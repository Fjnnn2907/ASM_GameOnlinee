using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPet : MonoBehaviour
{
    public float expandSpeed = 2f;
    public float maxScale = 3f;
    public float existTime = 2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(ExpandAndDestroy());
    }

    IEnumerator ExpandAndDestroy()
    {
        float timer = 0f;

        while (transform.localScale.x < maxScale)
        {
            float scaleStep = expandSpeed * Time.deltaTime;
            transform.localScale += new Vector3(scaleStep, scaleStep, 0);
            timer += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(existTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(KnockbackSmooth(other.transform));
        }
    }

    IEnumerator KnockbackSmooth(Transform enemy)
    {
        float knockDuration = 0.3f;
        float knockSpeed = 6f;
        float timer = 0f;

        Vector2 direction = (enemy.position - transform.position).normalized;

        while (timer < knockDuration)
        {
            timer += Time.deltaTime;

            enemy.position += (Vector3)(direction * knockSpeed * Time.deltaTime);

            yield return null;
        }
    }
}

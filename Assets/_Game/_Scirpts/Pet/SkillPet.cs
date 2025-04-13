using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPet : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(10f, 10f, 10f);
    public float duration = 1f;

    private Vector3 initialScale;
    private float timer = 0f;
    private bool scaling = true;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (!scaling) return;

        timer += Time.deltaTime;
        transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);

        if (timer >= duration)
        {
            transform.localScale = targetScale;
            scaling = false;
        }
    }
}

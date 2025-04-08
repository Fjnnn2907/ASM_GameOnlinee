using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LegendaryTextEffect : MonoBehaviour
{
    public TextMeshProUGUI legendaryText;
    public Animator animator;

    public void PlayLegendaryAnimation()
    {
        legendaryText.text = "Congratulations MinhHandSome for winning Legendary";
        gameObject.SetActive(true);
        animator.Play("TextNoti", -1, 0f);
        StartCoroutine(HideAfterDelay(2f));
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}

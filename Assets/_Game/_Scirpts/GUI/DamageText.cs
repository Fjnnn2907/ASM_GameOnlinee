using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    public void ShowDamage(int damage)
    {
        damageText.text = "-" + damage.ToString();
        StartCoroutine(DestroyAfterSeconds(2f));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}

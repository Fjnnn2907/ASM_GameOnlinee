using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gacha : MonoBehaviour
{
    [SerializeField] protected List<GachaItem> gachaItems = new();
    
    [Header("Obj")]
    [SerializeField] protected GameObject ShowResultGacha;
    [SerializeField] protected Animator anim;

    [Header("GUI")]
    [SerializeField] protected Button spinButton;
    [SerializeField] protected Image resultImage;
    [SerializeField] protected TextMeshProUGUI resultText;

    protected void Start()
    {
        anim.GetComponent<Animator>();
        spinButton.onClick.AddListener(SpinGacha);
    }
    public void SpinGacha()
    {
        if (gachaItems.Count == 0)
            return;

        CoinManager.Instance.RemoveDiamond(250);

        float totalRandom = 0;
        foreach (GachaItem item in gachaItems)
        {
            totalRandom += item.dropRate;
        }

        float random = Random.Range(0, totalRandom);
        float current = 0;
        foreach (GachaItem item in gachaItems)
        {
            current += item.dropRate;

            if (random < current)
            {
                ShowResult(item);

                if (item.isCoin)
                    CoinManager.Instance.AddCoin(item.coinAmount);
                else if (item.heroItem != null)
                    item.heroItem.SetActive(true);

                if (item.isOnce)
                    gachaItems.Remove(item);

                return;
            }
        }

    }
    public void ShowResult(GachaItem item)
    {
        ShowResultGacha.SetActive(true);
        anim.Play("In");
        resultImage.sprite = item.itemIcon;
        resultText.text = item.itemName;

    }
}

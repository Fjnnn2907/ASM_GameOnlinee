using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyButton : MonoBehaviour
{
    [SerializeField] private Button Button_Coin;
    [SerializeField] private TextMeshProUGUI Text_Value;
    [SerializeField] private GameObject itemHeroes;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip soundBuyItem;
    [SerializeField] private int priceCoin;

    private void Start()
    {
        Button_Coin.onClick.AddListener(BuyItem);
    }

    public void BuyItem()
    {
        if(!CoinManager.Instance.RemoveCoin(priceCoin))
            return;

        audioManager.AudioButton(soundBuyItem);
        Button_Coin.interactable = false;
        Text_Value.text = "Sold";
        itemHeroes.SetActive(true);
        
    }
}

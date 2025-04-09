using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyCoin : MonoBehaviour
{
    [SerializeField] private Button Button_Cost_Gem;
    [SerializeField] private int PriceGem;
    [SerializeField] private int goldReceived;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip soundBuyItem;
    private void Start()
    {
        Button_Cost_Gem.onClick.AddListener(BuyCoinCostGem);
    }

    public void BuyCoinCostGem()
    {
        var coin = CoinManager.Instance;
        if (!coin || coin.Diamond <= 0)
            return;

        audioManager.AudioButton(soundBuyItem);
        coin.RemoveDiamond(PriceGem);
        coin.AddCoin(goldReceived);
    }
}

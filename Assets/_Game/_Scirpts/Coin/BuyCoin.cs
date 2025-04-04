using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyCoin : MonoBehaviour
{
    [SerializeField] private Button Button_Cost_Gem;
    [SerializeField] private int PriceGem;
    [SerializeField] private int goldReceived;
    private void Start()
    {
        Button_Cost_Gem.onClick.AddListener(BuyCoinCostGem);
    }

    public void BuyCoinCostGem()
    {
        CoinManager.Instance.RemoveDiamond(PriceGem);
        CoinManager.Instance.AddCoin(goldReceived);
    }
}

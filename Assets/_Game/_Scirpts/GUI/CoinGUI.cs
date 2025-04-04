using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;
    private void OnEnable()
    {
        Obsever.AddObsever("UpdateCoin", UpdateCoin);
        Obsever.AddObsever("UpdateDiamond", UpdateDiamond);
    }
    private void OnDisable()
    {
        Obsever.RemoveObsever("UpdateCoin", UpdateCoin);
        Obsever.AddObsever("UpdateDiamond", UpdateDiamond);
    }
    private void UpdateCoin()
    {
        coinText.text = CoinManager.Instance.Coin.ToString();
    }
    private void UpdateDiamond()
    {
        diamondText.text = CoinManager.Instance.Diamond.ToString();
    }
}

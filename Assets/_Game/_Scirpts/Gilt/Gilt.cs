using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gilt : MonoBehaviour
{
    public List<GiftCodeData> giftCodeDatas = new();

    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button okBtn;

    private void Start()
    {
        okBtn.onClick.AddListener(InputCode);
    }

    public void InputCode(string code)
    {
        foreach(var codeData in giftCodeDatas)
        {
            if(codeData.code == code)
            {
                CoinManager.Instance.AddCoin(codeData.coinReward);
                CoinManager.Instance.AddDiamond(codeData.diamondReward);
                giftCodeDatas.Remove(codeData);
                return;
            }
        }
    }

    public void InputCode()
    {
        string code = codeInput.text;
        InputCode(code);
        codeInput.text = "";
    }

}

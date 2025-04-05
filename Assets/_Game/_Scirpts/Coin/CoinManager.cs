using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private static CoinManager instance;
    public static CoinManager Instance => instance;
    private int coin = 0;
    public int Coin => coin;

    private int diamond = 0;
    public int Diamond => diamond;

    private void Awake() => instance = this;

    private void Start()
    {
        AddCoin(500);
        AddDiamond(1000);
    }

    public void AddCoin(int coin)
    {
        this.coin += coin;
        Obsever.Notify("UpdateCoin");
    }

    public bool RemoveCoin(int coin)
    {
        if(this.coin >= coin)
        {
            this.coin -= coin;

            Obsever.Notify("UpdateCoin");
            return true;
        }
        return false;
    }
    public void AddDiamond(int diamond)
    {
        this.diamond += diamond;
        Obsever.Notify("UpdateDiamond");
    }
    public bool RemoveDiamond(int diamond)
    {
        if(this.diamond >= diamond)
        {
            this.diamond -= diamond;

            Obsever.Notify("UpdateDiamond");
            return true;
        }
        return false;
    }
}

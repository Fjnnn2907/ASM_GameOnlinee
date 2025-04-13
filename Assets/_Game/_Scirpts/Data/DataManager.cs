using UnityEngine;
using System.Collections.Generic;

public static class DataManager
{
    public static int coin = 0;
    public static int diamond = 0;

    private static Dictionary<string, int> data = new Dictionary<string, int>()
    {
        { "Coin", 0 },
        { "Diamond", 0 }
    };

    public static void SaveData()
    {
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("Diamond", diamond);
        PlayerPrefs.Save();
        Debug.Log("Data Saved: Coin = " + coin + ", Diamond = " + diamond);
    }

    public static void LoadData()
    {
        coin = PlayerPrefs.GetInt("Coin", 0);
        diamond = PlayerPrefs.GetInt("Diamond", 0);
        Debug.Log("Data Loaded: Coin = " + coin + ", Diamond = " + diamond);
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteKey("Coin");
        PlayerPrefs.DeleteKey("Diamond");
        PlayerPrefs.Save();
        coin = 0;
        diamond = 0;
        Debug.Log("Data Reset");
    }
}

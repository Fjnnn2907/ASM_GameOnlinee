using UnityEngine;

[System.Serializable]
public class GachaItem
{
    public string itemName;
    public Sprite itemIcon;
    public float dropRate;
    public bool isOnce;

    public bool isCoin;
    public int coinAmount;

    public GameObject heroItem;
}

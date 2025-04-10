using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Normal, Rare, Legendary, Coin, Weapons, Skin, Item }
public class PetSlotData : MonoBehaviour
{
    public Sprite icon;
    public Rarity rarity;
    public string pet;
}

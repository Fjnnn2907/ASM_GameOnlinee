using UnityEngine;

[CreateAssetMenu(fileName = ("ItemSO"))]
public class ItemSO : ScriptableObject
{
    public string id;
    public bool stackable;
    public Sprite image;


    [Space]
    public ItemType type;
    public ActionType action;
}
public enum ItemType
{
    Tool, Item
}
public enum ActionType
{
    None, Axe, Water, Attack, Dig, Plough, Seed, Basket
}

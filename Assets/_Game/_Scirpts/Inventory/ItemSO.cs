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
    Tool
}
public enum ActionType
{
    Axe, Water, Attack, Dig, Plough, Seed, Basket
}

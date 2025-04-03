using UnityEngine;

public class DemoHotBar : MonoBehaviour
{
    public HotBarManager hotBarManager;

    public ItemSO[] itemSOs;

    public void PickUpItem()
    {
        hotBarManager.AddItem(itemSOs[Random.Range(0, itemSOs.Length)]);
    }
    public void GetSelectItem()
    {
        hotBarManager.GetSelectedItem();
    }
}

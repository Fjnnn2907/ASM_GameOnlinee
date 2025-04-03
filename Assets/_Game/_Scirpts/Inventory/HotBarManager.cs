using UnityEngine;

public class HotBarManager : MonoBehaviour
{
    public static HotBarManager instance;

    public HotBarSlot[] hotBarSlots;
    public GameObject hotBarItemPrefab;

    private int selectedSlot = -1;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 8)
                ChangeSelectedSlot(number - 1);
        }
    }
    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
            hotBarSlots[selectedSlot].DeSelect();

        hotBarSlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(ItemSO item)
    {
        foreach (var slot in hotBarSlots)
        {
            HotBarItem itemSlot = slot.GetComponentInChildren<HotBarItem>();

            if (itemSlot == null)
            {
                SpawnItem(item, slot);
                return true;
            }
            else if (itemSlot.itemSO == item && itemSlot.count < 99 && itemSlot.itemSO.stackable)
            {
                itemSlot.count++;
                itemSlot.RefreshCount();
                return true;
            }
        }
        return false;
    }

    private void SpawnItem(ItemSO item, HotBarSlot slot)
    {
        GameObject newObj = Instantiate(hotBarItemPrefab, slot.transform);
        newObj.GetComponent<HotBarItem>().BeginItem(item);
    }

    public ItemSO GetSelectedItem()
    {
        if (selectedSlot < 0 || selectedSlot >= hotBarSlots.Length)
            return null;

        HotBarSlot slot = hotBarSlots[selectedSlot];
        HotBarItem itemInSlot = slot.GetComponentInChildren<HotBarItem>();

        if (itemInSlot != null)
            return itemInSlot.itemSO;

        return null;
    }
}
